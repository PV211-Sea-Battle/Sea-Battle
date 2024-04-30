#pragma warning disable SYSLIB0011
using Microsoft.Data.SqlClient;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.Configuration;

namespace Server
{
    public class Server
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Sea Battle Server";

            int port = 9001;
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); ;
            IPEndPoint ep;
            TcpListener listener;
            BinaryFormatter bf = new BinaryFormatter();
            DbServer db = new DbServer();

            Console.WriteLine("Press Y if you wish to use custom IP and port.");
            Console.WriteLine("Press any other key to use default IP and port (127.0.0.1:9001).");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                try
                {
                    Console.Write("\nEnter IP address> ");
                    ipAddress = IPAddress.Parse(Console.ReadLine()??"");
                    Console.Write("\nEnter port> ");
                    port = int.Parse(Console.ReadLine()??"");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message);
                    return;
                }
            }

            ep = new IPEndPoint(ipAddress, port);
            listener = new TcpListener(ep);
            listener.Start();
            Console.WriteLine($"\n\n[{DateTime.Now.ToLongTimeString()}] Server started.\n");

            try
            {
                while(true)
                {
                    throw new NotImplementedException("Рабочий цикл еще не реализован.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message);
                return;
            }
        }
    }
}
