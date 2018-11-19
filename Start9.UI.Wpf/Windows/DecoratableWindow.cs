using Start9.UI.Wpf.Statics;
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

    [TemplatePart(Name = PartSystemMenuRestore, Type = typeof(MenuItem))]
    [TemplatePart(Name = PartSystemMenuMove, Type = typeof(MenuItem))]
    [TemplatePart(Name = PartSystemMenuSize, Type = typeof(MenuItem))]
    [TemplatePart(Name = PartSystemMenuMinimize, Type = typeof(MenuItem))]
    [TemplatePart(Name = PartSystemMenuMaximize, Type = typeof(MenuItem))]
    [TemplatePart(Name = PartSystemMenuClose, Type = typeof(MenuItem))]

    [ContentProperty("Content")]
    public partial class DecoratableWindow : ShadowedWindow
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

        const String PartSystemMenuRestore = "PART_SystemMenuRestore";
        const String PartSystemMenuMove = "PART_SystemMenuMove";
        const String PartSystemMenuSize = "PART_SystemMenuSize";
        const String PartSystemMenuMinimize = "PART_SystemMenuMinimize";
        const String PartSystemMenuMaximize = "PART_SystemMenuMaximize";
        const String PartSystemMenuClose = "PART_SystemMenuClose";

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

        MenuItem _systemMenuRestore;
        MenuItem _systemMenuMove;
        MenuItem _systemMenuSize;
        MenuItem _systemMenuMinimize;
        MenuItem _systemMenuMaximize;
        MenuItem _systemMenuClose;

        new public WindowStyle WindowStyle
        {
            get => (WindowStyle)GetValue(WindowStyleProperty);
            set => SetValue(WindowStyleProperty, value);
        }

        new public static readonly DependencyProperty WindowStyleProperty =
            DependencyProperty.Register("WindowStyle", typeof(WindowStyle), typeof(DecoratableWindow), new PropertyMetadata(WindowStyle.SingleBorderWindow));

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

        public bool ShowTitlebarText
        {
            get => (bool)GetValue(ShowTitlebarTextProperty);
            set => SetValue(ShowTitlebarTextProperty, value);
        }

        public static readonly DependencyProperty ShowTitlebarTextProperty =
            DependencyProperty.Register("ShowTitlebarText", typeof(bool), typeof(DecoratableWindow), new PropertyMetadata(true));

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

        static DecoratableWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(DecoratableWindow), new FrameworkPropertyMetadata(typeof(DecoratableWindow)));
        }

        public DecoratableWindow()
        {
            /*base.WindowStyle = WindowStyle.None;
            base.AllowsTransparency = true;*/

            StateChanged += (sneder, args) =>
            {
                if (WindowState == WindowState.Maximized)
                {
                    System.Windows.Forms.Screen s = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)SystemScaling.WpfUnitsToRealPixels(Left), (int)SystemScaling.WpfUnitsToRealPixels(Top)));
                    MaxWidth = s.WorkingArea.Width;
                    MaxHeight = s.WorkingArea.Height;
                }
                else
                {
                    MaxWidth = Int32.MaxValue;
                    MaxHeight = Int32.MaxValue;
                }

                ValidateSystemMenuItemStates();
            };

            ////DefaultStyleKey = typeof(DecoratableWindow);
            //Style = (Style)Resources[typeof(DecoratableWindow)];

            ////SetResourceReference(StyleProperty, typeof(DecoratableWindow));

            bool windowResourceIsNull = TryFindResource(GetType()) == null;
            bool appResourceIsNull = TryFindResource(GetType()) == null;

            bool defaultStyleFallback = false;

            if (Application.Current == null)
                defaultStyleFallback = windowResourceIsNull;
            else
                defaultStyleFallback = windowResourceIsNull && appResourceIsNull;

            if ((Style == null) && defaultStyleFallback)
            {
                Debug.WriteLine("DecoratableWindow type: " + GetType().FullName);
                SetResourceReference(StyleProperty, typeof(DecoratableWindow));
            }

            /*Loaded += (sneder, args) =>
            {
                SyncShadowToWindowSize();
            };*/
        }

        /*protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            if (this.SizeToContent != SizeToContent.Manual)
            {
                SyncShadowToWindowSize();
            }
        }*/

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _titlebar = GetTemplateChild(PartTitlebar) as Grid;
            if (_titlebar != null)
            {
                _titlebar.MouseLeftButtonDown += Titlebar_MouseLeftButtonDown;
                //_titlebar.double
            }

            _minButton = GetTemplateChild(PartMinimizeButton) as Button;
            if (_minButton != null)
                _minButton.Click += (sneder, args) =>
                {
                    WindowState = WindowState.Minimized;
                };


            _maxButton = GetTemplateChild(PartMaximizeButton) as Button;
            if (_maxButton != null)
                _maxButton.Click += (sneder, args) =>
                {
                    WindowState = WindowState.Maximized;
                };


            _restButton = GetTemplateChild(PartRestoreButton) as Button;
            if (_restButton != null)
                _restButton.Click += (sneder, args) =>
                {
                    WindowState = WindowState.Normal;
                };

            _closeButton = GetTemplateChild(PartCloseButton) as Button;
            if (_closeButton != null)
                _closeButton.Click += (sneder, args) =>
                {
                    Close();
                };


            _thumbBottom = GetTemplateChild(PartThumbBottom) as Thumb;
            if (_thumbBottom != null)
                _thumbBottom.DragDelta += ThumbBottom_DragDelta;


            _thumbTop = GetTemplateChild(PartThumbTop) as Thumb;
            if (_thumbTop != null)
                _thumbTop.DragDelta += ThumbTop_DragDelta;


            _thumbBottomRightCorner = GetTemplateChild(PartThumbBottomRightCorner) as Thumb;
            if (_thumbBottomRightCorner != null)
                _thumbBottomRightCorner.DragDelta += ThumbBottomRightCorner_DragDelta;


            _thumbTopRightCorner = GetTemplateChild(PartThumbTopRightCorner) as Thumb;
            if (_thumbTopRightCorner != null)
                _thumbTopRightCorner.DragDelta += ThumbTopRightCorner_DragDelta;


            _thumbTopLeftCorner = GetTemplateChild(PartThumbTopLeftCorner) as Thumb;
            if (_thumbTopLeftCorner != null)
                _thumbTopLeftCorner.DragDelta += ThumbTopLeftCorner_DragDelta;


            _thumbBottomLeftCorner = GetTemplateChild(PartThumbBottomLeftCorner) as Thumb;
            if (_thumbBottomLeftCorner != null)
                _thumbBottomLeftCorner.DragDelta += ThumbBottomLeftCorner_DragDelta;


            _thumbRight = GetTemplateChild(PartThumbRight) as Thumb;
            if (_thumbRight != null)
                _thumbRight.DragDelta += ThumbRight_DragDelta;


            _thumbLeft = GetTemplateChild(PartThumbLeft) as Thumb;
            if (_thumbLeft != null)
            {
                /*_thumbLeft.MouseLeftButtonDown += (sneder, args) =>
                {
                    NativeMethods.SendMessage(new WindowInteropHelper(this).Handle, 0xA1, 0xF000 + NativeMethods.ScSizeHtLeft, 0);
                };*/
                _thumbLeft.DragDelta += ThumbLeft_DragDelta;
            }

            _resizeGrip = GetTemplateChild(PartResizeGrip) as Thumb;
            if (_resizeGrip != null)
                _resizeGrip.DragDelta += ThumbBottomRightCorner_DragDelta;

            _systemMenuRestore = GetTemplateChild(PartSystemMenuRestore) as MenuItem;
            if (_systemMenuRestore != null)
                _systemMenuRestore.Click += (sneder, args) =>
                {
                    WindowState = WindowState.Normal;
                };

            _systemMenuMove = GetTemplateChild(PartSystemMenuMove) as MenuItem;
            /*if (_systemMenuMove != null)
                _systemMenuMove.Click += (sneder, args) =>
                {
                };*/

            _systemMenuSize = GetTemplateChild(PartSystemMenuSize) as MenuItem;
            /*if (_systemMenuSize != null)
                _systemMenuSize.Click += (sneder, args) =>
                {
                };*/

            _systemMenuMinimize = GetTemplateChild(PartSystemMenuMinimize) as MenuItem;
            if (_systemMenuMinimize != null)
                _systemMenuMinimize.Click += (sneder, args) =>
                {
                    WindowState = WindowState.Minimized;
                };

            _systemMenuMaximize = GetTemplateChild(PartSystemMenuMaximize) as MenuItem;
            if (_systemMenuMaximize != null)
                _systemMenuMaximize.Click += (sneder, args) =>
                {
                    WindowState = WindowState.Maximized;
                };

            _systemMenuClose = GetTemplateChild(PartSystemMenuClose) as MenuItem;
            if (_systemMenuClose != null)
                _systemMenuClose.Click += (sneder, args) =>
                {
                    Close();
                };

            ValidateSystemMenuItemStates();
        }

        void ValidateSystemMenuItemStates()
        {
            if (WindowState == WindowState.Maximized)
            {
                if (_systemMenuRestore != null)
                    _systemMenuRestore.IsEnabled = true;

                if (_systemMenuMaximize != null)
                    _systemMenuMaximize.IsEnabled = false;
            }
            else
            {
                if (_systemMenuRestore != null)
                    _systemMenuRestore.IsEnabled = false;

                if (_systemMenuMaximize != null)
                    _systemMenuMaximize.IsEnabled = true;
            }

            if (_systemMenuMove != null)
                _systemMenuMove.IsEnabled = false;

            if (_systemMenuSize != null)
                _systemMenuSize.IsEnabled = false;
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