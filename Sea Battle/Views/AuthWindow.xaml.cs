using Sea_Battle.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Sea_Battle
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();

            DataContext = new AuthWindowViewModel();
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox)
            {
                return;
            }
            
            passwordBox.Visibility = checkBox.IsChecked 
                ?? true
                ? Visibility.Collapsed
                : Visibility.Visible;

            textBox.Visibility = checkBox.IsChecked
                ?? true
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}