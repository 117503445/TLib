using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IWshRuntimeLibrary;

namespace TLib.Windows
{
    public static class StartUp
    {

        public static void SetStartUp(string exeName)
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + exeName + ".lnk");
            //设置快捷方式的目标所在的位置(源程序完整路径) 
            shortcut.TargetPath = System.Windows.Forms.Application.ExecutablePath;
            //应用程序的工作目录 
            //当用户没有指定一个具体的目录时，快捷方式的目标应用程序将使用该属性所指定的目录来装载或保存文件。 
            shortcut.WorkingDirectory = Environment.CurrentDirectory;
            //目标应用程序窗口类型(1.Normal window普通窗口,3.Maximized最大化窗口,7.Minimized最小化) 
            shortcut.WindowStyle = 1;
            shortcut.Description = exeName + "_Ink";
            shortcut.Save();
        }

        public static void UnSetStartUp(string exeName)
        {
            System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + exeName + ".lnk");
        }
        public static bool IsStartUp(string exeName)
        {
            return System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + exeName + ".lnk");
        }
    }
}
