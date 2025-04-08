using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace SmartSpin.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class ButtonSelectorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is string)
            {
                int.TryParse((string)parameter, out int par);
                parameter = par;
            }
            if ((value is int) && (parameter is int)) { 
                if ((int)value == (int)parameter)
                {
                    return "#AAAAAA";
                }
            }
            return "#595959";
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

    [ValueConversion(typeof(bool), typeof(string))]
    public class BButtonSelectorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is string)
            {
                bool.TryParse((string)parameter, out bool par);
                parameter = par;
            }
            if ((value is bool) && (parameter is bool))
            {
                if ((bool)value == (bool)parameter)
                {
                    return "#AAAAAA";
                }
            }
            return "#595959";
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
