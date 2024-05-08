using Sea_Battle.Infrastructure;
using Models;
using PropertyChanged;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class ConnectWindowViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ICommand ConnectCommand { get; }
        public ICommand CancelCommand { get; }
        public ConnectWindowViewModel()
        {
            ConnectCommand = new RelayCommand<string>(Connect, CanConnect);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }
        public async void Connect(string header)
        {
            try
            {
                var request = new Request()
                {
                    Header = header,
                    User = CurrentUser.user,
                    Game = CurrentUser.game,
                    EnteredGamePassword = Password
                };

                await CurrentUser.SendMessageAsync(request);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                Name = string.Empty;
                Password = string.Empty;
            }
        }
        public void Cancel()
        {
            //CurrentUser.SwitchWindow<MainMenuWindow>(this);
        }
        public bool CanConnect()
            => string.IsNullOrEmpty(Name) == false
            && string.IsNullOrEmpty(Password) == false;
        public bool CanCancel()
            => true;
    }
}