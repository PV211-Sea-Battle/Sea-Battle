using Sea_Battle.ViewModels;
using System.Windows;

namespace Sea_Battle
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();

            DataContext = new AuthWindowViewModel();
        }
    }
}