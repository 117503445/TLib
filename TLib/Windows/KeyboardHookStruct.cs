using System.Runtime.InteropServices;
namespace TLib.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    public class KeyboardHookStruct
    {
        public int VkCode { get; set; }
        public int ScanCode { get; set; }
        public int Flags { get; set; }
        public int Time { get; set; }
        public int DwExtraInfo { get; set; }
    }
}