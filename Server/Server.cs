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
        private static bool _isServerStarted = false;
        static async Task Main(string[] args)
        {
            Console.Title = "Sea Battle Server";
            int port = 9001;
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); ;
            IPEndPoint ep;
            TcpListener listener;
            BinaryFormatter bf = new BinaryFormatter();
            DbServer db = new DbServer();

            while (true)
            {
                if(_isServerStarted)
                {
                    _ = Task.Factory.StartNew(() =>
                    {
                        while (Console.ReadKey(true).Key != ConsoleKey.End) ;
                        _isServerStarted = false;
                        Console.WriteLine($"\n\n[{DateTime.Now.ToLongTimeString()}] Server stopped.\n");
                    });
                    try
                    {
                        //место для рабочего цикла
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message);
                        return;
                    }
                }
                else
                {
                    Task.Delay(100).Wait();
                    Console.Write("Enter 'START' to start server. " +
                        "Without configuration it will use default IP and port (127.0.0.1:9001)." +
                        "\nEnter 'EDIT' to set custom IP and port" +
                        "\nEnter 'VIEWDB' to view first 5 rows of every table in database" +
                        "\nEnter anything else to close the application\n>");
                    string? choice = await Console.In.ReadLineAsync();
                    switch(choice.ToLower())
                    {
                        case "start":
                            ep = new IPEndPoint(ipAddress, port);
                            listener = new TcpListener(ep);
                            listener.Start();
                            _isServerStarted = true;
                            Console.WriteLine($"\n\n[{DateTime.Now.ToLongTimeString()}] Server started. Press End to stop it.\n");
                            break;
                        case "edit":
                            try
                            {
                                Console.Write("\nEnter IP address> ");
                                ipAddress = IPAddress.Parse(Console.ReadLine() ?? "");
                                Console.Write("\nEnter port> ");
                                port = int.Parse(Console.ReadLine() ?? "");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"\n[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message);
                                return;
                            }
                            break;
                        case "viewdb":
                            try
                            {
                                throw new NotImplementedException("Работа с БД еще не реализована.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message);
                                return;
                            }
                            break;
                        default:
                            return;
                    }
                }
            }

            
        }
    }
}
