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
using static Start9.UI.Wpf.MonitorInfo.NativeMethods;
//using static Start9.UI.Wpf.Windows.AppBarWindow.NativeMethods;
//using Rect = Start9.UI.Wpf.Windows.AppBarWindow.NativeMethods.Rect;
//using SysWinRect = System.Windows.Rect;

namespace Start9.UI.Wpf.Windows
{
    public class AppBarWindow : CompositingWindow
    {
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

        public AppBarDockMode DockMode
        {
            get { return (AppBarDockMode)GetValue(DockModeProperty); }
            set { SetValue(DockModeProperty, value); }
        }

        public static readonly DependencyProperty DockModeProperty =
            DependencyProperty.Register("DockMode", typeof(AppBarDockMode), typeof(AppBarWindow),
                new FrameworkPropertyMetadata(AppBarDockMode.Left, DockLocation_Changed));

        public MonitorInfo Monitor
        {
            get { return (MonitorInfo)GetValue(MonitorProperty); }
            set { SetValue(MonitorProperty, value); }
        }

        public static readonly DependencyProperty MonitorProperty =
            DependencyProperty.Register("Monitor", typeof(MonitorInfo), typeof(AppBarWindow),
                new FrameworkPropertyMetadata(null, DockLocation_Changed));

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
