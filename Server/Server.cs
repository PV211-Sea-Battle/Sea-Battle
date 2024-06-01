using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Models;
using System.Runtime.Serialization;

#pragma warning disable SYSLIB0011

namespace Server
{
    public class Server
    {
        private static bool _isServerStarted;
        private static bool _isExtendedLogsEnabled;

        private static IPEndPoint? ep;
        private static TcpListener? listener;

        private static readonly BinaryFormatter bf = new();
        static async Task Main()
        {
            Console.Title = "Sea Battle Server";

            try
            {
                string[] serverConfig = File.ReadAllText("../../../serverConfig.ini").Split(':');

                _isExtendedLogsEnabled = bool.Parse(serverConfig[2]);
                int port = int.Parse(serverConfig[1]);
                IPAddress address = IPAddress.Parse(serverConfig[0]);

                ep = new(address, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read configuration from serverConfig.ini\n" +
                    $"Make sure that this file exist and it contains valid IP, port and extnd logs status in 'ip:port:status' format.\nDetails: " + ex.Message);
                return;
            }

            while (true)
            {
                if(_isServerStarted)
                {
                    try
                    {
                        using TcpClient acceptor = await listener!.AcceptTcpClientAsync();
                        await using NetworkStream ns = acceptor.GetStream();

                        Request request = (Request)bf.Deserialize(ns);
                        Response response = new();

                        string status = "FAILURE";

                        DbServer db = new();

                        switch (request.Header)
                        {
                            case "SIGN IN":
                                User? user = await DbServer.SignIn(request.User!.Login, request.User.Password);
                                
                                if (user is not null)
                                {
                                    response.User = user;
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    response.ErrorMessage = "An error occured during authentication:\nIncorrect login or password.";
                                }

                                await Log($"{request.Header} request. Login: {request.User.Login} | Status: {status} \n");
                                break;

                            case "REGISTER":
                                if (await DbServer.RegisterUser(request.User!.Login, request.User.Password))
                                {
                                    response.User = await DbServer.SignIn(request.User.Login, request.User.Password);
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    response.ErrorMessage = "An error occured during registration:\nLogin was taken.";
                                }

                                await Log($"{request.Header} request. Login: {request.User.Login} | Status: {status} \n");
                                break;

                            case "GAME LIST":
                                List<Game> res = await DbServer.GetGameList();

                                if (res is not null)
                                {
                                    response.Games = res;
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    response.ErrorMessage = "An error occured during getting games list from database.\nCheck server console for details.";
                                }

                                if (_isExtendedLogsEnabled)
                                {
                                    await Log($"[EXT - LOG] {request.Header} request. | Status: {status} \n");
                                }
                                break;

                            case "JOIN":
                                Game? game = await DbServer.GetGame(request.Game!.Id);

                                if (game is not null)
                                {
                                    if (game.ClientUser is null)
                                    {
                                        if (game.IsPrivate)
                                        {
                                            if (request.Game.Password == game.Password)
                                            {
                                                game = await DbServer.JoinGame(game.Id, request.User!.Id);

                                                if (game is not null)
                                                {
                                                    status = "SUCCESS";
                                                }
                                            }
                                            else 
                                            {
                                                status = "FAILURE-INCORR_PASS";
                                            }
                                        }
                                        else
                                        {
                                            game = await DbServer.JoinGame(game.Id, request.User!.Id);

                                            if (game is not null)
                                            {
                                                status = "SUCCESS";
                                            }
                                        }
                                    }
                                    else 
                                    {
                                        status = "FAILURE-GAME_FULL";
                                    }
                                }

                                if (status == "SUCCESS")
                                {
                                    response.Game = game;
                                }
                                else
                                {
                                    response.ErrorMessage = status switch
                                    {
                                        "FAILURE-INCORR_PASS" => "Failed to join the game:\nIncorrect password.",
                                        "FAILURE-GAME_FULL" => "Failed to join the game:\nGame is full.",
                                        _ => "Failed to join the game:\nGame or user does not exist.",
                                    };
                                }

                                await Log($"{request.Header} request. Login: {request.User?.Login}. Room Name: {request.Game.Name} | Status: {status} \n");
                                break;

                            case "CREATE":
                                game = await DbServer.CreateGame(request.Game!, request.User!.Id);

                                if (game is not null)
                                {
                                    response.Game = game;
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    response.ErrorMessage = "Failed to create the game:\nName was already taken or user does not exist";
                                }

                                await Log($"{request.Header} request. Login: {request.User.Login}. Room Name: {request.Game?.Name} | Status: {status} \n");
                                break;

                            case "READY":
                                status = await DbServer.Ready(request.Field!, request.User!.Id, request.Game!.Id);

                                if (status != "SUCCESS")
                                {
                                    response.ErrorMessage = status;
                                    status = "FAILURE";
                                }

                                if (_isExtendedLogsEnabled)
                                {
                                    await Log($"[EXT - LOG] {request.Header} request. | UserId: {request.User.Id} | Status: {status} \n");
                                }
                                break;

                            case "ENEMY WAIT":
                                game = await DbServer.EnemyWait(request.Game!.Id, request.User!.Id);

                                if (game is not null)
                                {
                                    response.Game = game;
                                    status = "SUCCESS";
                                }
                                else
                                {
                                    response.ErrorMessage = "Error:\nInvalid user or game";
                                }

                                if (_isExtendedLogsEnabled)
                                {
                                    await Log($"[EXT - LOG] {request.Header} request. | GameId: {request.Game.Id} | UserId: {request.User.Id} | Status: {status} \n");
                                }
                                break;

                            case "SHOOT":
                                status = await DbServer.Shoot(request.Field.Id, request.Game.Id, request.User.Id, request.Index.Value);

                                if (status != "SUCCESS")
                                {
                                    response.ErrorMessage = status;
                                    status = "FAILURE";
                                }

                                await Log($"{request.Header} request. | GameId: {request.Game.Id} | UserId: {request.User.Id} | Status: {status} \n");
                                break;

                            case "OFFLINE":
                                await DbServer.Offline(request.User.Id);
                                break;

                            default:
                                response.ErrorMessage = "Incorrect request header";
                                break;
                        }

                        bf.Serialize(ns, response);
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
                    Console.Clear();

                    Console.Write("Available commands:" +
                    "\n'START' - Start the server." +
                    $"\n'EDIT' - Edit address file. Current address is {ep.Address}:{ep.Port}" +
                    "\n'VIEWDB' - View first 5 rows of every table in database." +
                    "\n'LOGS' - Switch the extended server logs. Current status: ");
                    if (_isExtendedLogsEnabled) Console.Write("ON"); else Console.Write("OFF");

                    Console.Write("\n'EXIT' - Close the application.\n>");
                    string choice = Console.ReadLine() ?? "";
                    
                    switch (choice.ToLower().Trim())
                    {
                        case "start":
                            try
                            {
                                listener = new TcpListener(ep);
                                listener.Start();
                                _isServerStarted = true;

                                _ = Task.Run(StopThread);

                                await Log($"Server started with address {ep.Address}:{ep.Port}. Press End to stop it.\n");
                            }
                            catch (Exception ex)
                            {
                                await LogError("Server starting error: " + ex.Message);
                            }
                            break;

                        case "edit":
                            try
                            {
                                await Log("Enter IP address> ");
                                IPAddress address = IPAddress.Parse(Console.ReadLine() ?? "");

                                await Log("Enter port> ");
                                int port = int.Parse(Console.In.ReadLine() ?? "");

                                ep = new(address, port);

                                File.WriteAllText("../../../serverConfig.ini", $"{address}:{port}:{_isExtendedLogsEnabled}");
                            }
                            catch (Exception ex)
                            {
                                await Log("IP/Port editing error: " + ex.Message);
                            }
                            break;

                        case "viewdb":
                            await DbServer.ShowFirst5RowsOfEveryTable();

                            await LogError(string.Empty);
                            break;

                        case "logs":
                            _isExtendedLogsEnabled ^= true;

                            File.WriteAllText("../../../serverConfig.ini", $"{ep.Address}:{ep.Port}:{_isExtendedLogsEnabled}");
                            break;

                        case "exit":
                            return;
                    }
                }
            }
        }

        private static void StopThread()
        {
            while (Console.ReadKey(true).Key != ConsoleKey.End) { }

            _isServerStarted = false;
            listener?.Stop();
        }

        private static async Task Log(string message)
            => await Console.Out.WriteLineAsync($"\n[{DateTime.Now.ToLongTimeString()}] {message}");

        private static async Task LogError(string message)
        {
            await Log(message);

            Console.Write("\nPress any button to continue");
            Console.ReadKey();
        }
    }
}
