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
        public string Password { get; set; } = string.Empty;
        public ICommand ConnectCommand { get; }
        public ICommand CancelCommand { get; }
        public ConnectWindowViewModel()
        {
            ConnectCommand = new RelayCommand<Window>(Connect, CanConnect);
            CancelCommand = new RelayCommand<Window>(Cancel);
        }
        public async void Connect(Window window)
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

                await CurrentUser.SendMessageAsync(request);

                // Построение защиты

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                Password = string.Empty;
            }
        }
        public void Cancel(Window window)
            => CurrentUser.SwitchWindow<MainMenuWindow>(window);
        public bool CanConnect()
            => string.IsNullOrEmpty(Password) == false;
    }
}