using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace SmartSpin.Converters
{
    [ValueConversion(typeof(object), typeof(System.Windows.Visibility))]
    public class ObjectToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value != null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
