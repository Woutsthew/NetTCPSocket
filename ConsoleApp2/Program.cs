using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTCPSocket.TCPClient;
using NetTCPSocket;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "192.168.1.64";
            int port = 8888;

            TCPClient client = new TCPClient(ip, port);
            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;
            client.OnMessage += Client_OnMessage;
            client.OnError += Client_OnError;
            client.Connect();

            while(true)
            {
                string msg = Console.ReadLine();
                Message request = new Message(msg);
                client.Send(request);
            }
        }

        #region event client

        private static void Client_OnConnected(TCPClient client)
        {
            Console.WriteLine("Connected...");
        }

        private static void Client_OnDisconnected(TCPClient client)
        {
            Console.WriteLine("Disconnected...");
        }

        private static void Client_OnMessage(Message message, TCPClient session)
        {
            Console.WriteLine(message.GetAllCommand());
        }

        private static void Client_OnError(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        #endregion
    }
}
