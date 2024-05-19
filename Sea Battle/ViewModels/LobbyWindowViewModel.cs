using Models;
using PropertyChanged;
using Sea_Battle.Infrastructure;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class LobbyWindowViewModel
    {
        public readonly CancellationTokenSource cancellationTokenSource = new();
        public Field Field { get; set; } = new() { Cells = new Cell[100].ToList() };
        public ObservableCollection<int> Ships { get; set; } = [4, 3, 2, 1];
        public ObservableCollection<string> LogList { get; set; } = [];
        public int ReadyPlayers { get; set; } = 0;
        public ICommand CellCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ReadyCommand { get; set; }
        public LobbyWindowViewModel()
        {
            Task.Run(Refresh);

            Log($"{CurrentUser.user} Connected");

            for (int x = 0; x < 100; x++)
            {
                Field.Cells[x] = new();
            }

            CellCommand = new RelayCommand<int>(Cell, CanCell);
            ResetCommand = new RelayCommand(Reset);
            ReadyCommand = new RelayCommand(Ready, CanReady);
        }

        private async void Refresh()
        {
            while (cancellationTokenSource.IsCancellationRequested == false)
            {
                var request = new Request
                {
                    Header = "ENEMY WAIT",
                    User = CurrentUser.user,
                    Game = CurrentUser.game
                };
                try
                {
                    var responce = await CurrentUser.SendMessageAsync(request);
                    if (responce.ErrorMessage is not null)
                    {
                        MessageBox.Show(responce.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                    if(responce.User.IsReady)
                    {
                        //тут можно сделать активной для хоста кнопку "начать", или что то в этом роде
                    }
                    else
                    {
                        //а тут сделать противоположное действие
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
            => LogList.Add($"{DateTime.Now.ToShortTimeString()} : {message}");

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
            
            var request = new Request
            {
                Header = "READY",
                Field = Field,
                User = CurrentUser.user,
                Game = CurrentUser.game
            };
            try
            {
                var responce = await CurrentUser.SendMessageAsync(request);
                if (responce.ErrorMessage is not null)
                    throw new Exception(responce.ErrorMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CurrentUser.user.IsReady)
            {
                CurrentUser.user.IsReady = false;
                ReadyPlayers--;
                Log($"{CurrentUser.user} canceled his readiness.");
            }
            else
            {
                CurrentUser.user.IsReady = true;
                ReadyPlayers++;
                Log($"{CurrentUser.user} is ready.");
            }
        }

        private bool CanReady()
            => Ships.Count(item => item == 0) == 4;

        private bool CanCell()
            => CurrentUser.user.IsReady == false;
    }
}
