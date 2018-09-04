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
            timer.Start();
            //Loaded += MainWindow_Loaded;
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
    }
}
