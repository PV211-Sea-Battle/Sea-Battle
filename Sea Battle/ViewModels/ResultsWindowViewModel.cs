﻿using PropertyChanged;
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
        private readonly Window window;
        public ObservableCollection<string> LogList { get; set; } = [];
        public ICommand BackCommand { get; set; }
        public string ImagePath { get; set; }
        public ResultsWindowViewModel(Window window)
        {
            this.window = window;

            if (CurrentUser.game?.Winner?.Login == CurrentUser.user?.Login)
            {
                ImagePath = "../Resources/Victory.jpg";
            }
            else
            {
                ImagePath = "../Resources/Defeat.jpg";
            }

            BackCommand = new RelayCommand(Back);
        }
        private void Back()
            => CurrentUser.SwitchWindow<MainMenuWindow>(window);
        private void Log(string message)
            => window.Dispatcher.Invoke(() =>
            {
                LogList.Add($"{DateTime.Now.ToShortTimeString()} : {message}");
            });
    }
}
