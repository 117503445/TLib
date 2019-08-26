using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLib.Software;
namespace TLib.IO
{
    /// <summary>
    /// 同步文件夹
    /// </summary>
    public static class SyncDir
    {
        /// <summary>
        /// 高效的进行文件夹同步
        /// </summary>
        public static void Sync(string dirSource, string dirDest, string dirBackup = "")
        {
            if (!Directory.Exists(dirSource))
            {
                throw new ArgumentException("源路径不存在");

            }
            if (!Directory.Exists(dirDest))
            {
                Directory.CreateDirectory(dirDest);
            }
            BuildDirs(dirDest, GetRelativePath(dirSource, GetAllDirs(new DirectoryInfo(dirSource))));
            CutDirs(dirSource, dirDest, GetRelativePath(dirDest, GetAllDirs(new DirectoryInfo(dirDest))));
            CopyFiles(dirSource, dirDest);
            CutFiles(dirSource, dirDest, dirBackup);
        }

        /// <summary>
        /// 用原文件夹生成目标文件夹的所有文件夹
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="relativePaths"></param>
        private static void BuildDirs(string dest, List<string> relativePaths)
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
        private static void CutDirs(string source, string dest, List<string> relativePaths)
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
        private static async void CopyFiles(string source, string dest)
        {
            var i = GetAllFiles(new DirectoryInfo(source));
            foreach (var item in i)
            {
                string u = CutString(source, item.FullName);
                if (!FileEquals(source + u, dest + u))
                {
                    await TIO.SafeCopy(source + u, dest + u).ConfigureAwait(false);
                }
            }
        }
        /// <summary>
        /// 目标文件夹有,原文件夹没有的文件,删除
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="backupStr">例:D:\temp\backup</param>
        private static async void CutFiles(string source, string dest, string backupStr)
        {
            var i = GetAllFiles(new DirectoryInfo(dest));
            foreach (var item in i)
            {
                string u = CutString(dest, item.FullName);
                if (!FileEquals(source + u, dest + u))
                {
                    if (Directory.Exists(backupStr))
                    {
                        string x = backupStr + "\\" + item.Name.Substring(0, item.Name.Length - item.Extension.Length) + TimeStamp.Now + item.Extension;
                        await TIO.SafeCopy(dest + u, x).ConfigureAwait(false);
                    }
                    else if (string.IsNullOrEmpty(backupStr))
                    {
                        throw new ArgumentException("备份路径不存在");
                    }
                    await TIO.SafeDelete(dest + u).ConfigureAwait(false);

                }
            }
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
        public static List<FileInfo> GetAllFiles(DirectoryInfo info)
        {

            return info.GetFiles("*", SearchOption.AllDirectories).ToList();
        }
        /// <summary>
        /// 字符串相减以得到相对路径.EXP:strLittle="a",strBig="ab",return b
        /// </summary>
        /// <param name="strLittle"></param>
        /// <param name="strBig"></param>
        /// <returns></returns>
        private static string CutString(string strLittle, string strBig) { return strBig.Substring(strLittle.Length); }
        /// <summary>
        /// 剥离DirectoryInfos,获取相对路径
        /// </summary>
        /// <param name="source"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        private static List<string> GetRelativePath(string source, List<DirectoryInfo> infos)
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
        private static bool FileEquals(string file1, string file2)
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
