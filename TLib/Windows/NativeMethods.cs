using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace TLib.Windows
{
    public static class NativeMethods
    {
        #region HotKey
        /// <summary>
        /// 由热键触发的 Windows 消息
        /// </summary>
        public const int WmHotKey = 0x0312;
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        #region KeyboardHook
        public const int WMKEYDOWN = 0x100;
        public const int WMKEYUP = 0x101;
        public const int WMSYSKEYDOWN = 0x104;
        public const int WMSYSKEYUP = 0x105;
        public const int WHKEYBOARDLL = 13;

        internal delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        //安装钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        //卸下钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool UnhookWindowsHookEx(int idHook);
        //下一个钩挂的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);
        //检测大小写锁定状态
        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        internal static extern int GetKeyboardState(byte[] pbKeyState);
        #endregion
        //User32 wrappers cover API's used for Mouse input
        #region Simulation
        // Two special bitmasks we define to be able to grab
        // shift and character information out of a VKey.
        internal const int VKeyShiftMask = 0x0100;
        internal const int VKeyCharMask = 0x00FF;

        // Various Win32 constants
        internal const int KeyeventfExtendedkey = 0x0001;
        internal const int KeyeventfKeyup = 0x0002;
        internal const int KeyeventfScancode = 0x0008;

        internal const int MouseeventfVirtualdesk = 0x4000;

        internal const int SMXvirtualscreen = 76;
        internal const int SMYvirtualscreen = 77;
        internal const int SMCxvirtualscreen = 78;
        internal const int SMCyvirtualscreen = 79;

        internal const int XButton1 = 0x0001;
        internal const int XButton2 = 0x0002;
        internal const int WheelDelta = 120;

        internal const int InputMouse = 0;
        internal const int InputKeyboard = 1;

        // Various Win32 data structures
        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            internal int type;
            internal INPUTUNION union;
        };

        [StructLayout(LayoutKind.Explicit)]
        internal struct INPUTUNION
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mouseInput;
            [FieldOffset(0)]
            internal KEYBDINPUT keyboardInput;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal int dwFlags;
            internal int time;
            internal IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal short wVk;
            internal short wScan;
            internal int dwFlags;
            internal int time;
            internal IntPtr dwExtraInfo;
        };

        [Flags]
        internal enum SendMouseInputFlags
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            Absolute = 0x8000,
        };

        // Importing various Win32 APIs that we need for input
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int MapVirtualKey(int nVirtKey, int nMapType);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int SendInput(int nInputs, ref INPUT mi, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern short VkKeyScan(char ch);

        #endregion
    }
}