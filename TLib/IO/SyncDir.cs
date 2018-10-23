using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLib.IO
{
    /// <summary>
    /// 同步文件夹
    /// </summary>
    public static class SyncDir
    {
        //private static string sourceStr;
        //private static string destStr;
        //private static string backupStr;

        ///// <summary>
        ///// 同步的文件夹对
        ///// </summary>
        ///// <param name="sourceStr">例:D:\temp\source</param>
        ///// <param name="destStr"></param>
        ///// <param name="backupStr"></param>
        //public SyncDir()
        //{
        //    this.sourceStr = sourceStr;
        //    this.destStr = destStr;
        //    this.backupStr = backupStr;
        //}

        /// <summary>
        /// 高效的进行文件夹同步
        /// </summary>
        public static void Sync(string sourceStr, string destStr, string backupStr = "")
        {
            if (!Directory.Exists(sourceStr))
            {
                throw new ArgumentException("源路径不存在");

            }
            if (!Directory.Exists(destStr))
            {
                Directory.CreateDirectory(destStr);
            }
            BuildDirs(destStr, GetRelativePath(sourceStr, GetAllDirs(new DirectoryInfo(sourceStr))));
            CutDirs(sourceStr, destStr, GetRelativePath(destStr, GetAllDirs(new DirectoryInfo(destStr))));
            CopyFiles(sourceStr, destStr);
            CutFiles(sourceStr, destStr, backupStr);
        }

        /// <summary>
        /// 用原文件夹生成目标文件夹的所有文件夹
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="relativePaths"></param>
        public static void BuildDirs(string dest, List<string> relativePaths)
        {
            foreach (var item in relativePaths)
            {
                Directory.CreateDirectory(dest + item);
            }
        }
        /// <summary>
        /// 删除目标文件夹有,原文件夹没有的文件夹
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="relativePaths"></param>
        public static void CutDirs(string source, string dest, List<string> relativePaths)
        {

            foreach (var item in relativePaths)
            {
                if (!Directory.Exists(source + item))
                {
                    try
                    {
                        Directory.Delete(dest + item, true);
                    }
                    catch (Exception)
                    {
                    }

                }
            }
        }
        /// <summary>
        /// 原文件夹有的,目标文件夹没有的文件,复制
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public static async void CopyFiles(string source, string dest)
        {
            var i = GetAllFiles(new DirectoryInfo(source));
            foreach (var item in i)
            {
                string u = CutString(source, item.FullName);
                if (!FileEquals(source + u, dest + u))
                {
                    await UserIO.SafeCopy(source + u, dest + u);
                }
            }
        }
        /// <summary>
        /// 目标文件夹有,原文件夹没有的文件,删除
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="backupStr">例:D:\temp\backup</param>
        public static async void CutFiles(string source, string dest, string backupStr)
        {
            var i = GetAllFiles(new DirectoryInfo(dest));
            foreach (var item in i)
            {
                string u = CutString(dest, item.FullName);
                if (!FileEquals(source + u, dest + u))
                {
                    if (Directory.Exists(backupStr))
                    {
                        string x = backupStr + "\\" + item.Name.Substring(0, item.Name.Length - item.Extension.Length) + TimeStamp() + item.Extension;
                        await UserIO.SafeCopy(dest + u, x);
                    }
                    else if (backupStr != "")
                    {
                        throw new ArgumentException("备份路径不存在");
                    }
                    await UserIO.SafeDelete(dest + u);

                }
            }
        }

        public static string TimeStamp()
        {
            var t = DateTime.Now;
            return string.Format("{0},{1},{2},{3},{4}", t.Month, t.Day, t.Hour, t.Minute, t.Second);
        }
        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<DirectoryInfo> GetAllDirs(DirectoryInfo info)
        {
            return info.GetDirectories("*", SearchOption.AllDirectories).ToList();
        }
        /// <summary>
        /// 遍历文件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<FileInfo> GetAllFiles(DirectoryInfo info) { return info.GetFiles("*", SearchOption.AllDirectories).ToList(); }
        /// <summary>
        /// 字符串相减以得到相对路径.EXP:strLittle="a",strBig="ab",return b
        /// </summary>
        /// <param name="strLittle"></param>
        /// <param name="strBig"></param>
        /// <returns></returns>
        public static string CutString(string strLittle, string strBig) { return strBig.Substring(strLittle.Length); }
        /// <summary>
        /// 剥离DirectoryInfos,获取相对路径
        /// </summary>
        /// <param name="source"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static List<string> GetRelativePath(string source, List<DirectoryInfo> infos)
        {
            List<string> list = new List<string>();
            foreach (var item in infos)
            {
                list.Add(CutString(source, item.FullName));
            }
            return list;
        }
        /// <summary>
        /// 基于最后修改时间,大小判断文件是否相同
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool FileEquals(string file1, string file2)
        {
            FileInfo f1 = new FileInfo(file1);
            FileInfo f2 = new FileInfo(file2);
            if (!f1.Exists | !f2.Exists)
            {
                return false;
            }
            return f1.LastWriteTime == f2.LastWriteTime && f1.Length == f2.Length;
        }
    }
}
