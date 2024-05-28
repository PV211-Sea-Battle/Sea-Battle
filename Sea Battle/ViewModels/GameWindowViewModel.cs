using PropertyChanged;
using Models;
using System.Windows;
using System.Windows.Input;
using Sea_Battle.Infrastructure;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class GameWindowViewModel
    {
        public readonly CancellationTokenSource cancellationTokenSource = new();
        public Field CurrentUserField { get; set; }
        public Field OpponentField { get; set; }
        public ICommand ShootCommand { get; set; }

        public GameWindowViewModel()
        {
            if (CurrentUser.game.HostUser.Login == CurrentUser.user.Login)
            {
                CurrentUserField = CurrentUser.game.HostField;
                OpponentField = CurrentUser.game.ClientField;
            }
            else
            {
                CurrentUserField = CurrentUser.game.ClientField;
                OpponentField = CurrentUser.game.HostField;
            }

            Task.Run(Refresh);

            ShootCommand = new RelayCommand<int>(Shoot, CanShoot);
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

                    if (response.Game.HostUser.Login == CurrentUser.user.Login)
                    {
                        CurrentUserField = response.Game.HostField;
                        OpponentField = response.Game.ClientField;
                    }
                    else
                    {
                        CurrentUserField = response.Game.ClientField;
                        OpponentField = response.Game.HostField;
                    }

                    if (response.Game.HostUser.Login == CurrentUser.user.Login)
                    {
                        CurrentUser.user = response.Game.HostUser;
                    }
                    else
                    {
                        CurrentUser.user = response.Game.ClientUser;
                    }

                    CurrentUser.game = response.Game;

                    if (response.Game.Winner is not null)
                    {
                        //окно результатов
                    }

                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private async void Shoot(int index)
        {
            try
            {
                Request request = new()
                {
                    Header = "SHOOT",
                    Cell = OpponentField.Cells[index],
                    User = CurrentUser.user,
                    Game = CurrentUser.game
                };

                await CurrentUser.SendMessageAsync(request);

                CurrentUser.user.IsTurn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        private bool CanShoot()
            => CurrentUser.user.IsTurn;
    }
}
