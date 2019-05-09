using Start9.UI.Wpf;
using Start9.UI.Wpf.Behaviors;
using Start9.UI.Wpf.Windows;
using Start9.Wpf.Styles.Shale;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrontEndTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DecoratableWindow
    {
        enum TestEnum
        {
            OK,
            No_u,
            Cancel
        }

        ShaleAccent _accent = ShaleAccents.Blue; //new ShaleAccent(183, 28);
        //ResourceDictionary _dictionary = null;

        public MainWindow()
        {
            InitializeComponent();
            Timer timer = new Timer(5000);
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (IsVisible)
                        Hide();
                    else
                        Show();
                }));
            };
            //timer.Start();

            /*Start9.UI.Wpf.Windows.MessageBox<TestEnum>.Show("Body Text", "Caption", new ResourceDictionary()
            {
                Source = new Uri("/Start9.UI.Wpf;component/Themes/Plex.xaml", UriKind.RelativeOrAbsolute)
            });*/
            TimeDurationTextBox.Text = ScrollAnimationBehavior.GetTimeDuration(SmoothScrollTestListView).ToString();

            ShaleHueSlider.Value = _accent.Hue;
            ShaleSaturationSlider.Value = _accent.Saturation;

            CalculateColor();

            ShaleHueSlider.ValueChanged += ShaleSliders_ValueChanged;
            ShaleSaturationSlider.ValueChanged += ShaleSliders_ValueChanged;
            /*ShaleHueSlider.PreviewMouseLeftButtonUp += Sliders_PreviewMouseLeftButtonUp;
            ShaleSaturationSlider.PreviewMouseLeftButtonUp += Sliders_PreviewMouseLeftButtonUp;*/
        }

        private void Sliders_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Mouse up");
            CalculateColor();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TimeDurationTextBox.TextChanged += TimeDurationTextBox_TextChanged;
            AnimationTypeComboBox.SelectionChanged += ScrollAnimationComboBox_SelectionChanged;
            EasingModeComboBox.SelectionChanged += ScrollAnimationComboBox_SelectionChanged;
            EnableSmoothScrollingCheckBox.Checked += EnableSmoothScrollingCheckBox_Checked;
            EnableSmoothScrollingCheckBox.Unchecked += EnableSmoothScrollingCheckBox_Checked;
            LightsToggleSwitch.Checked += LightsToggleSwitch_Checked;
            LightsToggleSwitch.Unchecked += LightsToggleSwitch_Unchecked;
        }

        public void CalculateColor()
        {
            for (int i = 2; i < Application.Current.Resources.MergedDictionaries.Count; i++)
            {
                Application.Current.Resources.MergedDictionaries.RemoveAt(0);
            }
            //Application.Current.Resources.MergedDictionaries[1].MergedDictionaries.Clear();
            Debug.WriteLine("Values: " + ShaleHueSlider.Value + ", " + ShaleSaturationSlider.Value);
            _accent = new ShaleAccent(ShaleHueSlider.Value, ShaleSaturationSlider.Value);

            var dictionary = _accent.GetDictionary();
            Debug.WriteLine(dictionary["SelectedHighlightLightColor"]);
            Application.Current.Resources.MergedDictionaries.Insert(0, dictionary);
            var lightDictionary = Application.Current.Resources.MergedDictionaries[2];
            Uri source = lightDictionary.Source;
            //remove and recreate light/dark ResourceDictionary (yes this is necessary...for some reason...
            Application.Current.Resources.MergedDictionaries.Remove(lightDictionary);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = source //new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Colors/BaseDark.xaml")
            });
        }

        private void ToggleGlassToggleButton_Click(object sender, RoutedEventArgs e)
        {
            /*if (ToggleGlassToggleButton.IsChecked == true)
                EnableGlass = true;
            else
                EnableGlass = false;*/
            //Debug.WriteLine("Glass toggled:" + EnableGlass.ToString());

        }

        private void CycleCompositionStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (CompositionState == WindowCompositionState.Alpha)
                CompositionState = WindowCompositionState.Glass;
            else if (CompositionState == WindowCompositionState.Glass)
                CompositionState = WindowCompositionState.Accent;
            else if (CompositionState == WindowCompositionState.Accent)
                CompositionState = WindowCompositionState.Acrylic;
            else
                CompositionState = WindowCompositionState.Alpha;

            CurrentCompositionStateTextBlock.Text = CompositionState.ToString();
        }

        private void ThemeToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            /*Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("/Start9.UI.Wpf;component/Themes/Colors/LightPlexBlue.xaml", UriKind.RelativeOrAbsolute)
            });*/
        }

        private void ThemeToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            /*Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("/Start9.UI.Wpf;component/Themes/Colors/DarkPlexBlue.xaml", UriKind.RelativeOrAbsolute)
            });*/
        }

        private void ScrollAnimationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EasingFunctionBase ease = null;
            if (AnimationTypeComboBox.SelectedIndex != 0)
            {
                Type easeType = /*(*/AnimationTypeComboBox.SelectedItem/* as ComboBoxItem).Content*/.GetType();
                ease = (EasingFunctionBase)Activator.CreateInstance(easeType);
                ease.EasingMode = (EasingMode)Enum.Parse(typeof(EasingMode), ((ComboBoxItem)EasingModeComboBox.SelectedItem).Content.ToString());
            }

            ScrollAnimationBehavior.SetEasingFunction(SmoothScrollTestListView, ease);

            Debug.WriteLine("ScrollAnimationComboBox_SelectionChanged");
        }

        private void TimeDurationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TimeSpan.TryParse(TimeDurationTextBox.Text, out TimeSpan resultTime))
            {
                ScrollAnimationBehavior.SetTimeDuration(SmoothScrollTestListView, resultTime);
                TimeDurationBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
                Debug.WriteLine("TimeDurationTextBox_TextChanged: " + true);
            }
            else
            {
                TimeDurationBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                Debug.WriteLine("TimeDurationTextBox_TextChanged: " + false);
            }
        }

        private void EnableSmoothScrollingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool enable = EnableSmoothScrollingCheckBox.IsChecked == true;
            ScrollAnimationBehavior.SetIsEnabled(SmoothScrollTestListView, enable);
            Debug.WriteLine("EnableSmoothScrollingCheckBox_Checked: " + enable);
        }


        private void ShaleSliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CalculateColor();
        }

        bool reset = false;
        private void ResetColouresButton_Click(object sender, RoutedEventArgs e)
        {
            /*Debug.WriteLine("SelectedHighlightLightColor: " + Application.Current.Resources["SelectedHighlightLightColor"]);
            Debug.WriteLine("SelectedHighlightBrush: " + Application.Current.Resources["SelectedHighlightBrush"]);
            Debug.WriteLine("SelectedHighlightBrush.Color: " + ((SolidColorBrush)Application.Current.Resources["SelectedHighlightBrush"]).Color);
            Debug.WriteLine("Dictionaries count: " + Application.Current.Resources.MergedDictionaries.Count);*/
            if (reset)
            {
                _accent = ShaleAccents.Beige;
                reset = false;
            }
            else
            {
                _accent = ShaleAccents.Blue;
                reset = true;
            }

            ShaleHueSlider.Value = _accent.Hue;
            ShaleSaturationSlider.Value = _accent.Saturation;
            CalculateColor();
        }

        private void LightsToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries[2].Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Colors/BaseLight.xaml", UriKind.Relative);
        }

        private void LightsToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries[2].Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Colors/BaseDark.xaml", UriKind.Relative);
        }
    }
}
