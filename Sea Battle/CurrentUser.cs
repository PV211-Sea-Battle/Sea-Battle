using Models;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

#pragma warning disable SYSLIB0011

namespace Sea_Battle
{
    static class CurrentUser
    {
        public static string? address = "127.0.0.1";
        public static int? port = 9001;

        public static User? user;
        public static Game? game;

        public static async Task<Response> SendMessageAsync(Request request, bool waitForResponse = true)
        {
            try
            {
                if (address is null || port is null)
                {
                    throw new Exception("You aren't connected to the server!");
                }

                using TcpClient acceptor = new(address, port.Value);
                await using NetworkStream ns = acceptor.GetStream();

                BinaryFormatter bf = new();

                bf.Serialize(ns, request);

                if (waitForResponse)
                {
                    Response response = (Response)bf.Deserialize(ns);

                    if (response.ErrorMessage is null)
                    {
                        return response;
                    }
                    else
                    {
                        throw new Exception(response.ErrorMessage);
                    }
                }

                return null!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void SwitchWindow<T>(Window currentWindow) where T : Window
        {
            if (Application.Current.Windows.OfType<T>().Any())
            {
                return;
            }

            if (Activator.CreateInstance(typeof(T)) is T window)
            {
                window.Show();
                currentWindow.Close();
            }
        }
    }
}
