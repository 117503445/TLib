using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TLib.Windows
{

    public partial class KeyboardHookAPI
    {
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
        public const int WH_KEYBOARD_LL = 13;
        [StructLayout(LayoutKind.Sequential)] //声明键盘钩子的封送结构类型
        public class KeyboardHookStruct
        {
            public int vkCode; //表示一个在1到254间的虚似键盘码 
            public int scanCode; //表示硬件扫描码
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        //安装钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        //卸下钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        //下一个钩挂的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        //检测大小写锁定状态
        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        public static extern int GetKeyboardState(byte[] pbKeyState);
    }
    public class KeyboardHook
    {
        public static bool CapsLockStatus
        {
            get
            {
                byte[] bs = new byte[256];
                KeyboardHookAPI.GetKeyboardState(bs);
                bool result = (bs[0x14] == 1);
                //Console.WriteLine("result="+result);
                return result;
            }
        }

        readonly bool isWriteDown = false;
        public KeyboardHook(bool isWriteDown) { this.isWriteDown = isWriteDown; }
        int hHook;
        KeyboardHookAPI.HookProc KeyboardHookDelegate;
        /// <summary>
        /// 安装键盘钩子
        /// </summary>
        public void SetHook()
        {
            KeyboardHookDelegate = new KeyboardHookAPI.HookProc(KeyboardHookProc);
            ProcessModule cModule = Process.GetCurrentProcess().MainModule;
            var mh = KeyboardHookAPI.GetModuleHandle(cModule.ModuleName);
            hHook = KeyboardHookAPI.SetWindowsHookEx(KeyboardHookAPI.WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);
            GCHandle.Alloc(KeyboardHookDelegate);
        }
        /// <summary>
        /// 卸载键盘钩子
        /// </summary>
        public void UnHook()
        {
            KeyboardHookAPI.UnhookWindowsHookEx(hHook);
        }
        public EventHandler<KeyEventArgs> OnKeyDownEvent;
        public EventHandler<KeyEventArgs> OnKeyUpEvent;
        /// <summary>
        /// 获取键盘消息
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 如果该消息被丢弃（nCode<0
            if (nCode >= 0)
            {
                KeyboardHookAPI.KeyboardHookStruct KeyDataFromHook = (KeyboardHookAPI.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookAPI.KeyboardHookStruct));
                int keyData = KeyDataFromHook.vkCode;
                //WM_KEYDOWN和WM_SYSKEYDOWN消息，将会引发OnKeyDownEvent事件
                if ((wParam == KeyboardHookAPI.WM_KEYDOWN || wParam == KeyboardHookAPI.WM_SYSKEYDOWN))
                {
                    Key key = KeyInterop.KeyFromVirtualKey(keyData);
                    OnKeyDownEvent?.Invoke(this, new KeyEventArgs(key));
                    if (isWriteDown)
                    {
                        System.IO.File.AppendAllText("KeyLog.txt", string.Format("{0};{1};{2}\r\n", key.ToString(), CapsLockStatus ? 1 : 0, 0));
                    }
                }
                //WM_KEYUP和WM_SYSKEYUP消息，将引发OnKeyUpEvent事件 
                if ((wParam == KeyboardHookAPI.WM_KEYUP || wParam == KeyboardHookAPI.WM_SYSKEYUP))
                {
                    Key key = KeyInterop.KeyFromVirtualKey(keyData);
                    OnKeyUpEvent?.Invoke(this, new KeyEventArgs(key));
                    if (isWriteDown)
                    {
                        System.IO.File.AppendAllText("KeyLog.txt", string.Format("{0};{1};{2}\r\n", key.ToString(), CapsLockStatus ? 1 : 0, 1));
                    }
                }
            }
            return KeyboardHookAPI.CallNextHookEx(hHook, nCode, wParam, lParam);
        }
        public class KeyEventArgs : EventArgs
        {
            public Key key;
            public KeyEventArgs(Key key) { this.key = key; }
        }
    }
}
