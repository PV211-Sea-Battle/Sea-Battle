using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sea_Battle.Infrastructure
{
    class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static BoolToVisibilityConverter? converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
            => converter ??= new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool statement
            ? statement
            ? Visibility.Visible
            : Visibility.Collapsed
            : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
