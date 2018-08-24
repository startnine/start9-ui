using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;

namespace Start9.UI.Wpf
{
    [TemplatePart(Name = PartTitlebar, Type = typeof(Grid))]
    [TemplatePart(Name = PartMinimizeButton, Type = typeof(Button))]
    [TemplatePart(Name = PartMaximizeButton, Type = typeof(Button))]
    [TemplatePart(Name = PartRestoreButton, Type = typeof(Button))]
    [TemplatePart(Name = PartCloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PartThumbBottom, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbTop, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbBottomRightCorner, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbTopRightCorner, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbTopLeftCorner, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbBottomLeftCorner, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbRight, Type = typeof(Thumb))]
    [TemplatePart(Name = PartThumbLeft, Type = typeof(Thumb))]

    [ContentProperty("Content")]
    public partial class DecoratableWindow : Window
    {
        const String PartTitlebar = "PART_Titlebar";
        const String PartMinimizeButton = "PART_MinimizeButton";
        const String PartMaximizeButton = "PART_MaximizeButton";
        const String PartRestoreButton = "PART_RestoreButton";
        const String PartCloseButton = "PART_CloseButton";
        const String PartThumbBottom = "PART_ThumbBottom";
        const String PartThumbTop = "PART_ThumbTop";
        const String PartThumbBottomRightCorner = "PART_ThumbBottomRightCorner";
        const String PartResizeGrip = "PART_ResizeGrip";
        const String PartThumbTopRightCorner = "PART_ThumbTopRightCorner";
        const String PartThumbTopLeftCorner = "PART_ThumbTopLeftCorner";
        const String PartThumbBottomLeftCorner = "PART_ThumbBottomLeftCorner";
        const String PartThumbRight = "PART_ThumbRight";
        const String PartThumbLeft = "PART_ThumbLeft";

        Button _closeButton;
        Button _maxButton;
        Button _minButton;
        Button _restButton;

        Thumb _thumbBottom;
        Thumb _thumbBottomLeftCorner;
        Thumb _thumbBottomRightCorner;
        Thumb _resizeGrip;
        Thumb _thumbLeft;
        Thumb _thumbRight;
        Thumb _thumbTop;
        Thumb _thumbTopLeftCorner;
        Thumb _thumbTopRightCorner;

        Grid _titlebar;

        IntPtr _handle;
        NativeMethods.DWM_BLURBEHIND _blurInfo = new NativeMethods.DWM_BLURBEHIND();
        NativeMethods.DWM_BLURBEHIND _unblurInfo = new NativeMethods.DWM_BLURBEHIND();
        /*{
            fEnable = false
        };*/

        new public WindowStyle WindowStyle
        {
            get => (WindowStyle)GetValue(WindowStyleProperty);
            set => SetValue(WindowStyleProperty, value);
        }

        new public static readonly DependencyProperty WindowStyleProperty =
            DependencyProperty.Register("WindowStyle", typeof(WindowStyle), typeof(DecoratableWindow), new PropertyMetadata(WindowStyle.SingleBorderWindow));

        public Thickness ShadowOffsetThickness
        {
            get => (Thickness)GetValue(ShadowOffsetThicknessProperty);
            set => SetValue(ShadowOffsetThicknessProperty, value);
        }

        public static readonly DependencyProperty ShadowOffsetThicknessProperty =
            DependencyProperty.Register("ShadowOffsetThickness", typeof(Thickness), typeof(DecoratableWindow), new PropertyMetadata(new Thickness(50)));

        public enum WindowCompositionState
        {
            Alpha,
            Glass,
            Accent,
            Acrylic
        }

        public WindowCompositionState CompositionState
        {
            get => (WindowCompositionState)GetValue(CompositionStateProperty);
            set => SetValue(CompositionStateProperty, value);
        }

        public static readonly DependencyProperty CompositionStateProperty =
            DependencyProperty.Register("CompositionState", typeof(WindowCompositionState), typeof(DecoratableWindow), new FrameworkPropertyMetadata(WindowCompositionState.Alpha, OnCompositionStatePropertyChangedCallback));

        static void OnCompositionStatePropertyChangedCallback(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("CompositionState: " + (sender as DecoratableWindow).CompositionState.ToString());
            (sender as DecoratableWindow).SetCompositionState((WindowCompositionState)(e.NewValue));
        }

        public Style ShadowStyle
        {
            get => (Style)GetValue(ShadowStyleProperty);
            set => SetValue(ShadowStyleProperty, value);
        }

        public static readonly DependencyProperty ShadowStyleProperty =
            DependencyProperty.Register("ShadowStyle", typeof(Style), typeof(DecoratableWindow), new PropertyMetadata());

        /*public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, value);
        }

        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(DecoratableWindow), new PropertyMetadata((double)1));

        public Visibility ShadowVisibility
        {
            get => (Visibility)GetValue(ShadowVisibilityProperty);
            set => SetValue(ShadowVisibilityProperty, value);
        }

        public static readonly DependencyProperty ShadowVisibilityProperty =
            DependencyProperty.Register("ShadowVisibility", typeof(Visibility), typeof(DecoratableWindow), new PropertyMetadata(Visibility.Visible));*/

        readonly Window _shadowWindow;

        public object TitleBarContent
        {
            get => GetValue(TitleBarContentProperty);
            set => SetValue(TitleBarContentProperty, value);
        }

        public static readonly DependencyProperty TitleBarContentProperty =
                    DependencyProperty.RegisterAttached("TitleBarContent", typeof(object), typeof(DecoratableWindow),
                        new PropertyMetadata(null));

        public object FullWindowContent
        {
            get => GetValue(FullWindowContentProperty);
            set => SetValue(FullWindowContentProperty, value);
        }

        public static readonly DependencyProperty FullWindowContentProperty =
                    DependencyProperty.RegisterAttached("FullWindowContent", typeof(object), typeof(DecoratableWindow),
                        new PropertyMetadata(null));

        /*static DecoratableWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DecoratableWindow), new FrameworkPropertyMetadata("{x:Type apictrl:DecoratableWindow}"));
        }*/

        public DecoratableWindow()
        {
            DefaultStyleKey = typeof(DecoratableWindow);
            base.WindowStyle = WindowStyle.None;
            base.AllowsTransparency = true;

            _shadowWindow = new Window()
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                ShowInTaskbar = false
            };

            _shadowWindow.SourceInitialized += (sneder, args) =>
            {
                var helper = new WindowInteropHelper(_shadowWindow);
                NativeMethods.SetWindowLong(helper.Handle, NativeMethods.GwlExstyle, (Int32)(NativeMethods.GetWindowLong(helper.Handle, NativeMethods.GwlExstyle)) | NativeMethods.WsExToolwindow | NativeMethods.WsExTransparent); ////WinApi
            };

            Binding shadowStyleBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("ShadowStyle"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.StyleProperty, shadowStyleBinding);

            Binding shadowTopmostBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("IsActive"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.TopmostProperty, shadowTopmostBinding);

            Binding shadowIsEnabledBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("IsActive"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(_shadowWindow, Window.IsEnabledProperty, shadowTopmostBinding);

            Binding shadowIsHitTestVisibleBinding = new Binding()
            {
                Source = this,
                Path = new PropertyPath("WindowState"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = new WindowStateIsMaximizedToBoolConverter()
            };
            BindingOperations.SetBinding(_shadowWindow, Window.IsHitTestVisibleProperty, shadowIsHitTestVisibleBinding);

            _blurInfo.dwFlags = NativeMethods.DWM_BB.Enable | NativeMethods.DWM_BB.BlurRegion | NativeMethods.DWM_BB.TransitionMaximized;
            _blurInfo.fEnable = true;
            //_blurInfo.hRgnBlur = IntPtr.Zero;
            _blurInfo.fTransitionOnMaximized = true;

            _unblurInfo.dwFlags = NativeMethods.DWM_BB.Enable | NativeMethods.DWM_BB.BlurRegion | NativeMethods.DWM_BB.TransitionMaximized;
            _unblurInfo.fEnable = false;
            _unblurInfo.hRgnBlur = IntPtr.Zero;
            _unblurInfo.fTransitionOnMaximized = true;

            _handle = new WindowInteropHelper(this).EnsureHandle();

            Closed += (sneder, args) =>
            {
                _shadowWindow.Close();
            };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            SetCompositionState(CompositionState);
        }

        public void SyncShadowToWindow()
        {
            _shadowWindow.Left = Left - ShadowOffsetThickness.Left;
            _shadowWindow.Top = Top - ShadowOffsetThickness.Top;
        }

        public void SyncShadowToWindowSize()
        {
            _shadowWindow.Width = Width + ShadowOffsetThickness.Left + ShadowOffsetThickness.Right;
            _shadowWindow.Height = Height + ShadowOffsetThickness.Top + ShadowOffsetThickness.Bottom;
            if (CompositionState != WindowCompositionState.Alpha)
                SetCompositionState();
        }

        void SetCompositionState()
        {
            if (CompositionState != WindowCompositionState.Alpha)
                SetCompositionState(CompositionState);
        }

        void ClearCompositionState()
        {
            NativeMethods.DwmEnableBlurBehindWindow(_handle, ref _unblurInfo);

            if (Environment.OSVersion.Version >= new Version(10, 0, 17134, 0))
            {
                var accent = new NativeMethods.AccentPolicy();
                var accentStructSize = Marshal.SizeOf(accent);
                accent.GradientColor = (0x990000 << 24) | (0x990000 /* BGR */ & 0xFFFFFF);
                accent.AccentState = NativeMethods.AccentState.ACCENT_DISABLED;

                var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new NativeMethods.WindowCompositionAttributeData();
                data.Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY;
                data.SizeOfData = accentStructSize;
                data.Data = accentPtr;

                NativeMethods.SetWindowCompositionAttribute(_handle, ref data);

                Marshal.FreeHGlobal(accentPtr);
            }
            else
            {
                if (Environment.OSVersion.Version >= new Version(6, 2, 9200, 0))
                {
                    var accent = new NativeMethods.AccentPolicy();
                    var accentStructSize = Marshal.SizeOf(accent);
                    accent.AccentState = NativeMethods.AccentState.ACCENT_DISABLED;
                    accent.AccentFlags = 0;
                    accent.GradientColor = 0;
                    accent.AnimationId = 0;

                    var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                    Marshal.StructureToPtr(accent, accentPtr, false);

                    var data = new NativeMethods.WindowCompositionAttributeData
                    {
                        Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                        SizeOfData = accentStructSize,
                        Data = accentPtr
                    };

                    NativeMethods.SetWindowCompositionAttribute(_handle, ref data);

                    Marshal.FreeHGlobal(accentPtr);
                }
            }
        }

        void SetCompositionState(WindowCompositionState targetState)
        {
            if (NativeMethods.DwmIsCompositionEnabled())
            {
                ClearCompositionState();

                if (targetState == WindowCompositionState.Glass)
                {
                    if (new Version(10, 0, 16299, 0) <= Environment.OSVersion.Version)
                    {
                        var accent = new NativeMethods.AccentPolicy();
                        var accentStructSize = Marshal.SizeOf(accent);
                        accent.AccentState = NativeMethods.AccentState.ACCENT_ENABLE_BLURBEHIND;

                        var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                        Marshal.StructureToPtr(accent, accentPtr, false);

                        var data = new NativeMethods.WindowCompositionAttributeData();
                        data.Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY;
                        data.SizeOfData = accentStructSize;
                        data.Data = accentPtr;

                        NativeMethods.SetWindowCompositionAttribute(_handle, ref data);

                        Marshal.FreeHGlobal(accentPtr);
                    }
                    else if (Environment.OSVersion.Version.Major >= 10)
                    {
                        var accent = new NativeMethods.AccentPolicy();
                        var accentStructSize = Marshal.SizeOf(accent);
                        accent.GradientColor = (0x990000 << 24) | (0x990000 /* BGR */ & 0xFFFFFF);
                        accent.AccentState = NativeMethods.AccentState.ACCENT_ENABLE_BLURBEHIND;

                        var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                        Marshal.StructureToPtr(accent, accentPtr, false);

                        var data = new NativeMethods.WindowCompositionAttributeData();
                        data.Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY;
                        data.SizeOfData = accentStructSize;
                        data.Data = accentPtr;

                        NativeMethods.SetWindowCompositionAttribute(_handle, ref data);

                        Marshal.FreeHGlobal(accentPtr);
                    }
                    else //TODO: Figure something out for unmodified Windows 8.x
                    {
                        IntPtr windowRegion = IntPtr.Zero;
                        NativeMethods.RECT rect;
                        if (NativeMethods.GetWindowRect(_handle, out rect))
                        {
                            windowRegion = NativeMethods.CreateRectRgn(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);
                            _blurInfo.hRgnBlur = windowRegion;
                            NativeMethods.DwmEnableBlurBehindWindow(_handle, ref _blurInfo);
                        }
                    }
                }
                else if (targetState == WindowCompositionState.Accent)
                {
                    if (Environment.OSVersion.Version >= new Version(10, 0, 17134, 0))
                    {
                        var accent = new NativeMethods.AccentPolicy();
                        var accentStructSize = Marshal.SizeOf(accent);
                        accent.GradientColor = (0x990000 << 24) | (0x990000 /* BGR */ & 0xFFFFFF);
                        accent.AccentState = NativeMethods.AccentState.ACCENT_ENABLE_GRADIENT;

                        var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                        Marshal.StructureToPtr(accent, accentPtr, false);

                        var data = new NativeMethods.WindowCompositionAttributeData();
                        data.Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY;
                        data.SizeOfData = accentStructSize;
                        data.Data = accentPtr;

                        NativeMethods.SetWindowCompositionAttribute(_handle, ref data);

                        Marshal.FreeHGlobal(accentPtr);
                    }
                    else if (Environment.OSVersion.Version >= new Version(6, 2, 9200, 0))
                    {
                        var accent = new NativeMethods.AccentPolicy();
                        var accentStructSize = Marshal.SizeOf(accent);
                        accent.AccentState = NativeMethods.AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT;
                        accent.AccentFlags = 0;// 0x20 | 0x40 | 0x80| 0x100;
                        accent.GradientColor = 0;
                        accent.AnimationId = 1;

                        var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                        Marshal.StructureToPtr(accent, accentPtr, false);

                        var data = new NativeMethods.WindowCompositionAttributeData
                        {
                            Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                            SizeOfData = accentStructSize,
                            Data = accentPtr
                        };

                        NativeMethods.SetWindowCompositionAttribute(_handle, ref data);

                        Marshal.FreeHGlobal(accentPtr);
                    }
                    else //TODO: Figure something out for Windows 7
                    {

                    }
                }
                else if (targetState == WindowCompositionState.Acrylic)
                {
                    if (Environment.OSVersion.Version >= new Version(10, 0, 17134, 0))
                    {
                        var accent = new NativeMethods.AccentPolicy();
                        var accentStructSize = Marshal.SizeOf(accent);
                        accent.GradientColor = (0x990000 << 24) | (0x990000 /* BGR */ & 0xFFFFFF);
                        accent.AccentState = NativeMethods.AccentState.ACCENT_ENABLE_BLURBEHIND;

                        var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                        Marshal.StructureToPtr(accent, accentPtr, false);

                        var data = new NativeMethods.WindowCompositionAttributeData();
                        data.Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY;
                        data.SizeOfData = accentStructSize;
                        data.Data = accentPtr;

                        NativeMethods.SetWindowCompositionAttribute(_handle, ref data);

                        Marshal.FreeHGlobal(accentPtr);
                    }
                    else //TODO: Figure something out for older versions of Windows
                    {

                    }
                }
                else
                    NativeMethods.DwmEnableBlurBehindWindow(_handle, ref _unblurInfo);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            SyncShadowToWindow();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            try
            {
                _titlebar = GetTemplateChild(PartTitlebar) as Grid;
                _titlebar.MouseLeftButtonDown += Titlebar_MouseLeftButtonDown;
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("TITLEBAR \n" + ex);
            }

            try
            {
                _minButton = GetTemplateChild(PartMinimizeButton) as Button;
                _minButton.Click += delegate { WindowState = WindowState.Minimized; };
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("MINBUTTON \n" + ex);
            }

            try
            {
                _maxButton = GetTemplateChild(PartMaximizeButton) as Button;
                _maxButton.Click += delegate
                {
                    WindowState = WindowState.Maximized;
                };
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("MAXBUTTON \n" + ex);
            }

            try
            {
                _restButton = GetTemplateChild(PartRestoreButton) as Button;
                _restButton.Click += delegate
                {
                    WindowState = WindowState.Normal;
                };
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("RESTBUTTON \n" + ex);
            }

            try
            {
                _closeButton = GetTemplateChild(PartCloseButton) as Button;
                _closeButton.Click += delegate
                {
                    Close();
                };
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("CLOSEBUTTON \n" + ex);
            }


            try
            {
                _thumbBottom = GetTemplateChild(PartThumbBottom) as Thumb;
                _thumbBottom.DragDelta += ThumbBottom_DragDelta;


                _thumbTop = GetTemplateChild(PartThumbTop) as Thumb;
                _thumbTop.DragDelta += ThumbTop_DragDelta;


                _thumbBottomRightCorner = GetTemplateChild(PartThumbBottomRightCorner) as Thumb;
                _thumbBottomRightCorner.DragDelta += ThumbBottomRightCorner_DragDelta;


                _thumbTopRightCorner = GetTemplateChild(PartThumbTopRightCorner) as Thumb;
                _thumbTopRightCorner.DragDelta += ThumbTopRightCorner_DragDelta;


                _thumbTopLeftCorner = GetTemplateChild(PartThumbTopLeftCorner) as Thumb;
                _thumbTopLeftCorner.DragDelta += ThumbTopLeftCorner_DragDelta;


                _thumbBottomLeftCorner = GetTemplateChild(PartThumbBottomLeftCorner) as Thumb;
                _thumbBottomLeftCorner.DragDelta += ThumbBottomLeftCorner_DragDelta;


                _thumbRight = GetTemplateChild(PartThumbRight) as Thumb;
                _thumbRight.DragDelta += ThumbRight_DragDelta;


                _thumbLeft = GetTemplateChild(PartThumbLeft) as Thumb;
                _thumbLeft.DragDelta += ThumbLeft_DragDelta;
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("THUMBS \n" + ex);
            }

            try
            {
                _resizeGrip = GetTemplateChild(PartResizeGrip) as Thumb;
                _resizeGrip.DragDelta += ThumbBottomRightCorner_DragDelta;
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("RESIZEGRIP \n" + ex);
            }
        }

        void Titlebar_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            DragMove();
            SyncShadowToWindow();
        }

        void ThumbBottomRightCorner_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10)
                Width += e.HorizontalChange;
            if (Height + e.VerticalChange > 10)
                Height += e.VerticalChange;
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbTopRightCorner_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10)
                Width += e.HorizontalChange;
            if (Top + e.VerticalChange > 10)
            {
                Top += e.VerticalChange;
                Height -= e.VerticalChange;
            }
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbTopLeftCorner_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (Left + e.HorizontalChange > 10)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }
            if (Top + e.VerticalChange > 10)
            {
                Top += e.VerticalChange;
                Height -= e.VerticalChange;
            }
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbBottomLeftCorner_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (Left + e.HorizontalChange > 10)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }
            if (Height + e.VerticalChange > 10)
                Height += e.VerticalChange;
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbRight_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10)
                Width += e.HorizontalChange;
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbLeft_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (Left + e.HorizontalChange > 10)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbBottom_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (Height + e.VerticalChange > 10)
                Height += e.VerticalChange;
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        void ThumbTop_DragDelta(Object sender, DragDeltaEventArgs e)
        {
            if (Top + e.VerticalChange > 10)
            {
                Top += e.VerticalChange;
                Height -= e.VerticalChange;
            }
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }


        public static class NativeMethods
        {
            /*[DllImport("user32.dll")]
            private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

            [StructLayout(LayoutKind.Sequential)]
            internal struct WindowCompositionAttributeData
            {
                public WindowCompositionAttribute Attribute;
                public IntPtr Data;
                public int SizeOfData;
            }

            internal enum WindowCompositionAttribute
            {
                WCA_ACCENT_POLICY = 19
            }

            internal enum AccentState
            {
                ACCENT_DISABLED = 0,
                ACCENT_ENABLE_GRADIENT = 1,
                ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
                ACCENT_ENABLE_BLURBEHIND = 3,
                ACCENT_INVALID_STATE = 4
            }*/

            [DllImport("user32.dll")]
            internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

            [StructLayout(LayoutKind.Sequential)]
            internal struct WindowCompositionAttributeData
            {
                public WindowCompositionAttribute Attribute;
                public IntPtr Data;
                public int SizeOfData;
            }

            internal enum WindowCompositionAttribute
            {
                // ...
                WCA_ACCENT_POLICY = 19
                // ...
            }

            /*internal enum AccentState
            {
                ACCENT_DISABLED = 0,
                ACCENT_ENABLE_GRADIENT = 1,
                ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
                ACCENT_ENABLE_BLURBEHIND = 3,
                ACCENT_INVALID_STATE = 4
            }*/

            internal enum AccentState
            {
                ACCENT_DISABLED = 0,
                ACCENT_ENABLE_GRADIENT = 1,
                ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
                ACCENT_ENABLE_BLURBEHIND = 3,
                ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
                ACCENT_INVALID_STATE = 5
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct AccentPolicy
            {
                public AccentState AccentState;
                public int AccentFlags;
                public int GradientColor;
                public int AnimationId;
            }


            [DllImport("dwmapi.dll")]
            static extern Int32 DwmIsCompositionEnabled(out Boolean enabled);

            public static bool DwmIsCompositionEnabled()
            {
                bool returnValue;
                DwmIsCompositionEnabled(out returnValue);
                return returnValue;
            }


            public static IntPtr SetWindowLong(IntPtr hWnd, Int32 nIndex, Int32 dwNewLong) => IntPtr.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : SetWindowLong32(hWnd, nIndex, dwNewLong);

            [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
            static extern IntPtr SetWindowLong32(IntPtr hWnd, Int32 nIndex, Int32 dwNewLong);

            [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
            static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, Int32 nIndex, Int32 dwNewLong);


            public const Int32 GwlStyle = -16;
            public const Int32 GwlExstyle = -20;

            public const Int32 WsExToolwindow = 0x00000080;
            public const Int32 WsExTransparent = 0x00000020;


            public static IntPtr GetWindowLong(IntPtr hWnd, Int32 nIndex) => IntPtr.Size == 8
            ? GetWindowLongPtr64(hWnd, nIndex)
            : GetWindowLongPtr32(hWnd, nIndex);

            [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
            static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
            private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);


            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;        // x position of upper-left corner
                public int Top;         // y position of upper-left corner
                public int Right;       // x position of lower-right corner
                public int Bottom;      // y position of lower-right corner
            }


            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);


            [DllImport("user32.dll")]
            public static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

            public enum RegionFlags
            {
                ERROR = 0,
                NULLREGION = 1,
                SIMPLEREGION = 2,
                COMPLEXREGION = 3,
            }


            [DllImport("dwmapi.dll")]
            public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

            [StructLayout(LayoutKind.Sequential)]
            public struct DWM_BLURBEHIND
            {
                public DWM_BB dwFlags;
                public bool fEnable;
                public IntPtr hRgnBlur;
                public bool fTransitionOnMaximized;

                public DWM_BLURBEHIND(bool enabled)
                {
                    fEnable = enabled ? true : false;
                    hRgnBlur = IntPtr.Zero;
                    fTransitionOnMaximized = true;
                    dwFlags = DWM_BB.Enable;
                }

                public System.Drawing.Region Region
                {
                    get { return System.Drawing.Region.FromHrgn(hRgnBlur); }
                }

                public bool TransitionOnMaximized
                {
                    get { return fTransitionOnMaximized; }
                    set
                    {
                        fTransitionOnMaximized = value ? true : false;
                        dwFlags |= DWM_BB.TransitionMaximized;
                    }
                }

                public void SetRegion(System.Drawing.Graphics graphics, System.Drawing.Region region)
                {
                    hRgnBlur = region.GetHrgn(graphics);
                    dwFlags |= DWM_BB.BlurRegion;
                }
            }

            [Flags]
            public enum DWM_BB
            {
                Enable = 1,
                BlurRegion = 2,
                TransitionMaximized = 4
            }
        }
    }

    public class WindowStateIsMaximizedToBoolConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            if ((WindowState)value == WindowState.Maximized)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Object ConvertBack(Object value, Type targetType,
            Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}