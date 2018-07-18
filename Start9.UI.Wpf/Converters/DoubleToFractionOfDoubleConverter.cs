using System;
using System.Globalization;
using System.Windows.Data;

namespace Start9.UI.Wpf.Converters
{
    public class DoubleToFractionOfDoubleConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return ((Double)value) / 2;
            }
            else
            {
                var paramString = (String)parameter;
                return ((Double)value) / System.Convert.ToDouble(paramString);
            }
        }

        public Object ConvertBack(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                return ((Double)value) * 2;
            }
            else
            {
                var paramString = (String)parameter;
                return ((Double)value) * System.Convert.ToDouble(paramString);
            }
        }
    }
}
