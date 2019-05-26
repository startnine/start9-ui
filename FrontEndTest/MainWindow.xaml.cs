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
using MessageBox = Start9.UI.Wpf.Windows.MessageBox;

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


            TimeDurationTextBox.Text = ScrollAnimationBehavior.GetTimeDuration(SmoothScrollTestListView).ToString();

            ShaleHueSlider.Value = _accent.Hue;
            ShaleSaturationSlider.Value = _accent.Saturation;

            Application.Current.Resources.MergedDictionaries.Add(_accent.Dictionary);

            ShaleHueSlider.ValueChanged += ShaleSliders_ValueChanged;
            ShaleSaturationSlider.ValueChanged += ShaleSliders_ValueChanged;
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

            ColouresPresetsGrid.Children.Add(GetColoureButton(ShaleAccents.Blue));
            ColouresPresetsGrid.Children.Add(GetColoureButton(ShaleAccents.Beige));

            SkinsComboBox.SelectionChanged += SkinsComboBox_SelectionChanged;
        }

        private Button GetColoureButton(ShaleAccent accent)
        {
            Debug.WriteLine("PRESET COLOUR: " + accent.ToColor());

            Button button = new Button()
            {
                Background = new SolidColorBrush(accent.ToColor())
            };

            button.SetBinding(Button.HeightProperty, new Binding()
            {
                Source = button,
                Path = new PropertyPath("ActualWidth"),
                FallbackValue = 10.0
            });

            button.Click += (sneder, args) =>
            {
                ShaleHueSlider.Value = accent.Hue;
                ShaleSaturationSlider.Value = accent.Saturation;
            };

            return button;
        }

        public void CalculateColor()
        {
            _accent.Hue = ShaleHueSlider.Value;
            _accent.Saturation = ShaleSaturationSlider.Value;
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

            //CurrentCompositionStateTextBlock.Text = CompositionState.ToString();
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
            if (SkinsComboBox.SelectedIndex == 0)
                CalculateColor();
        }

        private void LightsToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (SkinsComboBox.SelectedIndex == 0)
                Application.Current.Resources.MergedDictionaries[1].Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Colors/BaseLight.xaml", UriKind.Relative);
            else if (SkinsComboBox.SelectedIndex == 1)
                Application.Current.Resources.MergedDictionaries[1].Source = new Uri("/Start9.Wpf.Styles.Plex;component/Themes/Colors/LightBlue.xaml", UriKind.Relative);
        }

        private void LightsToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SkinsComboBox.SelectedIndex == 0)
                Application.Current.Resources.MergedDictionaries[1].Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Colors/BaseDark.xaml", UriKind.Relative);
            else if (SkinsComboBox.SelectedIndex == 1)
                Application.Current.Resources.MergedDictionaries[1].Source = new Uri("/Start9.Wpf.Styles.Plex;component/Themes/Colors/DarkBlue.xaml", UriKind.Relative);
        }

        private void SkinsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SkinsComboBox.SelectedIndex == 0)
            {
                Application.Current.Resources.MergedDictionaries[0].Source = new Uri("/Start9.Wpf.Styles.Shale;component/Themes/Shale.xaml", UriKind.Relative);
                TitlebarHeight = 61;
            }
            else if (SkinsComboBox.SelectedIndex == 1)
            {
                Application.Current.Resources.MergedDictionaries[0].Source = new Uri("/Start9.Wpf.Styles.Plex;component/Themes/Plex.xaml", UriKind.Relative);
                ClearValue(TitlebarHeightProperty);
            }

            if (LightsToggleSwitch.IsChecked.Value)
                LightsToggleSwitch_Checked(LightsToggleSwitch, null);
            else
                LightsToggleSwitch_Unchecked(LightsToggleSwitch, null);
        }

        private void MessageBoxTestButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxActionSets.IgnoreRetryAbortButtons b = (MessageBoxActionSets.IgnoreRetryAbortButtons)MessageBox<MessageBoxActionSets.IgnoreRetryAbortActionSet>.Show("Here, have some actions to choose from.", "This is a MessageBox", (Rectangle)Resources["SampleIcon"]);

            MessageBox.Show("Result of previous MessageBox: " + b.ToString(), "haha yes");

            MessageBox<SampleActionSet>.Show("*laughs in custom MessageBox actions*", "I CAN DO ANYTHING", (Rectangle)Resources["SampleIcon"]);
        }
    }

    public enum SampleButtons
    {
        Yeet,
        NoU
    }

    public class SampleActionSet : IMessageBoxActionSet
    {
        public string GetDisplayName(object value)
        {
            switch ((SampleButtons)value)
            {
                case SampleButtons.Yeet:
                    return "Yeet";
                case SampleButtons.NoU:
                    return "no u";
                default:
                    return string.Empty;
            }
            /*if ((SampleButtons)value == SampleButtons.Yeet)
                return "Yeet";
            else if ((SampleButtons)value == SampleButtons.Yeet)
                return value.ToString();*/
        }

        public object[] GetValues()
        {
            object[] objects = new object[Enum.GetNames(typeof(SampleButtons)).Count()];
            Enum.GetValues(typeof(SampleButtons)).CopyTo(objects, 0);
            return objects;
        }
    }
}
