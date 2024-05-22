using Sea_Battle.Infrastructure;
using Sea_Battle.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sea_Battle
{
    public partial class GameWindow : Window
    {
        
        public GameWindow()
        {
            InitializeComponent();
            DataContext = new GameWindowViewModel();

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
                    Button button;
                    if(((GameWindowViewModel)DataContext).CurrentUserField.Cells[10 * x + y].IsContainsShip)
                    {
                        button = CreateButton(1);
                    }
                    else
                    {
                        button = CreateButton(2);
                    }

                    button.IsEnabled = false;
                    u_field.Children.Add(button);
                }

                for (int y = 0; y < 10; y++)
                {
                    Button button;
                    int buttonPosition = 10 * x + y;

                    if (((GameWindowViewModel)DataContext).OpponentField.Cells[buttonPosition].IsContainsShip)
                    {
                        button = CreateButton(1);
                    }
                    else if(((GameWindowViewModel)DataContext).OpponentField.Cells[buttonPosition].IsHit)
                    {
                        button = CreateButton(2);
                    }
                    else
                    {
                        button = CreateButton(3);
                    }

                    button.Command = ((GameWindowViewModel)DataContext).ShootCommand;
                    button.CommandParameter = buttonPosition;

                    Binding visibleBinding = new($"OpponentField.Cells[{buttonPosition}].IsVisible")
                    {
                        Converter = new BoolToVisibilityConverter()
                    };
                    ((Border)button.Content).SetBinding(VisibilityProperty, visibleBinding);

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

        private Button CreateButton(int id) {
            if (id == 1) {
                return new()
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    Padding = new Thickness(5),
                    Content = new Border()
                    {
                        Background = Brushes.Red
                    }
                };
            }
            else if(id == 2)
            {
                return new()
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    Padding = new Thickness(5),
                    Content = new Border()
                    {
                        Background = Brushes.Blue
                    }
                };
            }
            else
            {
                return new()
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    Padding = new Thickness(5),
                    Content = new Border()
                    {
                        Background = Brushes.Black
                    }
                };
            }
        }
        
        private void Window_Closed(object sender, EventArgs e)
            => ((GameWindowViewModel)DataContext).cancellationTokenSource.Cancel();
    }
}
