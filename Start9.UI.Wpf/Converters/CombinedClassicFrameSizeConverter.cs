using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Start9.UI.Wpf.Converters
{
    public class CombinedClassicFrameSizeConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var frame = SystemParameters.WindowResizeBorderThickness;
            frame.Top = SystemParameters.CaptionHeight;
            Debug.WriteLine("frame: " + frame);
            return frame;
        }

        public Object ConvertBack(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
