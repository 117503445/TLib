using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
namespace TLib.Windows
{
    /// <summary>
    /// 监控键盘
    /// </summary>
    public class KeyboardHook
    {
        /// <summary>
        /// 获得当前的大写锁定状态
        /// </summary>
        private static bool CapsLockStatus
        {
            get
            {
                byte[] bs = new byte[256];
                _ = NativeMethods.GetKeyboardState(bs);
                bool result = (bs[0x14] == 1);
                //Console.WriteLine("result="+result);
                return result;
            }
        }
        /// <summary>
        /// 是否拦截键盘消息
        /// </summary>
        public bool IsHoldKey { get; set; } = false;
        /// <summary>
        /// 创建hook对象,并自动开始监视
        /// </summary>
        public KeyboardHook()
        {
            SetHook();
        }
        private int hHook;
        NativeMethods.HookProc KeyboardHookDelegate;
        /// <summary>
        /// 手动安装键盘钩子
        /// </summary>
        public void SetHook()
        {
            KeyboardHookDelegate = new NativeMethods.HookProc(KeyboardHookProc);
            ProcessModule cModule = Process.GetCurrentProcess().MainModule;
            var mh = NativeMethods.GetModuleHandle(cModule.ModuleName);
            hHook = NativeMethods.SetWindowsHookEx(NativeMethods.WHKEYBOARDLL, KeyboardHookDelegate, mh, 0);
            GCHandle.Alloc(KeyboardHookDelegate);
        }
        /// <summary>
        /// 手动卸载键盘钩子
        /// </summary>
        public void UnHook()
        {
            NativeMethods.UnhookWindowsHookEx(hHook);
        }
        /// <summary>
        /// 案件按下
        /// </summary>
        public event EventHandler<KeyboardHookEventArgs> KeyDown;
        /// <summary>
        /// 按键弹起
        /// </summary>
        public event EventHandler<KeyboardHookEventArgs> KeyUp;
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
                KeyboardHookStruct KeyDataFromHook = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                int keyData = KeyDataFromHook.VkCode;
                //WM_KEYDOWN和WM_SYSKEYDOWN消息，将会引发OnKeyDownEvent事件
                if ((wParam == NativeMethods.WMKEYDOWN || wParam == NativeMethods.WMSYSKEYDOWN))
                {
                    Key key = KeyInterop.KeyFromVirtualKey(keyData);
                    KeyDown?.Invoke(this, new KeyboardHookEventArgs(key, CapsLockStatus));
                }
                //WM_KEYUP和WM_SYSKEYUP消息，将引发OnKeyUpEvent事件 
                if ((wParam == NativeMethods.WMKEYUP || wParam == NativeMethods.WMSYSKEYUP))
                {
                    Key key = KeyInterop.KeyFromVirtualKey(keyData);
                    KeyUp?.Invoke(this, new KeyboardHookEventArgs(key, CapsLockStatus));
                }
            }
            return IsHoldKey ? -1 : NativeMethods.CallNextHookEx(hHook, nCode, wParam, lParam);
        }
    }
    public class KeyboardHookEventArgs : EventArgs
    {
        /// <summary>
        /// 当前为大写锁定时 CapsLockStatus 为 false,否则为 True
        /// </summary>
        public bool CapsLockStatus { get; set; } = false;
        public Key Key { get; set; }

        public KeyboardHookEventArgs(Key key, bool CapsLockStatus)
        {
            Key = key;
            this.CapsLockStatus = CapsLockStatus;
        }
        public override string ToString()
        {
            return $"{Key.ToString()}{(CapsLockStatus ? 1 : 0)}{1}{Environment.NewLine}";
        }
    }
}