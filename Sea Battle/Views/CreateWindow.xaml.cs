using Sea_Battle.ViewModels;
using System.Windows;

namespace Sea_Battle
{
    public partial class CreateWindow : Window
    {
        public CreateWindow()
        {
            InitializeComponent();

            DataContext = new CreateWindowViewModel();
        }
    }
}
