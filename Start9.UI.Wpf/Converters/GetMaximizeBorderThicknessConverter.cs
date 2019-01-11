using Start9.UI.Wpf.Statics;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Start9.UI.Wpf.Converters
{
    public class GetMaximizeBorderThicknessConverter : IValueConverter
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double verticalWidth = SystemScaling.RealPixelsToWpfUnits(GetSystemMetrics(46) + GetSystemMetrics(32)); //32
            double horizontalHeight = SystemScaling.RealPixelsToWpfUnits(GetSystemMetrics(45) + GetSystemMetrics(33)); //33
            //double borderSize = SystemParameters.ResizeFrameVerticalBorderWidth + SystemParameters.FixedFrameVerticalBorderWidth - SystemParameters.BorderWidth;
            return new Thickness(verticalWidth - 1, horizontalHeight - 1, (verticalWidth - 1) * -1, (horizontalHeight - 1) * -1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
