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
            if (client.Connect() == false) return;

            try
            {
                while (client.isConnected == true)
                {
                    string msg = Console.ReadLine();
                    if (msg == "dis") client.Disconnect();
                    Message request = new Message(msg);
                    client.Send(request);
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            Console.ReadKey();
        }

        private static void Client_OnConnected(TCPClient client) { Console.WriteLine("Connected..."); }

        private static void Client_OnDisconnected(TCPClient client) { Console.WriteLine("Disconnected..."); }

        private static void Client_OnMessage(TCPClient session, Message message) { Console.WriteLine(message.GetAllCommand()); }

        private static void Client_OnError(TCPClient session, Exception e) { Console.WriteLine(e.Message + e.Source + e.TargetSite + e.StackTrace + e.InnerException); }
    }
}
