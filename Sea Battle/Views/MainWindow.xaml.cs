using Sea_Battle.ViewModels;
using System.Windows;

namespace Sea_Battle
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}