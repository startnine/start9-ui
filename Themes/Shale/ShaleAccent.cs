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
        public static ShaleAccent Blue => new ShaleAccent(183, 48);
        public static ShaleAccent Beige => new ShaleAccent(33, 13);
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
                    Rgb rgb = hsl.To<Rgb>();
                    color.R = (byte)rgb.R;
                    color.G = (byte)rgb.G;
                    color.B = (byte)rgb.B;

                    Dictionary[s] = color;
                }
            }
        }

        public Color ToColor()
        {
            return (Color)Dictionary["ButtonPressedBorderLightColor"];
        }
    }
}
