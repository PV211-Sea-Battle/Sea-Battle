using Models;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable SYSLIB0011

namespace Sea_Battle
{
    static class CurrentUser
    {
        public static User User { get; set; }
        public static string address;
        public static int port;
        public static async Task<Response> SendMessageAsync(Request request, bool waitForResponse = true)
        {
            try
            {
                using TcpClient acceptor = new(address, port);
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
    }
}
