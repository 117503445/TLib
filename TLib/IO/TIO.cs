using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TLib.IO
{
    public static class TIO
    {
        /// <summary>
        /// 基于遍历的文件夹复制
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        public static void CopyFolder(string sourcePath, string destPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    //目标目录不存在则创建
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("创建目标目录失败：" + ex.Message);
                    }
                }
                //获得源文件下所有文件

                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    File.Copy(c, destFile, true);//覆盖模式
                });
                //获得源文件下所有目录文件
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
                foreach (var item in folders)
                {
                    Console.WriteLine(item);
                }
                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    //采用递归的方法实现
                    try
                    {
                        CopyFolder(c, destDir);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
            }
            else
            {
                throw new DirectoryNotFoundException(sourcePath);
            }
        }

        /// <summary>
        /// 安全复制,不断尝试复制文件
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destFilePath"></param>
        /// <param name="waitTime">每次尝试复制以后等待时间,毫秒</param>
        /// <param name="n">次数</param>
        /// <returns></returns>
        public static async Task<bool> SafeCopy(string sourceFilePath, string destFilePath, int waitTime = 200, int n = 3)
        {
            if (!File.Exists(sourceFilePath))
            {
#if DEBUG
                throw new DirectoryNotFoundException(sourceFilePath);
#endif
            }
            bool isSuccess = false;
            await Task.Run(() =>
            {
                for (int i = 0; i < n; i++)
                {
                    try
                    {
                        File.Copy(sourceFilePath, destFilePath, true);
                        isSuccess = true;
                        break;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(waitTime);
                    }
                    if (i == n - 1)
                    {
                        isSuccess = false;
                    }
                }


            }).ConfigureAwait(false);
            return isSuccess;
        }
        /// <summary>
        /// 安全删除,不断尝试删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="waitTime">每次尝试复制以后等待时间,毫秒</param>
        /// <param name="n">次数</param>
        /// <returns></returns>
        public static async Task<bool> SafeDelete(string path, int waitTime = 200, int n = 3)
        {
            if (!File.Exists(path))
            {
#if DEBUG
                throw new DirectoryNotFoundException(path);
#endif
            }
            bool isSuccess = false;
            await Task.Run(() =>
            {
                for (int i = 0; i < n; i++)
                {
                    try
                    {
                        File.Delete(path);
                        isSuccess = true;
                        break;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(waitTime);
                    }
                    if (i == n - 1)
                    {
                        isSuccess = false;
                    }
                }
            }).ConfigureAwait(false);
            return isSuccess;
        }
    }
}
