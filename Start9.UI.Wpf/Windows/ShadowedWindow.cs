using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Start9.UI.Wpf.Statics;

namespace Start9.UI.Wpf.Windows
{
    public partial class ShadowedWindow : CompositingWindow
    {
        public Thickness ShadowOffsetThickness
        {
            get => (Thickness)GetValue(ShadowOffsetThicknessProperty);
            set => SetValue(ShadowOffsetThicknessProperty, value);
        }

        public static readonly DependencyProperty ShadowOffsetThicknessProperty =
            DependencyProperty.Register("ShadowOffsetThickness", typeof(Thickness), typeof(ShadowedWindow), new PropertyMetadata(new Thickness(50)));

        public Style ShadowStyle
        {
            get => (Style)GetValue(ShadowStyleProperty);
            set => SetValue(ShadowStyleProperty, value);
        }

        public static readonly DependencyProperty ShadowStyleProperty =
            DependencyProperty.Register("ShadowStyle", typeof(Style), typeof(ShadowedWindow), new PropertyMetadata());

        readonly Window _shadowWindow;
        readonly TimeSpan _noDuration = TimeSpan.FromMilliseconds(0);

        static ShadowedWindow()
        {
            IgnorePeekProperty.OverrideMetadata(typeof(ShadowedWindow), new FrameworkPropertyMetadata(false, ShadowedWindow.OnIgnorePeekChangedCallback));
        }

        WindowInteropHelper _helper = null;

        public ShadowedWindow()
        {
            _shadowWindow = new Window()
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                ShowInTaskbar = false,
                ShowActivated = false,
                Tag = this
            };

            _shadowWindow.SourceInitialized += (sneder, args) =>
            {
                _helper = new WindowInteropHelper(_shadowWindow);
                NativeMethods.SetWindowLong(_helper.Handle, NativeMethods.GwlExstyle, (Int32)(NativeMethods.GetWindowLong(_helper.Handle, NativeMethods.GwlExstyle)) | NativeMethods.WsExToolwindow | NativeMethods.WsExTransparent); ////WinApi

                SyncShadowToWindow();

                if (!IsWindowVisible)
                    _shadowWindow.Hide();
            };

            Binding shadowStyleBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("ShadowStyle"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.StyleProperty, shadowStyleBinding);

            /*Binding shadowTopmostBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("IsActive"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.TopmostProperty, shadowTopmostBinding);*/

            Binding shadowVisibilityBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("Visibility"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.VisibilityProperty, shadowVisibilityBinding);

            Binding shadowRenderTransformBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("RenderTransform"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.RenderTransformProperty, shadowRenderTransformBinding);

            Binding shadowOpacityBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("Opacity"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.OpacityProperty, shadowOpacityBinding);

            Binding shadowIsFocusedBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("IsActive"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.IsManipulationEnabledProperty, shadowIsFocusedBinding);

            Binding shadowIsEnabledBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("IsWindowVisible"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.IsEnabledProperty, shadowIsEnabledBinding);

            Binding shadowIsHitTestVisibleBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("WindowState"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = new WindowStateIsMaximizedToBoolConverter()
            };
            BindingOperations.SetBinding(_shadowWindow, Window.IsHitTestVisibleProperty, shadowIsHitTestVisibleBinding);

            Binding shadowBorderThicknessBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("ShadowOffsetThickness"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.BorderThicknessProperty, shadowBorderThicknessBinding);

            StateChanged += (sneder, args) =>
            {
                if (WindowState == WindowState.Normal)
                    _shadowWindow.Show();
                else
                    _shadowWindow.Hide();
            };

            Initialized += (sneder, args) =>
            {
                if (WindowState == WindowState.Normal && IsVisible)
                    _shadowWindow.Show();
                //UpdateDefaultStyle();
                //Style = (Style)(FindResource(typeof(DecoratableWindow)));
                //UpdateDefaultStyle();
                //Style = (Style)Resources[typeof(DecoratableWindow)];
            };

            Activated += (sneder, args) =>
            {
                _shadowWindow.Topmost = true;
                _shadowWindow.Topmost = false;
            };

            Closed += (sneder, args) =>
            {
                _shadowWindow.Close();
            };

            SizeChanged += (sneder, args) =>
            {
                SyncShadowToWindow();
            };
        }

        new internal static void OnIgnorePeekChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ShadowedWindow).SetPeekState();
        }

        internal override void SetPeekState()
        {
            base.SetPeekState();

            if (NativeMethods.DwmIsCompositionEnabled())
            {
                int peekValue = 0;

                if (IgnorePeek)
                    peekValue = 1;

                NativeMethods.DwmSetWindowAttribute(new WindowInteropHelper(_shadowWindow).EnsureHandle(), 12, ref peekValue, sizeof(int));
            }
        }

        public void SyncShadowToWindow()
        {
            //_shadowWindow.Left = Left - ShadowOffsetThickness.Left;
            //_shadowWindow.Top = Top - ShadowOffsetThickness.Top;
            if (_helper != null)
                NativeMethods.SetWindowPos(_helper.Handle, IntPtr.Zero, (int)(Left - ShadowOffsetThickness.Left), (int)(Top - ShadowOffsetThickness.Top), (int)(ActualWidth + ShadowOffsetThickness.Left + ShadowOffsetThickness.Right), (int)(ActualHeight + ShadowOffsetThickness.Top + ShadowOffsetThickness.Bottom), 0x0004 | 0x0010);
            /*DoubleAnimation leftAnimation = new DoubleAnimation()
            {
                From = Left,
                To = Left - ShadowOffsetThickness.Left,
                Duration = _noDuration
            };
            DoubleAnimation topAnimation = new DoubleAnimation()
            {
                From = Top,
                To = Top - ShadowOffsetThickness.Top,
                Duration = _noDuration
            };
            _shadowWindow.BeginAnimation(Window.LeftProperty, leftAnimation);
            _shadowWindow.BeginAnimation(Window.TopProperty, topAnimation);*/
        }

        /*protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }*/

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            SyncShadowToWindow();
        }
    }
}
