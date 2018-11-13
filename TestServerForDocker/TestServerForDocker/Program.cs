using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TestServerForDocker
{
    class Server
    {
        Socket listener;
        Socket client;
        int port;
        bool isWorking;
        string fileUri = @"..\..\HTML Files\MainPage.html";

        public Server(int port)
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
            isWorking = false;
            this.port = port;
        }
        public void Start()
        {
            isWorking = true;
            listener.Listen(10);
            string stringRequest, response;
            byte[] buffer = new byte[1024];
            do
            {
                Console.WriteLine("Wait for connect...");
                client = listener.Accept();
                Console.WriteLine("Connected. Receiving request...");
                client.Receive(buffer);
                stringRequest = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(stringRequest);
                StringBuilder sb = new StringBuilder();
                using (StreamReader fs = File.OpenText(fileUri))
                {
                    string input = null;
                    while ((input = fs.ReadLine()) != null)
                    {
                        sb.AppendLine(input);
                    }
                    response = sb.ToString();
                }
                byte[] buff = Encoding.UTF8.GetBytes(response);
                client.Send(buff);
                client.Disconnect(false);
                client.Close();
            } while (isWorking);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(80);
            server.Start();
            Console.ReadLine();
        }
    }
}
