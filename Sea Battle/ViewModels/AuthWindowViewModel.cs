using Sea_Battle.Infrastructure;
using Models;
using PropertyChanged;
using System.Windows;
using System.Windows.Input;
using Sea_Battle.Views;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class AuthWindowViewModel
    {
        private readonly Window window;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Address { get; set; } = CurrentUser.address ?? string.Empty;
        public string Port { get; set; } = CurrentUser.port.ToString() ?? string.Empty;
        public ICommand EntryCommand { get; }
        public ICommand ConnectCommand { get; }
        public AuthWindowViewModel(Window window)
        {
            this.window = window;

            EntryCommand = new RelayCommand<string>(Entry, CanEntry);
            ConnectCommand = new RelayCommand(Connect, CanConnect);
        }
        public async void Entry(string header)
        {
            try
            {
                var request = new Request()
                {
                    Header = header,
                    User = new User()
                    {
                        Login = Login,
                        Password = Password
                    }
                };

                Response response = await CurrentUser.SendMessageAsync(request);

                CurrentUser.user = response.User;

                CurrentUser.SwitchWindow<MainMenuWindow>(window);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                Login = string.Empty;
                Password = string.Empty;
            }
        }
        public void Connect()
        {
            try
            {
                CurrentUser.address = Address;
                CurrentUser.port = int.Parse(Port);
            }
            catch
            {
                MessageBox.Show("IP Address Field or Port Field are empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public bool CanConnect()
            => CurrentUser.address != Address
            || CurrentUser.port.ToString() != Port;
        public bool CanEntry()
            => string.IsNullOrEmpty(Login) == false
            && string.IsNullOrEmpty(Password) == false;
    }
}
