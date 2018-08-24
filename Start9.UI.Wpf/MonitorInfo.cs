using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;
using static Start9.UI.Wpf.MonitorInfo.NativeMethods;

namespace Start9.UI.Wpf
{
    /// <summary>
    /// This class and Start9.UI.Wpf.Windows.AppBarWindow were derived from https://github.com/mgaffigan/WpfAppBar
    /// </summary>
    public class MonitorInfo : IEquatable<MonitorInfo>
    {
        /// <summary>
        /// Gets the bounds of the viewport.
        /// </summary>
        public Rect ViewportBounds { get; }

        /// <summary>
        /// Gets the bounds of the work area.
        /// </summary>
        public Rect WorkAreaBounds { get; }

        /// <summary>
        /// Gets a value that determines if the monitor is the primary monitor or not.
        /// </summary>
        public Boolean IsPrimary { get; }

        /// <summary>
        /// Gets the device ID of the monitor.
        /// </summary>
        public String DeviceId { get; }

        internal MonitorInfo(MonitorInfoEx mex)
        {
            this.ViewportBounds = mex.rcMonitor;
            this.WorkAreaBounds = (Rect)mex.rcWork;
            this.IsPrimary = mex.dwFlags.HasFlag(MonitorInfoF.Primary);
            this.DeviceId = mex.szDevice;
        }

        /// <summary>
        /// Gets a collection of all monitors.
        /// </summary>
        public static ObservableCollection<MonitorInfo> AllMonitors
        {
            get
            {
                var monitors = new ObservableCollection<MonitorInfo>();
                MonitorEnumDelegate callback = delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
                {
                    MonitorInfoEx mi = new MonitorInfoEx
                    {
                        cbSize = Marshal.SizeOf(typeof(MonitorInfoEx))
                    };
                    if (!GetMonitorInfo(hMonitor, ref mi))
                    {
                        throw new System.ComponentModel.Win32Exception();
                    }

                    monitors.Add(new MonitorInfo(mi));
                    return true;
                };

                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero);

                return monitors;
            }
        }

        public override String ToString() => DeviceId;

        public override Boolean Equals(Object obj) => Equals(obj as MonitorInfo);

        public override Int32 GetHashCode() => DeviceId.GetHashCode();

        public Boolean Equals(MonitorInfo other) => this.DeviceId == other?.DeviceId;

        public static Boolean operator ==(MonitorInfo a, MonitorInfo b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static Boolean operator !=(MonitorInfo a, MonitorInfo b) => !(a == b);

        public static class NativeMethods
        {
            public delegate Boolean MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

            [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern Boolean GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

            [DllImport("user32.dll")]
            public static extern Boolean EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

            private const Int32 CchDeviceName = 32;

            [Flags]
            public enum MonitorInfoF
            {
                Primary = 0x1
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct MonitorInfoEx
            {
                public Int32 cbSize;
                public Rect rcMonitor;
                public Rect rcWork;
                public MonitorInfoF dwFlags;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CchDeviceName)]
                public String szDevice;
            }

            public enum ABM
            {
                New = 0,
                Remove,
                QueryPos,
                SetPos,
                GetState,
                GetTaskbarPos,
                Activate,
                GetAutoHideBar,
                SetAutoHideBar,
                WindowPosChanged,
                SetState
            }

            public enum ABN
            {
                StateChange = 0,
                PosChanged,
                FullScreenApp,
                WindowArrange
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WindowPos
            {
                public IntPtr hwnd;
                public IntPtr hwndInsertAfter;
                public Int32 x;
                public Int32 y;
                public Int32 cx;
                public Int32 cy;
                public Int32 flags;

                public System.Windows.Rect Bounds
                {
                    get { return new System.Windows.Rect(x, y, cx, cy); }
                    set
                    {
                        x = (Int32)value.X;
                        y = (Int32)value.Y;
                        cx = (Int32)value.Width;
                        cy = (Int32)value.Height;
                    }
                }
            }

            public const Int32
                SwpNoMove = 0x0002,
                SwpNoSize = 0x0001;

            public const Int32
                WmActivate = 0x0006,
                WmWindowPosChanged = 0x0047,
                WmSysCommand = 0x0112,
                WmWindowPosChanging = 0x0046;

            public const Int32
                ScMove = 0xF010;

            [DllImport("shell32.dll", ExactSpelling = true)]
            public static extern UInt32 SHAppBarMessage(ABM dwMessage, ref AppBarData pData);

            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public Rect(Int32 left, Int32 top, Int32 right, Int32 bottom)
                {
                    Left = left;
                    Top = top;
                    Right = right;
                    Bottom = bottom;
                }

                public Int32 Left;
                public Int32 Top;
                public Int32 Right;
                public Int32 Bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct AppBarData
            {
                public Int32 cbSize;
                public IntPtr hWnd;
                public Int32 uCallbackMessage;
                public Int32 uEdge;
                public Rect rc;
                public IntPtr lParam;
            }

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern Int32 RegisterWindowMessage(String msg);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, Int32 wParam, Int32 lParam);

            /*public static Rect SysWinRectToNativeRect(SysWinRect rect)
            {
                return new Rect((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
            }*/
        }
    }
}
