using System;
using System.Runtime.InteropServices;
namespace TLib.Windows
{
    public static class KeyboardHookAPI
    {
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
    }
}