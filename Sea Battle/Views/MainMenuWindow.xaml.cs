using Sea_Battle.ViewModels;
using System.Windows;

namespace Sea_Battle.Views
{
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();

            DataContext = new MainMenuWindowViewModel();
        }
    }
}
