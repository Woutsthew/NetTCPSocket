using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTCPSocket.TCPServer;
using NetTCPSocket;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "192.168.1.64";
            int port = 8888;

            ChatServer srv = new ChatServer(ip, port);
            srv.Start();

            Console.ReadKey();
        }

        public class ChatServer : TCPServer
        {
            public ChatServer(string address, int port) : base(address, port) { }

            protected override TCPSession CreateSession() { return new ChatSession(this); }

            protected override void OnConnected(TCPSession session) { Console.WriteLine("New connected: " + session.Id); }

            protected override void OnDisconnected(TCPSession session) { Console.WriteLine("Session disconnected: " + session.Id); }
        }

        public class ChatSession : TCPSession
        {
            public ChatSession(TCPServer server) : base(server) { }

            protected override void OnMessage(TCPSession session, Message message)
            {
                Console.WriteLine("Message from " + session.Id + ": " + message.GetAllCommand());

                string command = message.Dequeue();

                Message responce = new Message();

                if (command == "1")
                    responce = new Message("hello");
                else if (command == "2")
                    responce = new Message("привет");
                else if (command == "!")
                    session.Disconnect();
                else
                    responce = new Message("what?");

                session.Send(responce);
            }

            protected override void OnError(TCPSession session, Exception e)
            {
                Console.WriteLine(e.Message + e.Source + e.TargetSite + e.StackTrace + e.InnerException);
            }
        }
    }
}
