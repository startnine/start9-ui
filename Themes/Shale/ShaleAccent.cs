using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DrawColor = System.Drawing.Color;
using ColorMine;
using ColorMine.ColorSpaces;
using System.Diagnostics;

namespace Start9.Wpf.Styles.Shale
{
    //http://hslpicker.com/ is useful for manual rgb --> hsl conversions
    public static class ShaleAccents
    {
        /// <summary>
        /// #FF8DD3D6
        /// </summary>
        public static ShaleAccent Sky => new ShaleAccent(183, 48);

        /// <summary>
        /// #1E2B73
        /// </summary>
        public static ShaleAccent Ocean => new ShaleAccent(231, 59);

        /// <summary>
        /// #FFBCB3A7
        /// </summary>
        public static ShaleAccent Sand => new ShaleAccent(33, 13);

        /// <summary>
        /// #FF8577
        /// </summary>
        public static ShaleAccent Coral => new ShaleAccent(6, 100);

        /// <summary>
        /// #538049
        /// </summary>
        public static ShaleAccent Palm => new ShaleAccent(109, 27);

        /// <summary>
        /// #FDC82C
        /// </summary>
        public static ShaleAccent Sunlight => new ShaleAccent(45, 98);
    }
    public class ShaleAccent
    {
        double _hue = 0;
        public double Hue
        {
            get => _hue;
            set
            {
                _hue = value;
                UpdateColors();
            }
        }


        double _saturation = 0;
        public double Saturation
        {
            get => _saturation;
            set
            {
                _saturation = value;
                UpdateColors();
            }
        }

        public ShaleAccent(Color accentColor)
        {
            DrawColor color = DrawColor.FromArgb(1, accentColor.R, accentColor.G, accentColor.B);
            Hue = color.GetHue();
            Saturation = color.GetHue();
        }

        public ShaleAccent(double hue, double saturation)
        {
            Hue = hue;
            Saturation = saturation;
        }

        public ResourceDictionary Dictionary = new ResourceDictionary()
        {
            Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Colors/Accent.xaml", UriKind.RelativeOrAbsolute)
        };

        private void UpdateColors()
        {
            foreach (string s in Dictionary.Keys)
            {
                if (Dictionary[s] is Color color)
                {
                    Hsl hsl = (new Rgb() { R = color.R, G = color.G, B = color.B }).To<Hsl>();
                    hsl.S = _saturation;
                    hsl.H = _hue;
                    if (s.Contains("DarkColor"))
                        hsl.S /= 3; 

                    Rgb rgb = hsl.To<Rgb>();
                    color.R = (byte)rgb.R;
                    color.G = (byte)rgb.G;
                    color.B = (byte)rgb.B;

                    Dictionary[s] = color;
                }
            }
        }

        public SolidColorBrush Brush
        {
            get
            {
                /*UpdateColors();
                return (Color)Dictionary["ButtonPressedBorderLightColor"];*/
                var rgb = new Hsl()
                {
                    H = _hue,
                    S = _saturation,
                    L = 75
                }.ToRgb();
                return new SolidColorBrush(Color.FromArgb(0xFF, (byte)rgb.R, (byte)rgb.G, (byte)rgb.B));
            }
        }
    }
}
