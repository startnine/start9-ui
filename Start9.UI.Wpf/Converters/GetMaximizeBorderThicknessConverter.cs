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

            Debug.WriteLine("CONVERTING");
            //var win = value as CompositingWindow;
            //Debug.WriteLine("e");
            //NativeMethods.DwmGetWindowAttribute(win.Handle, NativeMethods.DwmWindowAttribute.ExtendedFrameBounds, out NativeMethods.RECT dwmRect, Marshal.SizeOf(typeof(NativeMethods.RECT))); //sizeof(NativeMethods.RECT)
            //DwmGetWindowAttribute
            //Debug.WriteLine("winRect: " + winRect.Left.ToString() + ", " + winRect.Top.ToString() + ", " + winRect.Right.ToString() + ", " + winRect.Bottom.ToString());
            //Debug.WriteLine("dwmRect: " + dwmRect.Left.ToString() + ", " + dwmRect.Top.ToString() + ", " + dwmRect.Right.ToString() + ", " + dwmRect.Bottom.ToString());
            //double verticalWidth = SystemScaling.RealPixelsToWpfUnits(NativeMethods.GetSystemMetrics(46)); //+ NativeMethods.GetSystemMetrics(32)); //32
            //double horizontalHeight = SystemScaling.RealPixelsToWpfUnits(/*NativeMethods.GetSystemMetrics(45) + */NativeMethods.GetSystemMetrics(33)); //33
            //double borderSize = SystemParameters.ResizeFrameVerticalBorderWidth + SystemParameters.FixedFrameVerticalBorderWidth - SystemParameters.BorderWidth;
            //Start9.UI.Wpf.MonitorInfo.AllMonitors

            MonitorInfo Monitor = null;
            foreach (MonitorInfo m in MonitorInfo.AllMonitors)
            {
                if (m.DeviceId == System.Windows.Forms.Screen.FromHandle((value as CompositingWindow).Handle).DeviceName)
                {
                    Monitor = m;
                    break;
                }
            }

            //var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)SystemScaling.WpfUnitsToRealPixels((value as CompositingWindow).Left), (int)SystemScaling.WpfUnitsToRealPixels((value as CompositingWindow).Top)));

            if (Monitor != null)
            {
                //NativeMethods.GetWindowRect((value as CompositingWindow).Handle, out NativeMethods.RECT winRect);
                double verticalWidth = Monitor.WorkAreaBounds.Left;// - winRect.Left;
                double horizontalHeight = Monitor.WorkAreaBounds.Top;// - winRect.Top;
                return new Thickness(verticalWidth, horizontalHeight, (verticalWidth) * -1, (horizontalHeight) * -1);
            }
            else
                throw new Exception("Monitor was null");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
