using Start9.UI.Wpf.Statics;
using Start9.UI.Wpf.Windows;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;

namespace Start9.UI.Wpf.Converters
{
    public class GetMaximizeBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var win = value as CompositingWindow;
            NativeMethods.GetWindowRect(win.Handle, out NativeMethods.RECT winRect);
            //Debug.WriteLine("e");
            //NativeMethods.DwmGetWindowAttribute(win.Handle, NativeMethods.DwmWindowAttribute.ExtendedFrameBounds, out NativeMethods.RECT dwmRect, Marshal.SizeOf(typeof(NativeMethods.RECT))); //sizeof(NativeMethods.RECT)
            //DwmGetWindowAttribute
            //Debug.WriteLine("winRect: " + winRect.Left.ToString() + ", " + winRect.Top.ToString() + ", " + winRect.Right.ToString() + ", " + winRect.Bottom.ToString());
            //Debug.WriteLine("dwmRect: " + dwmRect.Left.ToString() + ", " + dwmRect.Top.ToString() + ", " + dwmRect.Right.ToString() + ", " + dwmRect.Bottom.ToString());
            //double verticalWidth = SystemScaling.RealPixelsToWpfUnits(NativeMethods.GetSystemMetrics(46)); //+ NativeMethods.GetSystemMetrics(32)); //32
            //double horizontalHeight = SystemScaling.RealPixelsToWpfUnits(/*NativeMethods.GetSystemMetrics(45) + */NativeMethods.GetSystemMetrics(33)); //33
            //double borderSize = SystemParameters.ResizeFrameVerticalBorderWidth + SystemParameters.FixedFrameVerticalBorderWidth - SystemParameters.BorderWidth;
            var screen = System.Windows.Forms.Screen.FromHandle(win.Handle);
            double verticalWidth = screen.WorkingArea.Left - winRect.Left;
            double horizontalHeight = screen.WorkingArea.Top - winRect.Top;
            return new Thickness(verticalWidth, horizontalHeight, (verticalWidth) * -1, (horizontalHeight) * -1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
