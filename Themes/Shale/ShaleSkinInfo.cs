using Start9.UI.Wpf.Skinning;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Start9.Wpf.Styles.Shale
{
    public class ShaleSkinInfo : ISkinInfo
    {
        public bool HaveSettingsChanged = false;

        ShaleSettingsPage _page = null;

        public bool UseLightTheme = true;

        public double Hue = ShaleAccents.Sky.Hue;
        public double Saturation = ShaleAccents.Sky.Saturation;

        public ShaleAccent Accent = new ShaleAccent(ShaleAccents.Sky.Hue, ShaleAccents.Sky.Saturation);

        public virtual bool GetHaveSettingsChanged()
        {
            return HaveSettingsChanged;
        }

        public virtual Page GetSettingsPage()
        {
            if (_page == null)
                _page = new ShaleSettingsPage(this);

            return _page;
        }

        public virtual ResourceDictionary OnApplySkinSettings()
        {
            ResourceDictionary dictionary = new ResourceDictionary()
            {
                Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Shale.xaml", UriKind.Relative)
            };

            if (UseLightTheme)
                dictionary.MergedDictionaries[0].Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Colors/BaseLight.xaml", UriKind.Relative);
            else
                dictionary.MergedDictionaries[0].Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Colors/BaseDark.xaml", UriKind.Relative);

            Accent.Hue = Hue;
            Accent.Saturation = Saturation;

            dictionary.MergedDictionaries[1] = Accent.Dictionary;

            HaveSettingsChanged = false;
            return dictionary;
        }

        string ISkinInfo.SkinName
        {
            get => "Shale";
        }
    }
}
