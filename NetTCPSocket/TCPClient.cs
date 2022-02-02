using Crypto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetTCPSocket.TCPClient
{
    public class TCPClient : IDisposable
    {
        #region Variables

        public IPAddress IP_Address { get; private set; }
        public int Port { get; private set; }
        public NetworkStream Stream { get; private set; }
        public TcpClient client { get; private set; }
        public Thread receiveThread { get; private set; }


        public AES aes { get; private set; }
        public RSA rsa { get; private set; } = new RSA();

        private Queue<string> QueueMessages { get; set; } = new Queue<string>();

        #endregion

        #region Handler and Event

        public delegate void InfoHandler(TCPClient client);

        public event InfoHandler OnConnected;

        public event InfoHandler OnDisconnected;

        public delegate void MessageHandler(TCPClient client, Message message);

        public event MessageHandler OnMessage;

        public delegate void ErrorHandler(TCPClient client, Exception e);

        public event ErrorHandler OnError;

        #endregion

        public TCPClient(string address, int port) : this(IPAddress.Parse(address), port) { }

        public TCPClient(IPAddress address, int port)
        {
            IP_Address = address;
            Port = port;
        }

        #region Connect/Disconnect

        public bool isConnected { get; private set; }

        public bool Connect()
        {
            client = new TcpClient();
            client.Connect(IP_Address, Port);
            Stream = client.GetStream();
            isConnected = true;

            if (KeyExchange() == true)
            {
                receiveThread = new Thread(new ThreadStart(ReceiveProcess));
                receiveThread.Start();
                OnConnected?.Invoke(this);
                return true;
            }

            Disconnect();
            return false;
        }

        public void Disconnect()
        {
            Send(new Message("", CommandMessage.DisconnectMessage));
            Abort();
        }

        private void Abort()
        {
            OnDisconnected?.Invoke(this); isConnected = false;
            if (Stream != null) Stream.Close();
            if (client != null) client.Close();
        }

        #endregion

        #region Receive/Send

        private bool KeyExchange()
        {
            try
            {
                Message request = new Message(CommandType.CONNECT, rsa.PublicKey);

                Send(JsonConvert.SerializeObject(request));

                Receive();

                while (QueueMessages.Count != 0)
                {
                    var responce = JsonConvert.DeserializeObject<Message>(rsa.Decrypt(QueueMessages.Dequeue()));
                    string command = responce.Dequeue();
                    if (command == CommandType.ACCEPTED)
                    {
                        aes = new AES(responce.value.Split('\\').First(), responce.value.Split('\\').Last());
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                if (e is IOException) { Abort(); }
                else { OnError?.Invoke(this, e); Disconnect(); }
            }
            return false;
        }

        private void ReceiveProcess()
        {
            try
            {
                while (isConnected == true)
                {
                    Receive();

                    while (QueueMessages.Count != 0)
                    {
                        var request = JsonConvert.DeserializeObject<Message>(aes.Decrypt(QueueMessages.Dequeue()));
                        if (request.value == CommandMessage.DisconnectMessage) { Abort(); break; }
                        OnMessage?.Invoke(this, request);
                    }
                }
            }
            catch (Exception e)
            {
                if (e is IOException) { Abort(); }
                else { OnError?.Invoke(this, e); Disconnect(); }
            }
        }

        private void Receive()
        {
            byte[] data = new byte[2048];
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
