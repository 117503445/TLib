using System.Runtime.InteropServices;
namespace TLib.Windows
{
    [StructLayout(LayoutKind.Sequential)] //声明键盘钩子的封送结构类型
    public class KeyboardHookStruct
    {
        private int vkCode; //表示一个在1到254间的虚似键盘码 
        public int Flags { get; set; }
        public int Time { get; set; }

        public int DwExtraInfo { get; set; }
        public int ScanCode { get; set; }
        public int VkCode { get => vkCode; set => vkCode = value; }
    }
}