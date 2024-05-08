using Sea_Battle.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Sea_Battle
{
    public partial class ConnectWindow : Window
    {
        public ConnectWindow()
        {
            InitializeComponent();

            DataContext = new ConnectWindowViewModel();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ConnectWindowViewModel viewModel)
            {
                viewModel.Password = (sender as PasswordBox).Password;
            }
        }
    }
}
