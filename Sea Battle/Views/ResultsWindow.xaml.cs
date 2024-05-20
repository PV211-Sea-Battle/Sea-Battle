﻿using System.Windows;
using Sea_Battle.ViewModels;

namespace Sea_Battle
{
    public partial class ResultsWindow : Window
    {
        public ResultsWindow()
        {
            InitializeComponent();
            DataContext = new ResultsWindowViewModel();
        }
        private void Window_Closed(object sender, EventArgs e)
            => ((ResultsWindowViewModel)DataContext).cancellationTokenSource.Cancel();
    }
}
