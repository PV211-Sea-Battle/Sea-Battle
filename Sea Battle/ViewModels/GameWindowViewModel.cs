using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using Sea_Battle.Infrastructure;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class GameWindowViewModel
    {
        public readonly CancellationTokenSource cancellationTokenSource = new();
        public Field CurrentUserField { get; set; } = new() { Cells = new Cell[100].ToList() };
        public Field OpponentField { get; set; } = new() { Cells = new Cell[100].ToList() };
        public ICommand ShootCommand { get; set; }

        public GameWindowViewModel()
        {
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
                        Header = "GAME",
                        User = CurrentUser.user,
                        Game = CurrentUser.game
                    };

                    Response response = await CurrentUser.SendMessageAsync(request);

                    if(response.User.Login == CurrentUser.user.Login)
                    {
                        CurrentUser.user.IsTurn = true;
                    }
                    else
                    {
                        CurrentUser.user.IsTurn = false;
                    }

                    if(response.Game.HostUser.Login == CurrentUser.user.Login)
                    {
                        CurrentUserField = response.Game.HostField;
                        OpponentField = response.Game.ClientField;
                    }
                    else
                    {
                        CurrentUserField = response.Game.ClientField;
                        OpponentField = response.Game.HostField;
                    }

                    if(response.User.IsWinner == true && response.User.Login == CurrentUser.user.Login)
                    {
                        CurrentUser.user.IsWinner = true;
                        //окно результатов (победа)
                    }
                    if (response.User.IsWinner == true && response.User.Login != CurrentUser.user.Login)
                    {
                        //окно резуьтатов (поражение)
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
            OpponentField.Cells[index].IsVisible ^= true;
            try
            {
                Request request = new()
                {
                    Header = "SHOOT",
                    Field = OpponentField,
                    User = CurrentUser.user,
                    Game = CurrentUser.game
                };

                Response response = await CurrentUser.SendMessageAsync(request);

                //работа с этой клеткой: ее смена и прочее

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        private bool CanShoot()
            => CurrentUser.user.IsTurn == true;
    }
}
