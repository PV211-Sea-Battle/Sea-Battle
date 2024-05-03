using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Models;

#pragma warning disable SYSLIB0011
#pragma warning disable CS8600
#pragma warning disable CS8602 //мозолят глаза там, где null'ов быть не может

namespace Server
{
    //------ПЕРЕД ТЕСТАМИ - НАСТРОИТЬ appsettings.json ПОД СЕБЯ------\\
    public class Server
    {
        private static bool _isServerStarted = false;
        static async Task Main(string[] args)
        {
            Console.Title = "Sea Battle Server";

            string[] connectionAddress = null!;
            int port = 0;
            IPAddress ipAddress = null!;

            try
            {
                connectionAddress = File.ReadAllText("../../../connectionAddress.ini").Split(':');
                port = int.Parse(connectionAddress[1]);
                ipAddress = IPAddress.Parse(connectionAddress[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read connection address from connectionAddress.ini\n" +
                    $"Make sure that this file exist and it contains valid IP and port in 'ip:port' format.\nDetails: " + ex.Message);
                return;
            }

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
                                string status = "FAILURE";
                                string login = user.Login;
                                string pass = user.Password;

                                user = db.SignUp(login, pass);
                                if (user is not null)
                                {
                                    var signResponse = new Response()
                                    {
                                        User = user
                                    };
                                    bf.Serialize(ns, signResponse);
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    var signResponse = new Response()
                                    {
                                        ErrorMessage = "An error occured during authentication:\nIncorrect login or password."
                                    };
                                    bf.Serialize(ns, signResponse);
                                }

                                await Console.Out.WriteLineAsync($"\n\n[{DateTime.Now.ToLongTimeString()}] " +
                                    $"{request.Header} request. Login: {user.Login} | Status: {status} \n");
                                break;
                            case "REGISTER":
                                user = request.User;
                                status = "FAILURE";
                                login = user.Login;
                                pass = user.Password;

                                if (db.RegisterUser(login, pass))
                                {
                                    var regResponse = new Response()
                                    {
                                        User = db.SignUp(login, pass)
                                    };
                                    bf.Serialize(ns, regResponse);
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    var regResponse = new Response()
                                    {
                                        ErrorMessage = "An error occured during registration:\nLogin was taken."
                                    };
                                    bf.Serialize(ns, regResponse);
                                }

                                await Console.Out.WriteLineAsync($"\n\n[{DateTime.Now.ToLongTimeString()}] " +
                                    $"{request.Header} request. Login: {user.Login} | Status: {status} \n");
                                break;
                            case "GAME LIST":
                                var res = db.GetGameList();
                                status = "FAILURE";
                                if(res is not null)
                                {
                                    var glResponse = new Response()
                                    {
                                        Games = res
                                    };
                                    bf.Serialize(ns, glResponse);
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    var glResponse = new Response()
                                    {
                                        ErrorMessage = "An error occured during getting games list from database.\nCheck server console for details."
                                    };
                                    bf.Serialize(ns, glResponse);
                                }

                                //await Console.Out.WriteLineAsync($"\n\n[{DateTime.Now.ToLongTimeString()}] " +
                                //    $"{request.Header} request. | Status: {status} \n");
                                //при нормальной работе оно, как и refersh, enemywait и shoot будет сильно засирать консоль
                                //позже можно добавить в меню сервера опцию по типу "расширенные логи"
                                break;
                            case "JOIN":
                                Game game = await DbServer.GetGame(request.Game.Id);
                                user = request.User;
                                string? gamePassword = request.EnteredGamePassword;
                                status = "FAILURE";

                                if (game.ClientUserId == -1)
                                {
                                    if (game.IsPrivate)
                                    {
                                        if (gamePassword == game.Password)
                                        {
                                            game = db.JoinGame(game.Id, user.Id);
                                            if (game is not null)
                                                status = "SUCCESS";
                                        }
                                        else status = "FAILURE-INCORR_PASS";
                                    }
                                    else
                                    {
                                        game = db.JoinGame(game.Id, user.Id);
                                        if (game is not null)
                                            status = "SUCCESS";
                                    }
                                }
                                else status = "FAILURE-GAME_FULL";

                                if(status == "SUCCESS")
                                {
                                    var joinResponse = new Response()
                                    {
                                        Game = game
                                    };
                                    bf.Serialize(ns, joinResponse);
                                }
                                else
                                {
                                    var joinResponse = new Response();
                                    switch (status)
                                    {
                                        case "FAILURE-INCORR_PASS":
                                            joinResponse.ErrorMessage = "Failed to join the game:\nIncorrect password.";
                                            break;
                                        case "FAILURE-GAME_FULL":
                                            joinResponse.ErrorMessage = "Failed to join the game:\nGame is full.";
                                            break;
                                        default:
                                            joinResponse.ErrorMessage = "Failed to join the game:\nGame or user does not exist.";
                                            break;
                                    }
                                    bf.Serialize(ns, joinResponse);
                                }

                                await Console.Out.WriteLineAsync($"\n\n[{DateTime.Now.ToLongTimeString()}] " +
                                    $"{request.Header} request. Login: {user.Login}. Room Name: {game.Name} | Status: {status} \n");
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
                            case "FORFREIT":
                                throw new NotImplementedException();
                                break;
                            case "REMATCH":
                                throw new NotImplementedException();
                                break;
                            default:
                                var defaultResponse = new Response()
                                {
                                    ErrorMessage = "Incorrect request header"
                                };
                                bf.Serialize(ns, defaultResponse);
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
                        Console.Write("Enter 'START' to start server." +
                        $"\nEnter 'EDIT' to edit address file. Current address is {ipAddress}:{port}" +
                        "\nEnter 'VIEWDB' to view first 5 rows of every table in database." +
                        "\nEnter 'EXIT' to close the application.\n>");
                        choice = await Console.In.ReadLineAsync();
                    }
                    while (choice.ToLower().Trim() != "start" && choice.ToLower().Trim() != "edit" && 
                    choice.ToLower().Trim() != "viewdb" && choice.ToLower().Trim() != "exit");
                    
                    switch(choice.ToLower().Trim())
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
                                await Console.Out.WriteAsync("\nEnter IP address> ");
                                ipAddress = IPAddress.Parse(await Console.In.ReadLineAsync() ?? "");
                                await Console.Out.WriteAsync("\nEnter port> ");
                                port = int.Parse(await Console.In.ReadLineAsync() ?? "");

                                File.WriteAllText("../../../connectionAddress.ini", $"{ipAddress}:{port}");
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
