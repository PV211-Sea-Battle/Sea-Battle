using PropertyChanged;
using Models;
using System.Windows;
using System.Windows.Input;
using Sea_Battle.Infrastructure;
using Sea_Battle.Views;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class GameWindowViewModel
    {
        private readonly Window window;
        public readonly CancellationTokenSource cancellationTokenSource = new();
        public Field CurrentUserField { get; set; } = null!;
        public Field OpponentField { get; set; } = null!;
        public ICommand ShootCommand { get; set; }

        public GameWindowViewModel(Window window)
        {
            this.window = window;

            Task.Run(RefreshThread);

            ShootCommand = new RelayCommand<int>(Shoot, CanShoot);
        }
        private async void RefreshThread()
        {
            while (cancellationTokenSource.IsCancellationRequested == false)
            {
                await Refresh();
                await Task.Delay(100);
            }
        }
        private async Task Refresh()
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

                if (response.Game?.HostUser.Id == CurrentUser.user?.Id)
                {
                    CurrentUser.user = response.Game?.HostUser;

                    CurrentUserField = response.Game?.HostField!;
                    OpponentField = response.Game?.ClientField!;
                }
                else
                {
                    CurrentUser.user = response.Game?.ClientUser;

                    CurrentUserField = response.Game?.ClientField!;
                    OpponentField = response.Game?.HostField!;
                }

                CurrentUser.game = response.Game;

                if (response.Game?.Winner is not null)
                {
                    window.Dispatcher.Invoke(() =>
                    {
                        CurrentUser.SwitchWindow<ResultsWindow>(window);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Shoot(int index)
        {
            try
            {
                Request request = new()
                {
                    Header = "SHOOT",
                    Field = OpponentField,
                    Index = index,
                    User = CurrentUser.user,
                    Game = CurrentUser.game
                };

                await CurrentUser.SendMessageAsync(request);

                CurrentUser.user!.IsTurn = false;

                await Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private bool CanShoot(int index)
            => CurrentUser.user!.IsTurn
            && OpponentField?.Cells[index].IsHit == false;
    }
}
