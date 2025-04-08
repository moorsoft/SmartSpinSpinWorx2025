using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace SmartSpin.Converters
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class MemoryEnabledConverter : MarkupExtension,  IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is string)
            {
                int.TryParse((string)parameter, out int par);
                parameter = par;
            }
            if ((value is int) && (parameter is int))
            {
                return ((int)value >= (int)parameter);
            }
            return false;
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
