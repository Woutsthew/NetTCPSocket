using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetTCPSocket.TCPServer
{
    public class TCPServer : IDisposable
    {
        #region Variables

        public IPAddress IP_Address { get; private set; }
        public int Port { get; private set; }

        private TcpListener tcpListener { get; set; }

        private Thread listenThread { get; set; }

        protected List<TCPSession> Sessions { get; set; } = new List<TCPSession>();

        #endregion

        public TCPServer(string address, int port) : this(IPAddress.Parse(address), port) { }

        public TCPServer(IPAddress address, int port)
        {
            IP_Address = address;
            Port = port;
        }

        #region Start/Stop Listening

        public bool IsListening { get; private set; }

        public bool Start()
        {
            if (IsListening)
                return false;

            tcpListener = new TcpListener(IP_Address, Port);
            tcpListener.Start();

            listenThread = new Thread(new ThreadStart(Listen));
            listenThread.Start();

            IsListening = true;

            return true;
        }

        public bool Stop()
        {
            if (!IsListening)
                return false;

            DisconnectAll();

            tcpListener.Stop();

            listenThread.Abort();

            IsListening = false;

            return true;
        }

        protected internal void Listen()
        {
            while (IsListening)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                TCPSession tcpsession = CreateSession();
                tcpsession.Client = tcpClient;

                Thread clientThread = new Thread(new ThreadStart(tcpsession.ProcessReceive));
                clientThread.Start();
            }
        }

        public void DisconnectAll()
        {
            for ( ;Sessions.Count > 0; )
                Sessions[0].Disconnect();
        }

        #endregion

        #region Session Factory

        protected virtual TCPSession CreateSession() { return new TCPSession(this); }

        protected internal void AddSession(TCPSession SessionObject) { Sessions.Add(SessionObject); }

        protected internal void RemoveSession(Guid id)
        {
            TCPSession session = Sessions.Find(c => c.Id == id);
            if (session != null)
                Sessions.Remove(session);
        }

        #endregion

        public void Dispose() { IsListening = false; Stop(); }
    }
}
