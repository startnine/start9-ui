using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Start9.UI.Wpf.Converters
{
    public class ClassicFrameSizeConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
#if NET40
            var frame = new Thickness(2.5); // TODO: fix
#else
            var frame = SystemParameters.WindowResizeBorderThickness;
#endif
            return frame;
        }

        public Object ConvertBack(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
