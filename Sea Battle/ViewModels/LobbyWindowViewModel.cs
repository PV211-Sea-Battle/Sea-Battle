using Models;
using PropertyChanged;
using Sea_Battle.Infrastructure;
using Sea_Battle.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class LobbyWindowViewModel
    {
        private readonly Window window;
        public readonly CancellationTokenSource cancellationTokenSource = new();
        public Field Field { get; set; } = new() { Cells = new Cell[100].Select(_ => new Cell()).ToList() };
        public ObservableCollection<int> Ships { get; set; } = [4, 3, 2, 1];
        public ObservableCollection<string> LogList { get; set; } = [];
        public int ReadyPlayers { get; set; }
        public ICommand CellCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ReadyCommand { get; set; }
        public LobbyWindowViewModel(Window window)
        {
            this.window = window;

            Task.Run(Refresh);

            Log($"{CurrentUser.user} Connected");

            CellCommand = new RelayCommand<int>(Cell, CanCell);
            ResetCommand = new RelayCommand(Reset);
            ReadyCommand = new RelayCommand(Ready, CanReady);
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

                    CurrentUser.game = response.Game;

                    if (ReadyPlayers == 2)
                    {
                        CurrentUser.user!.IsTurn = CurrentUser.user?.Login == CurrentUser.game?.HostUser.Login;

                        window.Dispatcher.Invoke(() =>
                        {
                            CurrentUser.SwitchWindow<GameWindow>(window);
                        });
                    }

                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CountShips()
        {
            Ships = [4, 3, 2, 1];
            bool[] field = Field.Cells
                .Select(item => item.IsContainsShip)
                .ToArray();

            while (Array.IndexOf(field, true) is int index && index != -1)
            {
                int size = 1;

                field[index] = false;

                while ((index + 1) % 10 != 0 && field[index + 1])
                {
                    index++;
                    size++;
                    field[index] = false;
                }

                while (index + 10 < field.Length && field[index + 10])
                {
                    index += 10;
                    size++;
                    field[index] = false;
                }

                if (size - 1 < Ships.Count)
                {
                    Ships[size - 1]--;
                }
            }
        }

        private void Log(string message) 
            => window.Dispatcher.Invoke(() =>
            {
                LogList.Add($"{DateTime.Now.ToShortTimeString()} : {message}");
            });

        private void Cell(int index)
        {
            Field.Cells[index].IsContainsShip ^= true;

            CountShips();
        }

        private void Reset()
        {
            for (int x = 0; x < 100; x++)
            {
                Field.Cells[x].IsContainsShip = false;
            }

            CountShips();
        }

        private async void Ready()
        {
            try
            {
                Request request = new()
                {
                    Header = "READY",
                    Field = Field,
                    User = CurrentUser.user,
                    Game = CurrentUser.game
                };

                await CurrentUser.SendMessageAsync(request);

                if (CurrentUser.user!.IsReady)
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

        private bool CanReady()
            => Ships.Count(item => item == 0) == 4;

        private bool CanCell()
            => CurrentUser.user?.IsReady == false;
    }
}
