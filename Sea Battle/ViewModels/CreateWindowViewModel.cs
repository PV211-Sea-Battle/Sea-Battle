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
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }
        public CreateWindowViewModel()
        {
            CreateCommand = new RelayCommand<Window>(Create, CanCreate);
            CancelCommand = new RelayCommand<Window>(Cancel);
        }
        public async void Create(Window window)
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
                
                // Построение защиты
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                Name = string.Empty;
                Password = string.Empty;
            }
        }
        public void Cancel(Window window)
            => CurrentUser.SwitchWindow<MainMenuWindow>(window);
        public bool CanCreate()
            => string.IsNullOrEmpty(Name) == false
            && (IsChecked == false
            || string.IsNullOrEmpty(Password) == false);
    }
}
