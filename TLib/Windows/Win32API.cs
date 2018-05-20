using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLib.Windows
{
    public partial class Win32API
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);

        public static bool ShowWindowAsync(IntPtr hWnd, Win32WindowState windowState)
        {
            return ShowWindowAsync(hWnd, (int)windowState);
        }
    }
    public enum Win32WindowState
    {
        Hide = 0,
        Normal = 1,
        Maximize = 3,
        ShowNoActivate = 4,
        Show = 5,
        Minimize = 6,
        Restore = 9,
        Showdefault = 10
    }
}
