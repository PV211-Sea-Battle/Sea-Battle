using PropertyChanged;
using Sea_Battle.Infrastructure;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Sea_Battle.Views;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class ResultsWindowViewModel
    {
        public ObservableCollection<string> LogList { get; set; } = [];
        public ICommand BackCommand { get; set; }
        public string ImagePath { get; set; }
        public ResultsWindowViewModel()
        {
            if (CurrentUser.game?.Winner?.Login == CurrentUser.user?.Login)
            {
                ImagePath = "../Resources/Victory.jpg";
            }
            else
            {
                ImagePath = "../Resources/Defeat.jpg";
            }

            BackCommand = new RelayCommand<Window>(Back);
        }
        private void Back(Window window)
            => CurrentUser.SwitchWindow<MainMenuWindow>(window);
        private void Log(string message)
            => Application.Current.Dispatcher.Invoke(() =>
            {
                LogList.Add($"{DateTime.Now.ToShortTimeString()} : {message}");
            });
    }
}
