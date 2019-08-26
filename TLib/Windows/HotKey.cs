using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace TLib.Windows
{
    /// <summary>
    /// 注册热键
    /// </summary>
    public sealed class HotKey : IDisposable
    {
        /// <summary>
        /// 热键被触发
        /// </summary>
        public event Action<HotKey> HotKeyPressed;
        /// <summary>
        /// 标识 Hotkey
        /// </summary>
        private readonly int _id;
        public bool IsKeyRegistered { get; set; }
        /// <summary>
        /// 在对象初始化时注册热键
        /// </summary>
        /// <param name="modifierKeys"></param>
        /// <param name="key"></param>
        public HotKey(ModifierKeys modifierKeys, Key key)
        {
            Contract.Requires(modifierKeys != ModifierKeys.None || key != Key.None);
            Key = key;
            KeyModifier = modifierKeys;
            _id = GetHashCode();
            RegisterHotKey();
            ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;
        }

        ~HotKey()
        {
            Dispose();
        }
        public Key Key { get; private set; }
        public ModifierKeys KeyModifier { get; private set; }
        /// <summary>
        /// 注册热键
        /// </summary>
        public void RegisterHotKey()
        {
            //重新进行注册
            if (IsKeyRegistered)
            {
                UnregisterHotKey();
            }
            IsKeyRegistered = NativeMethods.RegisterHotKey(IntPtr.Zero, _id, KeyModifier, KeyInterop.VirtualKeyFromKey(Key));

            //如果注册失败,说明热键已经被占用
            if (!IsKeyRegistered)
            {
                throw new ApplicationException("Hotkey already in use");
            }
        }
        /// <summary>
        /// 取消注册
        /// </summary>
        public void UnregisterHotKey()
        {
            IsKeyRegistered = !NativeMethods.UnregisterHotKey(IntPtr.Zero, _id);
        }
        public void Dispose()
        {
            ComponentDispatcher.ThreadPreprocessMessage -= ThreadPreprocessMessageMethod;
            UnregisterHotKey();
            GC.SuppressFinalize(this);
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
                if (msg.message == NativeMethods.WmHotKey && (int)(msg.wParam) == _id)
                {
                    HotKeyPressed?.Invoke(this);
                    handled = true;
                }
            }
        }
    }
}