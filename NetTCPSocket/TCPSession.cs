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

        private const string EndMessage = "<EOF>";

        string disconnect = "Не удается прочитать данные из транспортного соединения: Удаленный хост принудительно разорвал существующее подключение.";

        #endregion

        #region Event

        protected virtual Message OnMessage(TCPSession session, Message message) { return new Message(); }

        protected virtual void OnError(TCPSession session, Exception e) { }

        #endregion

        public TCPSession(TCPServer serverObject)
        {
            Id = Guid.NewGuid();
            server = serverObject;
            serverObject.AddSession(this);
        }

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
                        Send(responce, rsa);
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

                KeyExchange();

                while (true)
                {
                    Receive();

                    while (QueueMessages.Count != 0)
                    {
                        var request = JsonConvert.DeserializeObject<Message>(aes.Decrypt(QueueMessages.Dequeue()));
                        Message responce = OnMessage(this, request);
                        Send(responce, aes);
                    }
                }
            }
            catch (Exception e) { if (e.Message != disconnect) OnError(this, e); Disconnect(); }
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

            foreach (string message in builder.ToString().Split(new string[] { EndMessage }, StringSplitOptions.RemoveEmptyEntries))
            {
                QueueMessages.Enqueue(message);
            }
        }

        private void Send(Message message, ICrypto cryptor)
        {
            string msg = JsonConvert.SerializeObject(message);
            byte[] data = Encoding.Unicode.GetBytes(cryptor.Encrypt(msg) + EndMessage);
            Stream.Write(data, 0, data.Length);
        }

        #endregion

        protected internal void Disconnect()
        {
            server.RemoveSession(this.Id);
            if (Stream != null) Stream.Close();
            if (session != null) session.Close();
        }

        public void Dispose() { Disconnect(); }
    }
}
