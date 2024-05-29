using Sea_Battle.Infrastructure;
using Sea_Battle.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sea_Battle.Views
{
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();

            DataContext = new GameWindowViewModel(this);

            for (int x = 0; x < 10; x++)
            {
                Border u_number = CreateBorder((x + 1).ToString());
                Border u_letter = CreateBorder(char.ConvertFromUtf32(x + 65));
                Border o_number = CreateBorder((x + 1).ToString());
                Border o_letter = CreateBorder(char.ConvertFromUtf32(x + 65));

                u_numbers.Children.Add(u_number);
                u_letters.Children.Add(u_letter);
                o_numbers.Children.Add(o_number);
                o_letters.Children.Add(o_letter);

                for (int y = 0; y < 10; y++)
                {
                    Button button = CreateButton();
                    button.IsEnabled = false;
                    int buttonPosition = 10 * x + y;

                    Binding shipBinding = new($"CurrentUserField.Cells[{buttonPosition}].IsContainsShip")
                    {
                        Converter = new BoolToVisibilityConverter()
                    };
                    Binding hitBinding = new($"CurrentUserField.Cells[{buttonPosition}].IsHit")
                    {
                        Converter = new BoolToVisibilityConverter()
                    };

                    ((Border)((Grid)button.Content).Children[0]).SetBinding(VisibilityProperty, shipBinding);
                    ((Path)((Grid)button.Content).Children[1]).SetBinding(VisibilityProperty, hitBinding);

                    u_field.Children.Add(button);
                }

                for (int y = 0; y < 10; y++)
                {
                    Button button = CreateButton();
                    int buttonPosition = 10 * x + y;

                    button.Command = ((GameWindowViewModel)DataContext).ShootCommand;
                    button.CommandParameter = buttonPosition;

                    Binding shipBinding = new($"OpponentField.Cells[{buttonPosition}].IsContainsShip")
                    {
                        Converter = new BoolToVisibilityConverter()
                    };
                    Binding hitBinding = new($"OpponentField.Cells[{buttonPosition}].IsHit")
                    {
                        Converter = new BoolToVisibilityConverter()
                    };

                    ((Border)((Grid)button.Content).Children[0]).SetBinding(VisibilityProperty, shipBinding);
                    ((Path)((Grid)button.Content).Children[1]).SetBinding(VisibilityProperty, hitBinding);

                    o_field.Children.Add(button);
                }
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
        {
            Button button = new()
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                Padding = new Thickness(5)
            };

            Grid grid = new();
            button.Content = grid;

            Border border = new()
            {
                Background = Brushes.Red
            };

            Path path = new()
            {
                Stretch = Stretch.Fill,
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                Data = Geometry.Parse("M 0 0 L 1 1 M 0 1 L 1 0")
            };

            grid.Children.Add(border);
            grid.Children.Add(path);

            return button;
        }

        private void Window_Closed(object sender, EventArgs e)
            => ((GameWindowViewModel)DataContext).cancellationTokenSource.Cancel();
    }
}
