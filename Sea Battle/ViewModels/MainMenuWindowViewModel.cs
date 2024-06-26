﻿using Models;
using PropertyChanged;
using Sea_Battle.Infrastructure;
using Sea_Battle.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Sea_Battle.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class MainMenuWindowViewModel
    {
        private readonly Window window;
        public readonly CancellationTokenSource cancellationTokenSource = new();
        public ICollectionView SortedGames { get; set; }
        public ObservableCollection<Game> Games { get; set; } = [];
        public Game? SelectedGame { get; set; }
        public ICommand SortCommand { get; }
        public ICommand JoinCommand { get; }
        public ICommand CreateCommand { get; }
        public MainMenuWindowViewModel(Window window)
        {
            Request request = new()
            {
                Header = "OFFLINE",
                User = CurrentUser.user
            };

            _ = CurrentUser.SendMessageAsync(request, false);

            this.window = window;

            Task.Run(Refresh);

            SortedGames = CollectionViewSource.GetDefaultView(Games);
            SortedGames.SortDescriptions.Add(new("Name", ListSortDirection.Ascending));

            SortCommand = new RelayCommand<string>(Sort);
            JoinCommand = new RelayCommand(Join, CanJoin);
            CreateCommand = new RelayCommand(Create);
        }
        private async void Refresh()
        {
            while (cancellationTokenSource.IsCancellationRequested == false)
            {
                try
                {
                    Request request = new()
                    {
                        Header = "GAME LIST"
                    };

                    Response response = await CurrentUser.SendMessageAsync(request);

                    await window.Dispatcher.InvokeAsync(() =>
                    {
                        int? id = null;
                        if (SelectedGame is not null)
                        {
                            id = SelectedGame.Id;
                        }

                        Games.Clear();
                        foreach (Game game in response.Games!)
                        {
                            Games.Add(game);
                        }

                        if (id is not null)
                        {
                            SelectedGame = Games.FirstOrDefault(item => item.Id == id);
                        }
                    });

                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void Sort(string header)
        {
            SortDescriptionCollection sdc = SortedGames.SortDescriptions;
            if (sdc[0].PropertyName == header && sdc[0].Direction == ListSortDirection.Ascending)
            {
                sdc[0] = new SortDescription(header, ListSortDirection.Descending);
            }
            else
            {
                sdc[0] = new SortDescription(header, ListSortDirection.Ascending);
            }
        }
        public async void Join()
        {
            CurrentUser.game = SelectedGame!;

            if (CurrentUser.game.IsPrivate)
            {
                CurrentUser.SwitchWindow<ConnectWindow>(window);
            }
            else
            {
                try
                {
                    Request request = new()
                    {
                        Header = "JOIN",
                        User = CurrentUser.user,
                        Game = CurrentUser.game
                    };

                    Response response = await CurrentUser.SendMessageAsync(request);

                    CurrentUser.game = response.Game;

                    CurrentUser.SwitchWindow<LobbyWindow>(window);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void Create()
            => CurrentUser.SwitchWindow<CreateWindow>(window);
        public bool CanJoin()
            => SelectedGame is not null;
    }
}
