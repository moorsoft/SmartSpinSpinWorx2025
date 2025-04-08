using SmartSpin.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace SmartSpin.Converters
{
    [ValueConversion(typeof(ParameterViewModel), typeof(string))]
    public class ParameterConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ParameterViewModel)
            {
                ParameterViewModel param = value as ParameterViewModel;
                if (string.IsNullOrEmpty(param.Format))
                {
                    return param.SettingValue.ToString();
                }
                else
                {
                    return string.Format("{0:"+param.Format+"}", param.SettingValue);
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
