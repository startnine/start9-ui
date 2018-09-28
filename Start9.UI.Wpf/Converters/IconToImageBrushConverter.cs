using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Start9.UI.Wpf.Statics.SystemScaling;

namespace Start9.UI.Wpf.Converters
{
    public class IconToImageBrushConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var icon = (value as Icon);
            if (icon != null)
            {
                int param = 32;

                /*if (parameter != null)
                    Debug.WriteLine("parameter type: " + parameter.GetType().ToString());*/

                if (parameter != null)
                    /*Debug.WriteLine("param parse outcome: " + */int.TryParse((string)parameter, out param)/* + " " + param.ToString())*/;

                int targetSize = param;
                if (param >= 256)//Start9.UI.Wpf.Statics.SystemScaling.ScalingFactor > 1)
                    targetSize = WpfUnitsToRealPixels(param);
                return new ImageBrush(Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(targetSize, targetSize)));
                //BitmapSizeOptions.FromWidthAndHeight(WpfUnitsToRealPixels(param), WpfUnitsToRealPixels(param))
            }
            else return new ImageBrush();
        }

        public Object ConvertBack(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}