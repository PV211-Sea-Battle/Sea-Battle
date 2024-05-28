using Sea_Battle.Infrastructure;
using Sea_Battle.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Sea_Battle.Views
{
    public partial class LobbyWindow : Window
    {
        public LobbyWindow()
        {
            InitializeComponent();

            DataContext = new LobbyWindowViewModel(this);

            for (int x = 0; x < 10; x++)
            {
                Border number = CreateBorder((x + 1).ToString());
                Border letter = CreateBorder(char.ConvertFromUtf32(x + 65));

                numbers.Children.Add(number);
                letters.Children.Add(letter);

                for (int y = 0; y < 10; y++)
                {
                    Button button = CreateButton();

                    button.Command = ((LobbyWindowViewModel)DataContext).CellCommand;
                    button.CommandParameter = 10 * x + y;

                    Binding shipBinding = new($"Field.Cells[{10 * x + y}].IsContainsShip")
                    {
                        Converter = new BoolToVisibilityConverter()
                    };
                    ((Border)button.Content).SetBinding(VisibilityProperty, shipBinding);

                    field.Children.Add(button);
                }
            }

            for (int count = 1, length = 4; count < 5 && length > 0; count++, length--)
            {
                UniformGrid grid = new()
                {
                    Columns = 5,
                    Rows = 2
                };

                Border border = CreateBorder();
                Binding binding = new($"Ships[{length - 1}]");
                ((TextBlock)border.Child).SetBinding(TextBlock.TextProperty, binding);

                grid.Children.Add(border);

                for (int x = 0; x < length; x++)
                {
                    Button button = CreateButton();
                    button.IsEnabled = false;

                    grid.Children.Add(button);
                }

                ships.Children.Add(grid);
                Grid.SetRow(grid, count);
            }
        }

        private Border CreateBorder(string? text = null)
            => new()
            {
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1),
                Child = new TextBlock()
                {
                    Style = (Style)FindResource("TextBlockDarkStyle"),
                    Text = text
                }
            };

        private Button CreateButton()
            => new()
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                Padding = new Thickness(5),
                Content = new Border()
                {
                    Background = Brushes.Red
                }
            };

        private void Window_Closed(object sender, EventArgs e)
            => ((LobbyWindowViewModel)DataContext).cancellationTokenSource.Cancel();
    }
}
