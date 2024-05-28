using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Models;
using System.Runtime.Serialization;

#pragma warning disable SYSLIB0011
#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604

namespace Server
{
    public class Server
    {
        private static bool _isServerStarted = false;
        private static bool _isExtendedLogsEnabled = false;
        static async Task Main(string[] args)
        {
            Console.Title = "Sea Battle Server";

            string[] serverConfig = null!;
            int port = 0;
            IPAddress ipAddress = null!;

            try
            {
                serverConfig = File.ReadAllText("../../../serverConfig.ini").Split(':');
                _isExtendedLogsEnabled = Convert.ToBoolean(serverConfig[2]);
                port = int.Parse(serverConfig[1]);
                ipAddress = IPAddress.Parse(serverConfig[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read configuration from serverConfig.ini\n" +
                    $"Make sure that this file exist and it contains valid IP, port and extnd logs status in 'ip:port:status' format.\nDetails: " + ex.Message);
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
                    _ = Task.Run(() =>
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
                                    $"{request.Header} request. Login: {request.User.Login} | Status: {status} \n");
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
                                    $"{request.Header} request. Login: {request.User.Login} | Status: {status} \n");
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

                                if (_isExtendedLogsEnabled)
                                    await Console.Out.WriteLineAsync($"\n\n[{DateTime.Now.ToLongTimeString()}][EXT-LOG] " +
                                        $"{request.Header} request. | Status: {status} \n");
                                break;
                            case "JOIN":
                                Game game = db.GetGame(request.Game.Id);
                                user = request.User;
                                string? gamePassword = request.EnteredGamePassword;
                                status = "FAILURE";

                                if(game is not null)
                                {
                                    if (game.ClientUserId == null)
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
                                }
                                

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
                                    $"{request.Header} request. Login: {request.User.Login}. Room Name: {request.Game.Name} | Status: {status} \n");
                                break;
                            case "CREATE":
                                game = db.CreateGame(request.Game, request.User.Id);
                                status = "FAILURE";

                                if(game is not null)
                                {
                                    var crResponce = new Response()
                                    {
                                        Game = game
                                    };
                                    bf.Serialize(ns, crResponce);
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    var crResponce = new Response()
                                    {
                                        ErrorMessage = "Failed to create the game:\nName was already taken or user does not exist"
                                    };
                                    bf.Serialize(ns, crResponce);
                                }

                                await Console.Out.WriteLineAsync($"\n\n[{DateTime.Now.ToLongTimeString()}] " +
                                    $"{request.Header} request. Login: {request.User.Login}. Room Name: {request.Game.Name} | Status: {status} \n");
                                break;
                            case "READY":
                                status = db.Ready(request.Field, request.User.Id, request.Game.Id);
                                var rdResponce = new Response();
                                if (status != "SUCCESS")
                                {
                                    rdResponce.ErrorMessage = status;
                                    status = "FAILURE";
                                }
                                bf.Serialize(ns, rdResponce);

                                if (_isExtendedLogsEnabled)
                                    await Console.Out.WriteLineAsync($"\n\n[{DateTime.Now.ToLongTimeString()}][EXT-LOG] " +
                                        $"{request.Header} request. | UserId: {request.User.Id} | Status: {status} \n");
                                break;
                            case "ENEMY WAIT":
                                Game Game = db.EnemyWait(request.Game.Id, request.User.Id);
                                status = "FAILURE";

                                if (Game is not null)
                                {
                                    var ewResponse = new Response()
                                    {
                                        Game = Game
                                    };
                                    bf.Serialize(ns, ewResponse);
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    var ewResponse = new Response()
                                    {
                                        ErrorMessage = "Error:\nInvalid user or game"
                                    };
                                    bf.Serialize(ns, ewResponse);
                                }

                                if (_isExtendedLogsEnabled)
                                    await Console.Out.WriteLineAsync($"\n\n[{DateTime.Now.ToLongTimeString()}][EXT-LOG] " +
                                        $"{request.Header} request. | GameId: {request.Game.Id} | " +
                                        $"UserId: {request.User.Id} | Status: {status} \n");
                                break;
                            case "SHOOT":
                                string? error = await DbServer.Shoot(request.Cell.Id, request.Game.Id, request.User.Id);
                                status = "SUCCESS";

                                if (error is not null)
                                {
                                    status = "FAILURE";
                                }

                                Response sresponse = new()
                                {
                                    ErrorMessage = error
                                };
                                bf.Serialize(ns, sresponse);

                                await Console.Out.WriteLineAsync($"\n\n[{DateTime.Now.ToLongTimeString()}] " +
                                        $"{request.Header} request. | GameId: {request.Game.Id} | " +
                                        $"UserId: {request.User.Id} | Status: {status} \n");
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
                    catch (IOException) { }
                    catch (SocketException) { }
                    catch (SerializationException) { }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message);
                        return;
                    }
                }
                else
                {
                    string? choice;
                    do
                    {
                        Console.Write("Avaivable commands:" +
                        "\n'START' - Start the server." +
                        $"\n'EDIT' - Edit address file. Current address is {ipAddress}:{port}" +
                        "\n'VIEWDB' - View first 5 rows of every table in database." +
                        "\n'LOGS' - Switch the extended server logs. Current status: ");
                        if (_isExtendedLogsEnabled) Console.Write("ON"); else Console.Write("OFF");

                        Console.Write("\n'EXIT' - Close the application.\n>");
                        choice = await Console.In.ReadLineAsync();
                    }
                    while (choice.ToLower().Trim() != "start" && choice.ToLower().Trim() != "edit" && 
                    choice.ToLower().Trim() != "viewdb" && choice.ToLower().Trim() != "logs" &&
                    choice.ToLower().Trim() != "exit");
                    
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

                                File.WriteAllText("../../../serverConfig.ini", $"{ipAddress}:{port}:{_isExtendedLogsEnabled}");
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
                        case "logs":
                            if (_isExtendedLogsEnabled)
                                _isExtendedLogsEnabled = false;
                            else
                                _isExtendedLogsEnabled = true;

                            File.WriteAllText("../../../serverConfig.ini", $"{ipAddress}:{port}:{_isExtendedLogsEnabled}");
                            break;
                        case "exit":
                            return;
                    }
                }
            }

            
        }
    }
}
