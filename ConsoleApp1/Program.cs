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

            srv.DisconnectAll();

            Console.ReadKey();
        }

        public class ChatServer : TCPServer
        {
            public ChatServer(string address, int port) : base(address, port) { }

            protected override TCPSession CreateSession() { return new ChatSession(this); }
        }

        public class ChatSession : TCPSession
        {
            public ChatSession(TCPServer server) : base(server) { }

            protected override void OnConnected(TCPSession session) { Console.WriteLine("New connected: " + session.Id); }

            protected override void OnDisconnected(TCPSession session) { Console.WriteLine("Session disconnected: " + session.Id); }

            protected override void OnMessage(TCPSession session, Message message)
            {
                Console.WriteLine("Message from " + session.Id + ": " + message.GetAllCommand());

                string command = message.Dequeue();

                Message responce = new Message();

                if (command == "1")
                    responce = new Message("hello");
                else if (command == "2")
                    responce = new Message("привет");
                else if (command == "test")
                    responce = new Message($"{CommandType.SUCCESS} {CommandType.TABLE}", "[{\"Name\":\"admin\",\"Password\":\"37bd45d638c2d11c49c641d2e9c4f49f406caf3ee282743e0c800aa1ed68e2ee\",\"User_Access\":\"gcd\",\"Switch_Access\":\"gcd\",\"ARP_Access\":\"gcd\",\"FDB_Access\":\"gcd\",\"IMPB_Access\":\"gcd\"},{\"Name\":\"test1\",\"Password\":\"60303ae22b998861bce3b28f33eec1be758a213c86c93c076dbe9f558c11c752\",\"User_Access\":\"gc\",\"Switch_Access\":\"gc\",\"ARP_Access\":\"g\",\"FDB_Access\":\"g\",\"IMPB_Access\":\"g\"},{\"Name\":\"test2\",\"Password\":\"60303ae22b998861bce3b28f33eec1be758a213c86c93c076dbe9f558c11c752\",\"User_Access\":\"gc\",\"Switch_Access\":\"gc\",\"ARP_Access\":\"gc\",\"FDB_Access\":\"gc\",\"IMPB_Access\":\"gc\"},{\"Name\":\"test3\",\"Password\":\"fd61a03af4f77d870fc21e05e7e80678095c92d808cfb3b5c279ee04c74aca13\",\"User_Access\":\"gcd\",\"Switch_Access\":\"gcd\",\"ARP_Access\":\"gcd\",\"FDB_Access\":\"gcd\",\"IMPB_Access\":\"gcd\"}]");
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
