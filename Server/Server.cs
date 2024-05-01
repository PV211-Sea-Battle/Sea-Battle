using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Models;

#pragma warning disable SYSLIB0011
#pragma warning disable CS8600
#pragma warning disable CS8602 //мозолят глаза там, где null'ов быть не может

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
            TcpListener listener = null!;
            BinaryFormatter bf = new();
            DbServer db = new();

            while (true)
            {
                if(_isServerStarted)
                {
                    _ = Task.Factory.StartNew(() =>
                    {
                        while (Console.ReadKey(true).Key != ConsoleKey.End) ;
                        _isServerStarted = false;
                        listener.Stop();
                        Console.WriteLine($"\n\n[{DateTime.Now.ToLongTimeString()}] Server stopped.\n");
                    });
                    try
                    {
                        TcpClient acceptor = await listener.AcceptTcpClientAsync();
                        NetworkStream ns = acceptor.GetStream();
                        Request request = (Request)bf.Deserialize(ns);

                        switch (request.Header)
                        {
                            case "SIGN IN":
                                User user = request.User;

                                string login = user.Login;
                                string pass = user.Password;

                                user = db.SignUp(login, pass);
                                if (user is not null)
                                {
                                    var regResponse = new Response()
                                    {
                                        User = user
                                    };
                                    bf.Serialize(ns, regResponse);
                                }
                                else
                                {
                                    var signResponse = new Response()
                                    {
                                        ErrorMessage = "An error occured during authentication:\nIncorrect login or password."
                                    };
                                    bf.Serialize(ns, signResponse);
                                }
                                break;
                            case "REGISTER":
                                user = request.User;

                                login = user.Login;
                                pass = user.Password;

                                if (db.RegisterUser(login, pass))
                                {
                                    var regResponse = new Response()
                                    {
                                        User = db.SignUp(login, pass)
                                    };
                                    bf.Serialize(ns, regResponse);
                                }
                                else
                                {
                                    var regResponse = new Response()
                                    {
                                        ErrorMessage = "An error occured during registration:\nLogin was taken."
                                    };
                                    bf.Serialize(ns, regResponse);
                                }
                                break;
                            case "GAME LIST":
                                throw new NotImplementedException();
                                break;
                            case "JOIN":
                                throw new NotImplementedException();
                                break;
                            case "CREATE":
                                throw new NotImplementedException();
                                break;
                            case "READY":
                                throw new NotImplementedException();
                                break;
                            case "ENEMY WAIT":
                                throw new NotImplementedException();
                                break;
                            case "REFRESH":
                                throw new NotImplementedException();
                                break;
                            case "SHOOT":
                                throw new NotImplementedException();
                                break;
                            case "FORFEIT":
                                throw new NotImplementedException();
                                break;
                            case "REMATCH":
                                throw new NotImplementedException();
                                break;
                        }

                        acceptor.Close();
                        ns.Close();
                    }
                    catch (SocketException) { }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message);
                        return;
                    }
                }
                else
                {
                    Task.Delay(100).Wait();
                    string? choice;
                    do
                    {
                        Console.Write("Enter 'START' to start server. " +
                        "Without configuration it will use default IP and port (127.0.0.1:9001)." +
                        "\nEnter 'EDIT' to set custom IP and port." +
                        "\nEnter 'VIEWDB' to view first 5 rows of every table in database." +
                        "\nEnter 'EXIT' to close the application.\n>");
                        choice = await Console.In.ReadLineAsync();
                    }
                    while (choice.ToLower() != "start" && choice.ToLower() != "edit" && 
                    choice.ToLower() != "viewdb" && choice.ToLower() != "exit");
                    
                    switch(choice.ToLower())
                    {
                        case "start":
                            try
                            {
                                ep = new IPEndPoint(ipAddress, port);
                                listener = new TcpListener(ep);
                                listener.Start();
                                _isServerStarted = true;
                                Console.WriteLine($"\n\n[{DateTime.Now.ToLongTimeString()}] " +
                                    $"Server started with address {ipAddress}:{port}. Press End to stop it.\n");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Server starting error: " + ex.Message);
                                return;
                            }
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
                                Console.WriteLine($"\n[{DateTime.Now.ToLongTimeString()}] IP/Port editing error: " + ex.Message);
                                return;
                            }
                            break;
                        case "viewdb":
                            db.ShowFirst5RowsOfEveryTable();
                            break;
                        case "exit":
                            return;
                    }
                }
            }

            
        }
    }
}
