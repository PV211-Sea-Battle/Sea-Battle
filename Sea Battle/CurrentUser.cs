using Models;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable SYSLIB0011

namespace Sea_Battle
{
    static class CurrentUser
    {
        public static Form? form = new AuthPage();

        public static string? address;
        public static int? port;

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

        public static void ButtonEnabledChanged(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.ForeColor = button.Enabled ? Color.White : Color.Gray;
                button.BackColor = button.Enabled ? Color.Black : Color.DimGray;
            }
        }

        // Пример использования метода SendMessageAsync
        private static async void Test()
        {
            // Создаем обработчик ошибок
            // Так как метод SendMessageAsync вызывает throw если с сервера была отправлена ошибка
            // В throw передается свойство ErrorMessage класса Response
            try
            {
                // Создаем запрос на сервер
                Request request = new()
                {
                    // Ключевое слово для сервера
                    Header = "TEST",

                    // Пользователь который обратился к серверу.
                    // После входа в аккаунт он сохранен в переменной user класса CurrentUser
                    User = CurrentUser.user,

                    // Добавляйте нужные параметры в класс Request в зависимости от запроса
                    // Ожидаемые параметры от клиента я прописал в Trello
                };

                // Отправка запроса на сервер через метод SendMessageAsync
                // Адресс и порт сервера сохранены в переменных address и port класса CurrentUser
                // SendMessageAsync возвращает ответ от сервера Response
                // Если вам надо использовать ответ сервера, то сохраняйте ответ в переменную
                Response response = await CurrentUser.SendMessageAsync(request);

                // Делать проверку на ошибку с сервера в свойстве ErrorMessage не нужно
                // Так как при наличии ошибки будет вызван throw, который перехватит наш catch

                // Если код дошел дальше вызова метода SendMessageAsync, значит ошибки у сервера не было
                // Это означает что все прошло ОК, и можно делать нужные действия

            }
            catch (Exception ex)
            {
                // Вывод ошибки 
                MessageBox.Show(ex.Message);
            }
        }
    }
}
