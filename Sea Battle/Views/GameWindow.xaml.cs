using Sea_Battle.Infrastructure;
using Sea_Battle.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

            CreateField(u_numbers, u_letters, u_field, true);
            CreateField(o_numbers, o_letters, o_field, false);
        }

        private void CreateField(UniformGrid numbers, UniformGrid letters, UniformGrid field, bool userField)
        {
            for (int x = 0; x < 10; x++)
            {
                Border number = CreateBorder((x + 1).ToString());
                Border letter = CreateBorder(char.ConvertFromUtf32(x + 65));

                numbers.Children.Add(number);
                letters.Children.Add(letter);

                for (int y = 0; y < 10; y++)
                {
                    int buttonPosition = 10 * x + y;

                    Button button = CreateButton(buttonPosition, userField);

                    button.IsEnabled = userField == false;

                    if (userField == false)
                    {
                        button.CommandParameter = buttonPosition;
                        button.Command = ((GameWindowViewModel)DataContext).ShootCommand;
                    }

                    field.Children.Add(button);
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

        private static Button CreateButton(int index, bool userField)
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

            string field = userField ? "CurrentUserField" : "OpponentField";

            Binding shipBinding = new($"{field}.Cells[{index}].IsContainsShip")
            {
                Converter = new BoolToVisibilityConverter()
            };
            Binding hitBinding = new($"{field}.Cells[{index}].IsHit")
            {
                Converter = new BoolToVisibilityConverter()
            };

            border.SetBinding(VisibilityProperty, shipBinding);
            path.SetBinding(VisibilityProperty, hitBinding);

            grid.Children.Add(border);
            grid.Children.Add(path);

            return button;
        }

        private void Window_Closed(object sender, EventArgs e)
            => ((GameWindowViewModel)DataContext).cancellationTokenSource.Cancel();
    }
}
