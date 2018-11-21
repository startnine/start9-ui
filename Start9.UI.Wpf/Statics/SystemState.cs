using System;
using System.Windows;

namespace Start9.UI.Wpf.Statics
{
    public class SystemState : DependencyObject
    {
        internal static bool FakeCompositionOff
        {
            get => System.IO.File.Exists(Environment.ExpandEnvironmentVariables(@"%appdata%\Start9\fakedwmoff.txt"));
        }

        public bool IsCompositionEnabled
        {
            get => (bool)GetValue(IsCompositionEnabledProperty);
            set => SetValue(IsCompositionEnabledProperty, value);
        }

        public static readonly DependencyProperty IsCompositionEnabledProperty =
            DependencyProperty.Register("IsCompositionEnabled", typeof(bool), typeof(SystemState), new UIPropertyMetadata(false));

        /*public static bool IsCompositionEnabled
        {
            get
            {
                if (SystemState.FakeCompositionOff)
                    return false;
                else
                    return NativeMethods.DwmIsCompositionEnabled();
            }
            set { }
        }*/

        public static SystemState Instance = new SystemState();

        private SystemState()
        {
            if (SystemState.FakeCompositionOff)
                IsCompositionEnabled = false;
            else
                IsCompositionEnabled = NativeMethods.DwmIsCompositionEnabled();
        }
    }
}
