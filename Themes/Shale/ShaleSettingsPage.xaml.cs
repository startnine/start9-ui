using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Start9.Wpf.Styles.Shale
{
    /// <summary>
    /// Interaction logic for ShaleSettingsPage.xaml
    /// </summary>
    public partial class ShaleSettingsPage : Page
    {
        ShaleSkinInfo _shaleSkinInfo = null;
        public ShaleSettingsPage(ShaleSkinInfo shaleSkinInfo)
        {
            _shaleSkinInfo = shaleSkinInfo;
            InitializeComponent();

            AccentPresetsListView.ItemsSource = new ShaleAccent[]
            {
                ShaleAccents.Sky,
                ShaleAccents.Ocean,
                ShaleAccents.Sand,
                ShaleAccents.Coral,
                ShaleAccents.Palm,
                ShaleAccents.Sunlight
            };
        }

        private void LightsToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            _shaleSkinInfo.HaveSettingsChanged = true;
            _shaleSkinInfo.UseLightTheme = true;
        }

        private void LightsToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            _shaleSkinInfo.HaveSettingsChanged = true;
            _shaleSkinInfo.UseLightTheme = false;
        }

        private void HueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _shaleSkinInfo.HaveSettingsChanged = true;
            _shaleSkinInfo.Hue = e.NewValue;
        }

        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _shaleSkinInfo.HaveSettingsChanged = true;
            _shaleSkinInfo.Saturation = e.NewValue;
        }

        private void AccentPresetsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is ShaleAccent acc)
            {
                HueSlider.Value = acc.Hue;
                SaturationSlider.Value = acc.Saturation;
            }
        }
    }
}
