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
        public static ShaleAccent Blue => new ShaleAccent(183, 28);
        public static ShaleAccent Beige => new ShaleAccent(33, 13);
    }
    public class ShaleAccent
    {
        public double Hue = 0;
        public double Saturation = 0;

        public int DarkThemeValueOffset { get; set; } = -40;

        //public ResourceDictionary Dictionary => GetDictionary();

        public ShaleAccent(Color accentColor)
        {
            DrawColor color = DrawColor.FromArgb(1, accentColor.R, accentColor.G, accentColor.B);
            Hue = color.GetHue();
            Saturation = color.GetHue();
            //colorHsv = new Rgb() { R = accentColor.R, G = accentColor.G, B = accentColor.B }.To<Hsv>();
        }

        public ShaleAccent(double hue, double saturation)
        {
            Hue = hue;
            Saturation = saturation;
        }

        protected Color[] GetColors(int value)
        {
            return GetColors(value, value + DarkThemeValueOffset, 255);
        }

        protected Color[] GetColors(int value, byte alpha)
        {
            return GetColors(value, value + DarkThemeValueOffset, alpha);
        }

        protected Color[] GetColors(int lightThemeValue, int darkThemeValue)
        {
            return GetColors(lightThemeValue, darkThemeValue, 255);
        }

        protected Color[] GetColors(int lightThemeValue, int darkThemeValue, byte alpha)
        {
            Color[] colors = new Color[2];
            
            //light Shale color
            Rgb lRgb = (new Hsl() { H = Hue, S = Saturation, L = lightThemeValue }).To<Rgb>();
            colors[0] = Color.FromArgb(alpha, (byte)lRgb.R, (byte)lRgb.G, (byte)lRgb.B);

            //dark Shale color
            Rgb dRgb = (new Hsl() { H = Hue, S = Saturation, L = darkThemeValue }).To<Rgb>();
            colors[1] = Color.FromArgb(alpha, (byte)dRgb.R, (byte)dRgb.G, (byte)dRgb.B);
            //colors[1] = Color.FromArgb(alpha, (byte)(dRgb.R / 255), (byte)(dRgb.G / 255), (byte)(dRgb.B / 255));

            return colors;
        }

        private ResourceDictionary _dictionary;

        protected string _lightText = "LightColor";
        protected string _darkText = "DarkColor";
        //static string _var = "COLORVAR";
        public static string ColorNameFiller = "COLORVARColor";

        protected void AddColorsToDictionary(string name, Color[] colors)
        {
            if (name.Contains(ColorNameFiller) && (colors.Length >= 2))
            {
                string lightColor = name.Replace(ColorNameFiller, _lightText);
                //Debug.WriteLine("LIGHT COLOR: " + lightColor + " = " + colors[0]); //("Light color: " + ", " + colors[0].A + ", " + colors[0].R + ", " + colors[0].G + ", " + colors[0].B + ",,, " + colors[0].ToString());
                _dictionary[lightColor] = colors[0];
                _dictionary[name.Replace(ColorNameFiller, _darkText)] = colors[1];
            }
            else
            {
                string error = "Could not add colors to Dictionary:";

                if (!name.Contains(ColorNameFiller))
                    error += "\n    Color name does not contain color filler";

                if (colors.Length < 2)
                    error += "\n    Invalid number of colors";

                throw new InvalidOperationException(error);
            }
        }

        public ResourceDictionary GetDictionary()
        {
            _dictionary = new ResourceDictionary()
            {
                Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Colors/Accent.xaml", UriKind.RelativeOrAbsolute)
            };


            AddColorsToDictionary("ButtonPressedBorder" + ColorNameFiller, GetColors(61));

            AddColorsToDictionary("ButtonPressedBackground" + ColorNameFiller + "0", GetColors(89));
            AddColorsToDictionary("ButtonPressedBackground" + ColorNameFiller + "1", GetColors(77));

            AddColorsToDictionary("ButtonPressedSecondaryBackground" + ColorNameFiller + "0", GetColors(81));
            AddColorsToDictionary("ButtonPressedSecondaryBackground" + ColorNameFiller + "1", GetColors(73));

            AddColorsToDictionary("CardListViewSelectedBackgroundBackground" + ColorNameFiller + "0", GetColors(89));
            AddColorsToDictionary("CardListViewSelectedBackgroundBackground" + ColorNameFiller + "1", GetColors(73));

            AddColorsToDictionary("HoverHighlight" + ColorNameFiller, GetColors(73, 55, 192));
            AddColorsToDictionary("SelectedHighlight" + ColorNameFiller, GetColors(76, 57));

            AddColorsToDictionary("CheckBoxTick" + ColorNameFiller, GetColors(17, 37));

            /*AddColorsToDictionary("ButtonPressedBorder" + ColorNameFiller, GetColors(70));

            AddColorsToDictionary("ButtonPressedBackground" + ColorNameFiller + "0", GetColors(95));
            AddColorsToDictionary("ButtonPressedBackground" + ColorNameFiller + "1", GetColors(88));

            AddColorsToDictionary("ButtonPressedSecondaryBackground" + ColorNameFiller + "0", GetColors(92));
            AddColorsToDictionary("ButtonPressedSecondaryBackground" + ColorNameFiller + "1", GetColors(84));

            AddColorsToDictionary("CardListViewSelectedBackgroundBackground" + ColorNameFiller + "0", GetColors(95));
            AddColorsToDictionary("CardListViewSelectedBackgroundBackground" + ColorNameFiller + "1", GetColors(84));

            AddColorsToDictionary("HoverHighlight" + ColorNameFiller, GetColors(84, 64, 0xC0));
            AddColorsToDictionary("SelectedHighlight" + ColorNameFiller, GetColors(86, 66));

            AddColorsToDictionary("CheckBoxTick" + ColorNameFiller, GetColors(34, 74));*/
            //_dictionary.
            return _dictionary;
        }
    }
}
