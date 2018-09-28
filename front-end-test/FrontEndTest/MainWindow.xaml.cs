using Start9.UI.Wpf;
using Start9.UI.Wpf.Windows;
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
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var win = new Window();
            win.Show();
            Timer timer = new Timer(5000);
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (win.IsVisible)
                        win.Hide();
                    else
                        win.Show();
                }));
            };
            timer.Start();
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
            Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("/Start9.UI.Wpf;component/Themes/Colors/LightPlexBlue.xaml", UriKind.RelativeOrAbsolute)
            });
        }

        private void ThemeToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("/Start9.UI.Wpf;component/Themes/Colors/DarkPlexBlue.xaml", UriKind.RelativeOrAbsolute)
            });
        }
    }
}
