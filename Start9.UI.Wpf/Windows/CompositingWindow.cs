using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Start9.UI.Wpf.Windows
{
    public partial class CompositingWindow : Window
    {
        IntPtr _handle;
        /*NativeMethods.DWM_BLURBEHIND _blurInfo = new NativeMethods.DWM_BLURBEHIND();
        NativeMethods.DWM_BLURBEHIND _unblurInfo = new NativeMethods.DWM_BLURBEHIND();*/
        /*{
            fEnable = false
        };*/
        NativeMethods.DWM_BLURBEHIND _blurInfo;
        NativeMethods.DWM_BLURBEHIND _unblurInfo;

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
            DependencyProperty.Register("CompositionState", typeof(WindowCompositionState), typeof(CompositingWindow), new FrameworkPropertyMetadata(WindowCompositionState.Alpha, OnCompositionStatePropertyChangedCallback));

        static void OnCompositionStatePropertyChangedCallback(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("CompositionState: " + (sender as CompositingWindow).CompositionState.ToString());
            (sender as CompositingWindow).SetCompositionState((WindowCompositionState)(e.NewValue));
        }

        public CompositingWindow()
        {
            base.WindowStyle = WindowStyle.None;
            base.AllowsTransparency = true;

            _blurInfo = new NativeMethods.DWM_BLURBEHIND()
            {
                dwFlags = NativeMethods.DWM_BB.Enable | NativeMethods.DWM_BB.BlurRegion | NativeMethods.DWM_BB.TransitionMaximized,
                fEnable = true,
                //hRgnBlur = IntPtr.Zero,
                fTransitionOnMaximized = true
            };

            _unblurInfo = new NativeMethods.DWM_BLURBEHIND()
            {
                dwFlags = NativeMethods.DWM_BB.Enable | NativeMethods.DWM_BB.BlurRegion | NativeMethods.DWM_BB.TransitionMaximized,
                fEnable = false,
                hRgnBlur = IntPtr.Zero,
                fTransitionOnMaximized = true
            };

            base.WindowStyle = WindowStyle.None;
            base.AllowsTransparency = true;
            _handle = new WindowInteropHelper(this).EnsureHandle();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
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

                var data = new NativeMethods.WindowCompositionAttributeData
                {
                    Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                    SizeOfData = accentStructSize,
                    Data = accentPtr
                };

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

        void SetCompositionState()
        {
            if (CompositionState != WindowCompositionState.Alpha)
                SetCompositionState(CompositionState);
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

                        var data = new NativeMethods.WindowCompositionAttributeData
                        {
                            Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                            SizeOfData = accentStructSize,
                            Data = accentPtr
                        };

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

                        var data = new NativeMethods.WindowCompositionAttributeData
                        {
                            Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                            SizeOfData = accentStructSize,
                            Data = accentPtr
                        };

                        NativeMethods.SetWindowCompositionAttribute(_handle, ref data);

                        Marshal.FreeHGlobal(accentPtr);
                    }
                    else //TODO: Figure something out for unmodified Windows 8.x
                    {
                        IntPtr windowRegion = IntPtr.Zero;
                        if (NativeMethods.GetWindowRect(_handle, out NativeMethods.RECT rect))
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

                        var data = new NativeMethods.WindowCompositionAttributeData
                        {
                            Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                            SizeOfData = accentStructSize,
                            Data = accentPtr
                        };

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

                        var data = new NativeMethods.WindowCompositionAttributeData
                        {
                            Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                            SizeOfData = accentStructSize,
                            Data = accentPtr
                        };

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
            if (CompositionState != WindowCompositionState.Alpha)
                SetCompositionState();
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
                DwmIsCompositionEnabled(out bool returnValue);
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
}
