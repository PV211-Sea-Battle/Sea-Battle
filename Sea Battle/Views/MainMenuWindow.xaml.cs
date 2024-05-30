using Sea_Battle.ViewModels;
using System.Windows;

namespace Sea_Battle.Views
{
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();

            DataContext = new MainMenuWindowViewModel(this);
        }

        private void Window_Closed(object sender, EventArgs e)
            => ((MainMenuWindowViewModel)DataContext).cancellationTokenSource.Cancel();
    }
}
