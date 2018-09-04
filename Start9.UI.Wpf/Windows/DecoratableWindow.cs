using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;

namespace Start9.UI.Wpf.Windows
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
    public partial class DecoratableWindow : CompositingWindow
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
            /*base.WindowStyle = WindowStyle.None;
            base.AllowsTransparency = true;*/

            _shadowWindow = new Window()
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                ShowInTaskbar = false,
                ShowActivated = false
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

            Closed += (sneder, args) =>
            {
                _shadowWindow.Close();
            };
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
            /*if (CompositionState != WindowCompositionState.Alpha)
                SetCompositionState();*/
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