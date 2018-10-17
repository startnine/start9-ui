using Start9.UI.Wpf.Statics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Orientation = System.Windows.Controls.Orientation;
using static Start9.UI.Wpf.MonitorInfo.NativeMethods;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
//using static Start9.UI.Wpf.Windows.AppBarWindow.NativeMethods;
//using Rect = Start9.UI.Wpf.Windows.AppBarWindow.NativeMethods.Rect;
//using SysWinRect = System.Windows.Rect;

namespace Start9.UI.Wpf.Windows
{

    [TemplatePart(Name = PartDragMoveGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PartResizeThumb, Type = typeof(Thumb))]
    public class AppBarWindow : CompositingWindow
    {
        const String PartDragMoveGrid = "PART_DragMoveGrid";
        const String PartResizeThumb = "PART_ResizeThumb";

        private Boolean IsAppBarRegistered;
        private Boolean IsInAppBarResize;

        static AppBarWindow()
        {
            ShowInTaskbarProperty.OverrideMetadata(typeof(AppBarWindow), new FrameworkPropertyMetadata(false));
            MinHeightProperty.OverrideMetadata(typeof(AppBarWindow), new FrameworkPropertyMetadata(20d, MinMaxHeightWidth_Changed));
            MinWidthProperty.OverrideMetadata(typeof(AppBarWindow), new FrameworkPropertyMetadata(20d, MinMaxHeightWidth_Changed));
            MaxHeightProperty.OverrideMetadata(typeof(AppBarWindow), new FrameworkPropertyMetadata(MinMaxHeightWidth_Changed));
            MaxWidthProperty.OverrideMetadata(typeof(AppBarWindow), new FrameworkPropertyMetadata(MinMaxHeightWidth_Changed));
        }

        public AppBarWindow()
        {
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
        }

        Grid _dragMoveGrid;
        Thumb _resizeThumb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dragMoveGrid = GetTemplateChild(PartDragMoveGrid) as Grid;
            if (_dragMoveGrid != null)
            {
                _dragMoveGrid.MouseLeftButtonDown += DragMoveGrid_PreviewMouseLeftButtonDown;
            }

            _resizeThumb = GetTemplateChild(PartResizeThumb) as Thumb;
            if (_resizeThumb != null)
            {
                _resizeThumb.PreviewMouseLeftButtonDown += ResizeThumb_PreviewMouseLeftButtonDown;
            }
        }

        private void DragMoveGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AppBarDockMode targetMode = DockMode;
            var timer = new System.Timers.Timer(10);

            Window highlightWindow = new Window()
            {
                Topmost = true,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Background = new SolidColorBrush(Colors.Red)
            };
            //highlightWindow.Show();
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        var curPos = System.Windows.Forms.Cursor.Position;
                        var winPoint = PointToScreen(new Point(0, 0));
                        if ((curPos.X > winPoint.X)
                        && (curPos.Y > winPoint.Y)
                        && (curPos.X <= SystemScaling.WpfUnitsToRealPixels(Left + ActualWidth))
                        && (curPos.Y <= SystemScaling.WpfUnitsToRealPixels(Top + ActualHeight))
                        )
                        {
                            targetMode = DockMode;
                            highlightWindow.Hide();
                        }
                        else
                        {
                            highlightWindow.Show();

                            var screen = System.Windows.Forms.Screen.FromPoint(curPos);
                            double horizontal = (double)(curPos.X) / (double)(screen.Bounds.Width);
                            double vertical = (double)(curPos.Y) / (double)(screen.Bounds.Height);

                            if (horizontal > 0.5)
                            {
                                if (vertical > 0.5)
                                {
                                    if (horizontal > vertical)
                                        targetMode = AppBarDockMode.Right;
                                    else
                                        targetMode = AppBarDockMode.Bottom;
                                }
                                else
                                {
                                    if (vertical > horizontal)
                                        targetMode = AppBarDockMode.Right;
                                    else
                                        targetMode = AppBarDockMode.Top;
                                }
                            }
                            else
                            {
                                if (vertical > 0.5)
                                {
                                    if (horizontal > vertical)
                                        targetMode = AppBarDockMode.Left;
                                    else
                                        targetMode = AppBarDockMode.Bottom;
                                }
                                else
                                {
                                    if (vertical > horizontal)
                                        targetMode = AppBarDockMode.Left;
                                    else
                                        targetMode = AppBarDockMode.Top;
                                }
                            }

                        if (targetMode == AppBarDockMode.Left)
                        {
                            highlightWindow.Left = screen.WorkingArea.Left;
                            highlightWindow.Top = screen.WorkingArea.Top;
                            highlightWindow.Width = DockedWidthOrHeight;
                            highlightWindow.Height = screen.WorkingArea.Height;
                        }
                        else if (targetMode == AppBarDockMode.Top)
                        {
                            highlightWindow.Left = screen.WorkingArea.Left;
                            highlightWindow.Top = screen.WorkingArea.Top;
                            highlightWindow.Width = screen.WorkingArea.Width;
                            highlightWindow.Height = DockedWidthOrHeight;
                        }
                        else if (targetMode == AppBarDockMode.Right)
                        {
                            highlightWindow.Left = screen.WorkingArea.Right - DockedWidthOrHeight;
                            highlightWindow.Top = screen.WorkingArea.Top;
                            highlightWindow.Width = DockedWidthOrHeight;
                            highlightWindow.Height = screen.WorkingArea.Height;
                        }
                        else
                        {
                            highlightWindow.Left = screen.WorkingArea.Left;
                            highlightWindow.Top = screen.WorkingArea.Bottom - DockedWidthOrHeight;
                            highlightWindow.Width = screen.WorkingArea.Width;
                            highlightWindow.Height = DockedWidthOrHeight;
                            }
                        }
                    }
                    else
                    {
                        DockMode = targetMode;
                        highlightWindow.Close();
                        timer.Stop();
                    }
                }));
            };

            if (IsUnlocked)
                timer.Start();
        }

        private void ResizeThumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (UseIntervalResizing)
            {
                var timer = new System.Timers.Timer(10);
                double targetWidthOrHeight = DockedWidthOrHeight;
                var initialCurPos = System.Windows.Forms.Cursor.Position;
                double xChange = 0;
                double yChange = 0;

                timer.Elapsed += (sneder, args) =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (Mouse.LeftButton == MouseButtonState.Pressed)
                        {
                            var curPos = System.Windows.Forms.Cursor.Position;
                            xChange = curPos.X - initialCurPos.X;
                            yChange = curPos.Y - initialCurPos.Y;

                            if (Orientation == Orientation.Vertical)
                            {
                                if (xChange > ResizeIntervalDistance)
                                {
                                    if (DockMode == AppBarDockMode.Left)
                                    {
                                        targetWidthOrHeight += ResizeIntervalDistance;
                                        //xChange -= ResizeIntervalDistance;
                                        //initialCurPos.X -= (int)ResizeIntervalDistance;
                                    }
                                    else
                                    {
                                        targetWidthOrHeight -= ResizeIntervalDistance;
                                        //xChange += ResizeIntervalDistance;
                                    }
                                    initialCurPos.X += (int)ResizeIntervalDistance;
                                }
                                else if (xChange <= (ResizeIntervalDistance * -1))
                                {
                                    if (DockMode == AppBarDockMode.Left)
                                    {
                                        targetWidthOrHeight -= ResizeIntervalDistance;
                                        //xChange += ResizeIntervalDistance;
                                        //initialCurPos.X += (int)ResizeIntervalDistance;
                                    }
                                    else
                                    {
                                        targetWidthOrHeight += ResizeIntervalDistance;
                                        //xChange -= ResizeIntervalDistance;
                                    }
                                    initialCurPos.X -= (int)ResizeIntervalDistance;
                                }
                            }
                            else
                            {
                                if (yChange > ResizeIntervalDistance)
                                {
                                    if (DockMode == AppBarDockMode.Top)
                                    {
                                        targetWidthOrHeight += ResizeIntervalDistance;
                                        //yChange -= ResizeIntervalDistance;
                                        //yChange = curPos.Y - (Top + Height);
                                        //initialCurPos.Y -= (int)ResizeIntervalDistance;
                                    }
                                    else
                                    {
                                        targetWidthOrHeight -= ResizeIntervalDistance;
                                        //yChange += ResizeIntervalDistance;
                                        //yChange = curPos.Y - Top;
                                    }
                                    initialCurPos.Y += (int)ResizeIntervalDistance;
                                }
                                else if (yChange <= (ResizeIntervalDistance * -1))
                                {
                                    if (DockMode == AppBarDockMode.Top)
                                    {
                                        targetWidthOrHeight -= ResizeIntervalDistance;
                                        //yChange += ResizeIntervalDistance;
                                        //yChange = curPos.Y - (Top + Height);
                                        //initialCurPos.Y += (int)ResizeIntervalDistance;
                                    }
                                    else
                                    {
                                        targetWidthOrHeight += ResizeIntervalDistance;
                                        //yChange -= ResizeIntervalDistance;
                                        //yChange = curPos.Y - Top;
                                    }
                                    initialCurPos.Y -= (int)ResizeIntervalDistance;
                                }
                            }

                            DockedWidthOrHeight = (int)targetWidthOrHeight;
                        }
                        else
                            timer.Stop();
                    }));
                };

                if (IsUnlocked)
                    timer.Start();
            }
        }

        public bool IsUnlocked
        {
            get => (bool)GetValue(IsUnlockedProperty);
            set => SetValue(IsUnlockedProperty, value);
        }

        public static readonly DependencyProperty IsUnlockedProperty =
            DependencyProperty.Register("IsUnlocked", typeof(bool), typeof(AppBarWindow),
                new FrameworkPropertyMetadata(true));

        public AppBarDockMode DockMode
        {
            get { return (AppBarDockMode)GetValue(DockModeProperty); }
            set { SetValue(DockModeProperty, value); }
        }

        public static readonly DependencyProperty DockModeProperty =
            DependencyProperty.Register("DockMode", typeof(AppBarDockMode), typeof(AppBarWindow),
                new FrameworkPropertyMetadata(AppBarDockMode.Left, DockLocation_Changed));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            private set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AppBarWindow), new FrameworkPropertyMetadata(Orientation.Vertical));

        public MonitorInfo Monitor
        {
            get { return (MonitorInfo)GetValue(MonitorProperty); }
            set { SetValue(MonitorProperty, value); }
        }

        public static readonly DependencyProperty MonitorProperty =
            DependencyProperty.Register("Monitor", typeof(MonitorInfo), typeof(AppBarWindow),
                new FrameworkPropertyMetadata(null, DockLocation_Changed));

        public bool UseIntervalResizing
        {
            get => (bool)GetValue(UseIntervalResizingProperty);
            set => SetValue(UseIntervalResizingProperty, value);
        }

        public static readonly DependencyProperty UseIntervalResizingProperty =
            DependencyProperty.Register("UseIntervalResizing", typeof(bool), typeof(AppBarWindow), new FrameworkPropertyMetadata(true));

        public double ResizeIntervalDistance
        {
            get { return (double)GetValue(ResizeIntervalDistanceProperty); }
            set { SetValue(ResizeIntervalDistanceProperty, value); }
        }

        public static readonly DependencyProperty ResizeIntervalDistanceProperty =
            DependencyProperty.Register("ResizeIntervalDistance", typeof(double), typeof(AppBarWindow),
                new FrameworkPropertyMetadata(30.0));

        public Int32 DockedWidthOrHeight
        {
            get { return (Int32)GetValue(DockedWidthOrHeightProperty); }
            set { SetValue(DockedWidthOrHeightProperty, value); }
        }

        public static readonly DependencyProperty DockedWidthOrHeightProperty =
            DependencyProperty.Register("DockedWidthOrHeight", typeof(Int32), typeof(AppBarWindow),
                new FrameworkPropertyMetadata(200, DockLocation_Changed, DockedWidthOrHeight_Coerce));

        private static Object DockedWidthOrHeight_Coerce(DependencyObject d, Object baseValue)
        {
            var @this = (AppBarWindow)d;
            var newValue = (Int32)baseValue;

            switch (@this.DockMode)
            {
                case AppBarDockMode.Left:
                case AppBarDockMode.Right:
                    return BoundIntToDouble(newValue, @this.MinWidth, @this.MaxWidth);

                case AppBarDockMode.Top:
                case AppBarDockMode.Bottom:
                    return BoundIntToDouble(newValue, @this.MinHeight, @this.MaxHeight);

                default: throw new NotSupportedException();
            }
        }

        private static Int32 BoundIntToDouble(Int32 value, Double min, Double max)
        {
            if (min > value)
            {
                return (Int32)Math.Ceiling(min);
            }
            if (max < value)
            {
                return (Int32)Math.Floor(max);
            }

            return value;
        }

        private static void MinMaxHeightWidth_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockedWidthOrHeightProperty);
        }

        private static void DockLocation_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (AppBarWindow)d;

            if (@this.IsAppBarRegistered)
            {
                @this.OnDockLocationChanged();
            }
        }/*

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);

            OnDockLocationChanged();
        }*/

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // add the hook, setup the appbar
            var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle); //HwndSource.FromHwnd(new WindowInteropHelper(this).EnsureHandle()); // (HwndSource)PresentationSource.FromVisual(this);
            source.AddHook(WndProc);

            var abd = GetAppBarData();
            SHAppBarMessage(ABM.New, ref abd);

            // set our initial location
            this.IsAppBarRegistered = true;
            OnDockLocationChanged();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (IsAppBarRegistered)
            {
                var abd = GetAppBarData();
                SHAppBarMessage(ABM.Remove, ref abd);
                IsAppBarRegistered = false;
            }
        }

        private Int32 WpfDimensionToDesktop(Double dim)
        {
            return (Int32)Math.Ceiling(SystemScaling.WpfUnitsToRealPixels(dim));
        }

        private Double DesktopDimensionToWpf(Double dim)
        {
            return (Int32)Math.Ceiling(SystemScaling.RealPixelsToWpfUnits(dim));
        }

        private void OnDockLocationChanged()
        {
            if (IsInAppBarResize)
            {
                return;
            }

            var abd = GetAppBarData();
            abd.rc = GetSelectedMonitor().ViewportBounds;

            SHAppBarMessage(ABM.QueryPos, ref abd);

            var dockedWidthOrHeightInDesktopPixels = WpfDimensionToDesktop(DockedWidthOrHeight);
            switch (DockMode)
            {
                case AppBarDockMode.Top:
                    abd.rc.Bottom = abd.rc.Top + dockedWidthOrHeightInDesktopPixels;
                    break;
                case AppBarDockMode.Bottom:
                    abd.rc.Top = abd.rc.Bottom - dockedWidthOrHeightInDesktopPixels;
                    break;
                case AppBarDockMode.Left:
                    abd.rc.Right = abd.rc.Left + dockedWidthOrHeightInDesktopPixels;
                    break;
                case AppBarDockMode.Right:
                    abd.rc.Left = abd.rc.Right - dockedWidthOrHeightInDesktopPixels;
                    break;
                default: throw new NotSupportedException();
            }

            if (DockMode == AppBarDockMode.Top || DockMode == AppBarDockMode.Bottom)
                Orientation = Orientation.Horizontal;
            else
                Orientation = Orientation.Vertical;

            SHAppBarMessage(ABM.SetPos, ref abd);
            IsInAppBarResize = true;
            try
            {
                WindowBounds = new System.Windows.Rect()
                {
                    Location = new Point(abd.rc.Left, abd.rc.Top),
                    Size = new Size(abd.rc.Right - abd.rc.Left, abd.rc.Bottom - abd.rc.Top)
                };
            }
            finally
            {
                IsInAppBarResize = false;
            }
        }

        private MonitorInfo GetSelectedMonitor()
        {
            var monitor = Monitor;
            var allMonitors = MonitorInfo.AllMonitors;
            if (monitor == null || !allMonitors.Contains(monitor))
            {
                monitor = allMonitors.First(f => f.IsPrimary);
            }

            return monitor;
        }

        private AppBarData GetAppBarData()
        {
            return new AppBarData()
            {
                cbSize = Marshal.SizeOf(typeof(AppBarData)),
                hWnd = new WindowInteropHelper(this).Handle,
                uCallbackMessage = AppBarMessageId,
                uEdge = (Int32)DockMode
            };
        }

        private static Int32 _AppBarMessageId;
        public static Int32 AppBarMessageId
        {
            get
            {
                if (_AppBarMessageId == 0)
                {
                    _AppBarMessageId = RegisterWindowMessage("AppBarMessage_EEDFB5206FC3");
                }

                return _AppBarMessageId;
            }
        }

        public IntPtr WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
        {
            if (msg == WmWindowPosChanging && !IsInAppBarResize)
            {
                var wp = (WindowPos)Marshal.PtrToStructure(lParam, typeof(WindowPos));
                wp.flags |= SwpNoMove | SwpNoSize;
                Marshal.StructureToPtr(wp, lParam, false);
            }
            else if (msg == WmActivate)
            {
                var abd = GetAppBarData();
                SHAppBarMessage(ABM.Activate, ref abd);
            }
            else if (msg == WmWindowPosChanged)
            {
                var abd = GetAppBarData();
                SHAppBarMessage(ABM.WindowPosChanged, ref abd);
            }
            else if (msg == AppBarMessageId)
            {
                switch ((ABN)(Int32)wParam)
                {
                    case ABN.PosChanged:
                        OnDockLocationChanged();
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }

        private System.Windows.Rect WindowBounds
        {
            set
            {
                this.Left = DesktopDimensionToWpf(value.Left);
                this.Top = DesktopDimensionToWpf(value.Top);
                this.Width = DesktopDimensionToWpf(value.Width);
                this.Height = DesktopDimensionToWpf(value.Height);
            }
        }

        public enum AppBarDockMode
        {
            Left = 0,
            Top,
            Right,
            Bottom
        }
    }
}
