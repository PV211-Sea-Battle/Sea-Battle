using Models;
using PropertyChanged;
using Sea_Battle.Infrastructure;
using Sea_Battle.Views;
using System.Windows;
using System.Windows.Input;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class CreateWindowViewModel
    {
        private readonly Window window;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }
        public CreateWindowViewModel(Window window)
        {
            this.window = window;

            CreateCommand = new RelayCommand(Create, CanCreate);
            CancelCommand = new RelayCommand(Cancel);
        }
        public async void Create()
        {
            try
            {
                var request = new Request()
                {
                    Header = "CREATE",
                    User = CurrentUser.user,
                    Game = new Game
                    {
                        Name = Name,
                        IsPrivate = IsChecked,
                        Password = Password
                    }
                };

                Response response = await CurrentUser.SendMessageAsync(request);

                CurrentUser.game = response.Game;

                CurrentUser.SwitchWindow<LobbyWindow>(window);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                Name = string.Empty;
                Password = string.Empty;
            }
        }
        public void Cancel()
            => CurrentUser.SwitchWindow<MainMenuWindow>(window);
        public bool CanCreate()
            => string.IsNullOrEmpty(Name) == false
            && (string.IsNullOrEmpty(Password) == false
            || IsChecked == false);
    }
}
