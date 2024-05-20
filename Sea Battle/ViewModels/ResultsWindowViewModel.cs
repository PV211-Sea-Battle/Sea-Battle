using Models;
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
        public readonly CancellationTokenSource cancellationTokenSource = new();
        public ObservableCollection<string> LogList { get; set; } = [];
        public int ReadyPlayers { get; set; } = 0;
        public ICommand BackCommand { get; set; }
        public ICommand RematchCommand { get; set; }
        public string ImagePath { get; set; }
        public ResultsWindowViewModel()
        {
            if (CurrentUser.user.IsWinner)
            {
                ImagePath = "../Resources/Victory.jpg";
            }
            else
            {
                ImagePath = "../Resources/Defeat.jpg";
            }

            Task.Run(Refresh);

            RematchCommand = new RelayCommand(Rematch);
            BackCommand = new RelayCommand<Window>(Back);
        }
        private async void Refresh()
        {
            while (cancellationTokenSource.IsCancellationRequested == false)
            {
                try
                {
                    Request request = new()
                    {
                        Header = "ENEMY WAIT",
                        User = CurrentUser.user,
                        Game = CurrentUser.game
                    };

                    Response response = await CurrentUser.SendMessageAsync(request);

                    if (response.Game?.ClientUser is User client)
                    {
                        if (CurrentUser.user?.Login == client.Login)
                        {
                            if (CurrentUser.game?.HostUser.IsReady != response.Game.HostUser.IsReady)
                            {
                                ReadyPlayers += response.Game.HostUser.IsReady ? 1 : -1;
                                Log($"{response.Game.HostUser} is{(response.Game.HostUser.IsReady ? "" : " not")} Ready");
                            }
                        }
                        else
                        {
                            if (CurrentUser.game?.ClientUser is null)
                            {
                                Log($"{response.Game.ClientUser} Connected");
                            }
                            else if (CurrentUser.game.ClientUser.IsReady != client.IsReady)
                            {
                                ReadyPlayers += client.IsReady ? 1 : -1;
                                Log($"{client.Login} is{(client.IsReady ? "" : " not")} Ready");
                            }
                        }
                    }

                    if (ReadyPlayers == 2)
                    {
                        // Окно построения защиты
                    }

                    CurrentUser.game = response.Game;

                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private async void Rematch()
        {
            try
            {
                Request request = new()
                {
                    Header = "REMATCH",
                    User = CurrentUser.user,
                    Game = CurrentUser.game
                };

                await CurrentUser.SendMessageAsync(request);

                if (CurrentUser.user.IsReady)
                {
                    CurrentUser.user.IsReady = false;
                    ReadyPlayers--;
                    Log($"{CurrentUser.user} is not Ready");
                }
                else
                {
                    CurrentUser.user.IsReady = true;
                    ReadyPlayers++;
                    Log($"{CurrentUser.user} is Ready.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        private async void Back(Window window)
        {
            Log($"{CurrentUser.user} left the game.");
            CurrentUser.SwitchWindow<MainMenuWindow>(window);
        }
        private void Log(string message)
            => Application.Current.Dispatcher.Invoke(() =>
            {
                LogList.Add($"{DateTime.Now.ToShortTimeString()} : {message}");
            });
    }
}
