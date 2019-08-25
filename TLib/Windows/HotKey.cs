using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace TLib.Windows
{
    /// <summary>
    /// 注册热键
    /// </summary>
    public sealed class HotKey : IDisposable
    {
        public event Action<HotKey> HotKeyPressed;
        private readonly int _id;
        public bool IsKeyRegistered { get; set; }

        readonly IntPtr _handle;
        public HotKey(ModifierKeys modifierKeys, Keys key, Window window)
            : this(modifierKeys, key, new WindowInteropHelper(window))
        {
            Contract.Requires(window != null);
        }
        public HotKey(ModifierKeys modifierKeys, Keys key, WindowInteropHelper window)
            : this(modifierKeys, key, window.Handle)
        {
            Contract.Requires(window != null);
        }
        public HotKey(ModifierKeys modifierKeys, Keys key, IntPtr windowHandle)
        {
            Contract.Requires(modifierKeys != ModifierKeys.None || key != Keys.None);
            Contract.Requires(windowHandle != IntPtr.Zero);
            Key = key;
            KeyModifier = modifierKeys;
            _id = GetHashCode();
            _handle = windowHandle;
            RegisterHotKey();
            ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;
        }
        ~HotKey()
        {
            Dispose();
        }
        public Keys Key { get; private set; }
        public ModifierKeys KeyModifier { get; private set; }
        public void RegisterHotKey()
        {
            if (Key == Keys.None)
                return;
            if (IsKeyRegistered)
                UnregisterHotKey();
            IsKeyRegistered = HotKeyAPI.RegisterHotKey(_handle, _id, KeyModifier, Key);
            if (!IsKeyRegistered)
                throw new ApplicationException("Hotkey already in use");
        }
        public void UnregisterHotKey()
        {
            IsKeyRegistered = !HotKeyAPI.UnregisterHotKey(_handle, _id);
        }
        public void Dispose()
        {
            ComponentDispatcher.ThreadPreprocessMessage -= ThreadPreprocessMessageMethod;
            UnregisterHotKey();
        }
        /// <summary>
        /// 热键被按下时第一个触发的方法
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="handled"></param>
        private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == HotKeyAPI.WmHotKey && (int)(msg.wParam) == _id)
                {
                    HotKeyPressed?.Invoke(this);
                    handled = true;
                }
            }
        }
    }
    public class HotKeyAPI
    {
        public const int WmHotKey = 0x0312;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, Keys vk);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
