# TLib
> 117503445's C# library.

117503445 的个人 C# 类库，分为 IO, Software, UI, Windows 四大类

TLib
    IO
        SyncDir
        高效的进行文件夹同步
            public static void Sync(string dir_source, string dir_dest, string dir_backup = "")
            高效的进行文件夹同步,避免目标文件夹已经有源文件夹的文件时,仍删除再复制

            public static List<DirectoryInfo> GetAllDirs(DirectoryInfo info)
            遍历文件夹

            public static List<FileInfo> GetAllFiles(DirectoryInfo info)
            遍历文件

        TIO
        TLib 对 File.IO 进行的封装
            public static void CopyFolder(string sourcePath, string destPath)
            基于遍历的文件夹复制

            public static async Task<bool> SafeCopy(string sourceFilePath, string destFilePath, int waitTime = 200, int n = 3)
            安全复制,不断尝试复制文件

            public static async Task<bool> SafeDelete(string path, int waitTime = 200, int n = 3)
            安全删除,不断尝试删除文件

        UsbCopyer
            public UsbCopyer(string dirBackup, bool isDirectCopy, bool isUseNotifyIcon)
            dirBackup:备份至电脑硬盘的文件夹路径,Exp:"D:/temp/"
            isDirectCopy:直接拷贝模式,插上U盘直接开始拷贝
            isUseNotifyIcon:使用托盘
            
    Software
        Logger
        适用于各种 C# 程序的 Logger,基于XML
            public static void Write(object info, string type = "info")
            写入各种对象,并储存该对象的toString()的值

            public static void WriteException(Exception e, bool handled = true, string info = "")
            写入异常

        Serializer
        适用于各种 C# 程序的序列化类,基于XML,侵入性低,只能序列化公开字段
            public Serializer(object reference, string file_XML, List<string> lstVarName)
            reference:被序列化类的一个对象
            file_XML:XML文件的路径
            lstVarName:公开字段的字符串的列表

        TimeStamp
        生成yyyy-mm-dd-hh-mm-ss-misd式的时间戳
            public static string Now
            返回yyyy-mm-dd-hh-mm-ss-misd格式的当前时间
            public static string standardFormatDatetime(DateTime dateTime)
            返回yyyy-mm-dd-hh-mm-ss-misd格式的时间

        WPF_ExpectionHandler
        WPF程序的异常处理
            public static void HandleExpection(Application app, AppDomain appDomain)
            在 App.xaml 中,注册 Application_Startup,
            WPF_ExpectionHandler.HandleExpection(Current,AppDomain.CurrentDomain);
            即可对所有异常进行处理

            public static event EventHandler<Exception> ExpectionCatched;
            对捕获的异常进行额外处理
