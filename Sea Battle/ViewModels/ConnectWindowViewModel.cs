using Sea_Battle.Infrastructure;
using Models;
using PropertyChanged;
using System.Windows;
using System.Windows.Input;
using Sea_Battle.Views;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class ConnectWindowViewModel
    {
        private readonly Window window;
        public string Password { get; set; } = string.Empty;
        public ICommand ConnectCommand { get; }
        public ICommand CancelCommand { get; }
        public ConnectWindowViewModel(Window window)
        {
            this.window = window;

            ConnectCommand = new RelayCommand(Connect, CanConnect);
            CancelCommand = new RelayCommand(Cancel);
        }
        public async void Connect()
        {
            try
            {
                var request = new Request()
                {
                    Header = "JOIN",
                    User = CurrentUser.user,
                    Game = CurrentUser.game,
                    EnteredGamePassword = Password
                };

                Response response = await CurrentUser.SendMessageAsync(request);

                CurrentUser.game = response.Game;

                CurrentUser.SwitchWindow<LobbyWindow>(window);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                Password = string.Empty;
            }
        }
        public void Cancel()
            => CurrentUser.SwitchWindow<MainMenuWindow>(window);
        public bool CanConnect()
            => string.IsNullOrEmpty(Password) == false;
    }
}