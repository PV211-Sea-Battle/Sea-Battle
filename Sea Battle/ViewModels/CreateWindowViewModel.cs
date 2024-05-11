using Models;
using PropertyChanged;
using Sea_Battle.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class CreateWindowViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsChecked { get; set; } = false;
        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }
        public CreateWindowViewModel()
        {
            CreateCommand = new RelayCommand<string>(Create, CanCreate);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }
        public async void Create(string header)
        {
            try
            {
                var request = new Request()
                {
                    Header = header,
                    User = CurrentUser.user,
                    Game = new Game
                    {
                        Name = Name,
                        IsPrivate = IsChecked,
                        Password = Password
                    }
                };

                await CurrentUser.SendMessageAsync(request);
                //CurrentUser.SwitchWindow<PrepareWindow>(this);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                Name = string.Empty;
                Password = string.Empty;
            }
        }
        public void Cancel()
        {
            //CurrentUser.SwitchWindow<MainMenuWindow>(this);
        }
        public bool CanCreate()
        {
            return !string.IsNullOrEmpty(Name) && ((!string.IsNullOrEmpty(Password) && IsChecked) || !IsChecked);
        }
        public bool CanCancel()
            => true;

    }
}
