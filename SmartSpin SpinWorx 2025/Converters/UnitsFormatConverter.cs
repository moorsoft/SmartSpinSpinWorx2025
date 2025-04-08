using SmartSpin.Hardware;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace SmartSpin.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class UnitsFormatConverter : MarkupExtension,  IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                if (Machine.Metric)
                {
                    return $"{(double)value:F2}";
                }
                else
                {
                    return $"{(double)value:F3}";
                }
            }
            return "";
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
