using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Crypto;

namespace NetTCPSocket.TCPServer
{
    public class TCPSession : IDisposable
    {
        #region Variables

        public Guid Id { get; private set; }
        public NetworkStream Stream { get; private set; }
        public TCPServer server { get; private set; }
        protected internal TcpClient session { get; set; }


        private AES aes { get; set; } = new AES();
        private RSA rsa { get; set; }

        private Queue<string> QueueMessages { get; set; } = new Queue<string>();

        #endregion

        #region Event

        protected virtual void OnMessage(TCPSession session, Message message) { }

        protected virtual void OnError(TCPSession session, Exception e) { }

        #endregion

        public TCPSession(TCPServer serverObject)
        {
            Id = Guid.NewGuid();
            server = serverObject;
            serverObject.AddSession(this);
        }

        #region Connect/Disconnect

        public bool isConnected { get; private set; }

        public void Disconnect()
        {
            Send(new Message("", CommandMessage.DisconnectMessage));
            Abort();
        }

        private void Abort()
        {
            server.RemoveSession(this.Id);
            server.OnDisconnected(this); isConnected = false;
            if (Stream != null) Stream.Close();
            if (session != null) session.Close();
        }

        #endregion

        #region Receive/Send

        private void KeyExchange()
        {
            bool isSuccess = false;
            do
            {
                Receive();

                while (QueueMessages.Count != 0)
                {
                    var request = JsonConvert.DeserializeObject<Message>(QueueMessages.Dequeue());
                    isSuccess = AcceptKey(request);

                    if (isSuccess)
                    {
                        Message responce = new Message(CommandType.ACCEPTED, aes.Key_IV);
                        string responceKey = JsonConvert.SerializeObject(responce);
                        Send(rsa.Encrypt(responceKey));
                        break;
                    }
                }

            }
            while (!isSuccess);
        }

        private bool AcceptKey(Message message)
        {
            string command = message.Dequeue();
            if (command == CommandType.CONNECT)
            {
                string publicKey = message.value;
                rsa = new RSA(KeyType.Public, publicKey);
                return true;
            }

            return false;
        }

        protected internal void ProcessReceive()
        {
            try
            {
                Stream = session.GetStream();

                isConnected = true;
                KeyExchange();
                server.OnConnected(this);

                while (isConnected == true)
                {
                    Receive();

                    while (QueueMessages.Count != 0)
                    {
                        var request = JsonConvert.DeserializeObject<Message>(aes.Decrypt(QueueMessages.Dequeue()));
                        if (request.value == CommandMessage.DisconnectMessage) { Abort(); break; }
                        OnMessage(this, request);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message == CommandMessage.HostTerminated || e.Message == CommandMessage.ClientTerminated) { Abort();}
                else { OnError(this, e); Disconnect(); }
            }
        }

        private void Receive()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            foreach (string message in builder.ToString().Split(new string[] { CommandMessage.EndMessage }, StringSplitOptions.RemoveEmptyEntries))
            {
                QueueMessages.Enqueue(message);
            }
        }

        public void Send(Message message)
        {
            string msg = JsonConvert.SerializeObject(message);
            Send(aes.Encrypt(msg));
        }

        private void Send(string message)
        {
            if (isConnected == false) return;
            byte[] data = Encoding.Unicode.GetBytes(message + CommandMessage.EndMessage);
            Stream.Write(data, 0, data.Length);
        }

        #endregion

        public void Dispose() { Disconnect(); }
    }
}
