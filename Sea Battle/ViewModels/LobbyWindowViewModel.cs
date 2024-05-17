using Models;
using PropertyChanged;
using Sea_Battle.Infrastructure;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class LobbyWindowViewModel
    {
        private bool isReady;
        public readonly CancellationTokenSource cancellationTokenSource = new();
        public Field Field { get; set; } = new() { Cells = new Cell[100].ToList() };
        public ObservableCollection<int> Ships { get; set; } = [4, 3, 2, 1];
        public ObservableCollection<string> LogList { get; set; } = [];
        public int ReadyPlayers { get; set; }
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
            ReadyCommand = new RelayCommand(Ready);
        }

        private async void Refresh()
        {
            while (cancellationTokenSource.IsCancellationRequested == false)
            {
                // ReadyPlayers - готовые игроки
                // Log - метод для логирования

                await Task.Delay(100);
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

                try
                {
                    while (index % 9 != 0 && field[index + 1])
                    {
                        index++;
                        size++;
                        field[index] = false;
                    }

                    while (field[index += 10])
                    {
                        size++;
                        field[index] = false;
                    }

                    Ships[size - 1]--;
                }
                catch { }
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
            // ReadyPlayers - готовые игроки
            // Log - метод для логирования
            // isReady - false, если игрок готов

            isReady ^= true;
        }

        private bool CanCell()
            => isReady == false;
    }
}
