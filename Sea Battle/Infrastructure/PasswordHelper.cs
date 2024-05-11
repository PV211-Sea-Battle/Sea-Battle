using System.Windows.Controls;
using System.Windows;

namespace Sea_Battle.Infrastructure
{
    public static class PasswordHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
                typeof(string),
                typeof(PasswordHelper),
                new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
                typeof(bool),
                typeof(PasswordHelper),
                new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating",
                typeof(bool),
                typeof(PasswordHelper));
        public static void SetAttach(DependencyObject dp, bool value)
            => dp.SetValue(AttachProperty, value);
        public static string GetPassword(DependencyObject dp)
            => (string)dp.GetValue(PasswordProperty);
        public static void SetPassword(DependencyObject dp, string value)
            => dp.SetValue(PasswordProperty, value);
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;

            if ((bool)passwordBox.GetValue(IsUpdatingProperty) == false)
            {
                passwordBox.Password = (string)e.NewValue;
            }
        }
        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;

            passwordBox.PasswordChanged += PasswordChanged;
        }
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;

            passwordBox.SetValue(IsUpdatingProperty, true);
            SetPassword(passwordBox, passwordBox.Password);
            passwordBox.SetValue(IsUpdatingProperty, false);
        }
    }
}
