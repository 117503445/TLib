using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLib.Software;
using System.Xml.Linq;

namespace FtpTest
{
    class Program
    {
        public static List<TftpFileInfo> Infos { get; set; } = new List<TftpFileInfo>();
        static int n = 0;
        static void Main(string[] args)
        {
            try
            {
                var i = TDownLoadInfo.Loads("FileTree.xml");
                FtpDownLoaders ftpDownLoaders = new FtpDownLoaders(15, i);
                ftpDownLoaders.SetDownLoad(null, null);
                while (true)
                {
                    var k = Console.ReadLine();
                    if (k == "e")
                    {
                        TDownLoadInfo.Saves(ftpDownLoaders.LstDownLoad, "Last.xml");
                        return;
                    }
                    else
                    {
                        ftpDownLoaders.SetDownLoad(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
            }
            #region 获取文件树数据
            //FtpFolderInfo ftpFolderInfo = new FtpFolderInfo(new FtpBaseUri("192.168.8.215"));
            //Foreach(ftpFolderInfo);
            //TftpFileInfo.Save(Infos);
            #endregion
            #region 建立文件树
            //var dInfo = new List<TDownLoadInfo>();

            //foreach (var item in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/File/"))
            //{
            //    var x = TftpFileInfo.Load(item);
            //    if (x != null)
            //    {
            //        // dInfo.Concat(x);
            //        foreach (var i in x)
            //        {
            //            dInfo.Add(TDownLoadInfo.SetFromTftpFileInfo(i));
            //        }
            //    }

            //}
            //TDownLoadInfo.Saves(dInfo, "1.xml");
            #endregion
            #region Sort
            //var i = TDownLoadInfo.Loads("FileTree.xml");
            //i.Sort();
            //TDownLoadInfo.Saves(i,"FileTree1.xml");
            #endregion
          

        }
        static void Foreach(FtpFolderInfo info)
        {
            n++;
            if (n > 10)
            {
                TftpFileInfo.Save(Infos);
                Infos.Clear();
                n = 0;
            }
            //Console.WriteLine(">>>>>>>>" + info.Uri);
            foreach (var item in info.GetFtpFiles())
            {
                var t = new TftpFileInfo();
                t.SetFromCHT(item);
                Infos.Add(t);
                Console.WriteLine("Add-->" + t.Url);
            }
            foreach (var item in info.GetFtpFolders())
            {
                Foreach(item);
            }

        }

    }


    public class FtpDownLoaders
    {
        public List<TDownLoadInfo> LstDownLoad { get; set; } = new List<TDownLoadInfo>();
        public List<DownLoader> DownLoaders { get; set; } = new List<DownLoader>();
        public FtpDownLoaders(int n, List<TDownLoadInfo> lst)
        {
            LstDownLoad = lst;
            for (int i = 0; i < n; i++)
            {
                var d = new DownLoader();
                d.DownLoadCompleted += SetDownLoad;
                DownLoaders.Add(d);
            }

        }
        public int index { get; set; } = 0;
        public async void SetDownLoad(object sender, TDownLoadInfo e)
        {
            await Task.Run(() =>
            {
                if (e != null)
                {
                    try
                    {
                        LstDownLoad[((DownLoader)sender).TaskIndex] = e;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }



                foreach (var item in DownLoaders)
                {
                    if (!item.DownLoading)
                    {
                        item.TaskIndex = index;
                        item.DownLoad(LstDownLoad[index]);
                        index++;
                        //TDownLoadInfo downLoadInfo = null;
                        //foreach (var dInfo in LstDownLoad)
                        //{
                        //    if (!dInfo.IsDownLoaded)
                        //    {
                        //        downLoadInfo = dInfo;
                        //        break;
                        //    }
                        //}
                        //if (downLoadInfo != null)
                        //{
                        //    await item.DownLoad(downLoadInfo);
                        //}
                    }
                }
            });


        }



    }

    public class DownLoader
    {
        public int TaskIndex { get; set; }
        public event EventHandler<TDownLoadInfo> DownLoadCompleted;
        public bool DownLoading { get; set; }
        public TDownLoadInfo DownLoadInfo { get; set; }
        public async void DownLoad(TDownLoadInfo downLoadInfo)
        {

            await Task.Run(() =>
            {
                this.DownLoadInfo = downLoadInfo;
                DownLoading = true;
                Console.WriteLine("DownLoad  " + downLoadInfo.Url);

                FtpFileInfo info = new FtpFileInfo();
                info.BaseUri = new FtpBaseUri() { ServerIP = "192.168.8.215" };
                info.DisplayName = DownLoadInfo.DisplayName;
                info.ParentUri = DownLoadInfo.ParentUri;

                //if (DownLoadInfo.Size > 2000000)
                //{
                //    Console.WriteLine("Skip  " + downLoadInfo.Url);
                //    DownLoadCompleted?.Invoke(this, DownLoadInfo);
                //    DownLoading = false;
                //    DownLoadInfo.IsDownLoaded = true;
                //    return;
                //}
                try
                {
                    info.Download(AppDomain.CurrentDomain.BaseDirectory + "DownLoad/");
                }
                catch (Exception)
                {
                    System.Threading.Thread.Sleep(3000);
                    try
                    {
                        info.Download(AppDomain.CurrentDomain.BaseDirectory + "DownLoad/");
                    }
                    catch (Exception ex)
                    {
                        DownLoadInfo.ExceptionInfo = ex.Message;
                    }
                }

                DownLoadCompleted?.Invoke(this, DownLoadInfo);
                DownLoadInfo.IsDownLoaded = true;
                DownLoading = false;
                Console.WriteLine("EndDownLoad  " + downLoadInfo.Url);
            });

        }
    }

    public class FileExtensions
    {
        public static string[] Pictures => new[] { ".jpg", ".png", ".bmp", ".jpeg", ".tif", ".tiff" };
    }


    public class TDownLoadInfo : TftpFileInfo, IComparable<TDownLoadInfo>
    {
        public static TDownLoadInfo SetFromTftpFileInfo(TftpFileInfo info)
        {
            TDownLoadInfo dInfo = new TDownLoadInfo
            {
                DisplayName = info.DisplayName,
                Extension = info.Extension,
                ParentUri = info.ParentUri,
                Size = info.Size,
                Url = info.Url
            };
            return dInfo;
        }


        public static void Saves(List<TDownLoadInfo> infos, string file_XML = "")
        {
            if (file_XML == "")
            {
                file_XML = AppDomain.CurrentDomain.BaseDirectory + "/File/" + TimeStamp() + ".xml";
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/File/");
            }
            using (FileStream fs = new FileStream(file_XML, FileMode.Create, FileAccess.Write))
            {
                //在进行XML序列化的时候，在类中一定要有无参数的构造方法(要使用typeof获得对象类型)
                XmlSerializer xml = new XmlSerializer(infos.GetType());
                xml.Serialize(fs, infos);
            }
        }
        public static List<TDownLoadInfo> Loads(string file_XML = "")
        {
            try
            {
                using (FileStream fs = new FileStream(file_XML, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(List<TDownLoadInfo>));

                    var x = (List<TDownLoadInfo>)xml.Deserialize(fs);
                    Console.WriteLine("-->" + DateTime.Now.ToLocalTime());
                    return x;
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Error");
                return null;
            }

        }

        public int CompareTo(TDownLoadInfo other)
        {
            return this.Size.CompareTo(other.Size);
        }

        public string ExceptionInfo { get; set; } = string.Empty;
        public bool IsDownLoaded { get; set; } = new bool();


    }



    [Serializable]
    public class TftpFileInfo
    {
        public static void Save(List<TftpFileInfo> infos, string file_XML = "")
        {
            if (file_XML == "")
            {
                file_XML = AppDomain.CurrentDomain.BaseDirectory + "/File/" + TimeStamp() + ".xml";
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/File/");
            }
            using (FileStream fs = new FileStream(file_XML, FileMode.Create, FileAccess.Write))
            {
                //在进行XML序列化的时候，在类中一定要有无参数的构造方法(要使用typeof获得对象类型)
                XmlSerializer xml = new XmlSerializer(infos.GetType());
                xml.Serialize(fs, infos);
            }
        }
        public static string TimeStamp()
        {
            var t = DateTime.Now;
            return string.Format("{0},{1},{2},{3},{4}", t.Month, t.Day, t.Hour, t.Minute, t.Second);
        }
        public static List<TftpFileInfo> Load(string file_XML)
        {
            try
            {
                using (FileStream fs = new FileStream(file_XML, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(List<TftpFileInfo>));

                    var x = (List<TftpFileInfo>)xml.Deserialize(fs);
                    Console.WriteLine("-->" + DateTime.Now.ToLocalTime());
                    return x;
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Error");
                return null;
            }

        }
        public void SetFromCHT(FtpFileInfo info)
        {
            ParentUri = info.ParentUri;
            DisplayName = info.DisplayName;
            //LastAccessTime = info.LastAccessTime;

            Size = info.Size;
            //Tag = info.Tag;
            Url = info.Uri;
            Extension = info.Extension;
        }
        public string ParentUri { get; set; }
        public string DisplayName { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public string Extension { get; set; }
    }
    public abstract class FtpFileSystemInfo
    {
        protected FtpFileSystemInfo()
        {
        }

        public FtpBaseUri BaseUri { get; internal set; }
        public abstract FtpFileType FileType { get; }
        public string ParentUri { get; internal set; }
        public string DisplayName { get; internal set; }
        public DateTime LastAccessTime { get; internal set; }
        public long Size { get; internal set; }
        public object Tag { get; set; }
        public string Uri
        {
            get
            {
                if (FileType == FtpFileType.File)
                {
                    return "ftp://" + BaseUri.ServerIP + "/" + ParentUri + DisplayName;
                }
                else
                {
                    return "ftp://" + BaseUri.ServerIP + "/" + ParentUri + DisplayName + "/";
                }
            }
        }



    }
    public class FtpFolderInfo : FtpFileSystemInfo
    {
        internal FtpFolderInfo()
        {
        }
        public FtpFolderInfo(FtpBaseUri baseUri, string relativeUri = "")
        {
            BaseUri = baseUri;
            if (relativeUri != "")
            {
                if (!(relativeUri.Last() == '\\' || relativeUri.Last() == '/'))
                {
                    relativeUri = relativeUri + "/";
                }
                relativeUri = relativeUri.Replace('\\', '/');
            }
            SplitUri(relativeUri, out string a, out string b);
            ParentUri = a; DisplayName = b;
        }

        public override FtpFileType FileType => FtpFileType.Folder;
        public FtpFileSystemInfo[] GetFtpFileSystemInfos()
        {
            try
            {
                List<FtpFileSystemInfo> infos = new List<FtpFileSystemInfo>();
                FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(new Uri(Uri));
                ftp.Credentials = new NetworkCredential(BaseUri.UserID, BaseUri.Password);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftp.UsePassive = false;

                using (WebResponse response = ftp.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    {
                        List<string> result = new List<string>();
                        while (!reader.EndOfStream)
                        {
                            result.Add(reader.ReadLine());
                        }

                        foreach (var item in result)
                        {
                            FtpFileSystemInfo ftpFileSystemInfo = TranslateTo(item);
                            if (ftpFileSystemInfo != null)
                            {
                                infos.Add(ftpFileSystemInfo);
                            }
                        }
                    }

                }

                return infos.ToArray();
            }
            catch (Exception)
            {
                return new FtpFileSystemInfo[0];
            }

        }
        public FtpFolderInfo[] GetFtpFolders() => GetFtpFileSystemInfos().OfType<FtpFolderInfo>().ToArray();
        public FtpFileInfo[] GetFtpFiles() => GetFtpFileSystemInfos().OfType<FtpFileInfo>().ToArray();
        /// <summary>
        /// 从此处递归文件夹的所有文件.
        /// </summary>
        /// <param name="action"></param>
        public async Task Foreach(Func<FtpFileInfo, Task> action)
        {
            foreach (var item in GetFtpFiles())
            {
                await action.Invoke(item);
            }
            foreach (var item in GetFtpFolders())
            {
                //向下递归.
                await item.Foreach(action);
            }
        }

        void SplitUri(string relativeUri, out string parentUri, out string displayName)
        {
            if (relativeUri == "")
            {
                parentUri = "";
                displayName = "";
            }
            else
            {
                string p = relativeUri.Remove(relativeUri.Length - 1);
                if (!p.Contains('/'))
                {
                    parentUri = "";
                    displayName = p;
                }
                else
                {
                    int index = p.LastIndexOf('/');
                    parentUri = p.Substring(0, index);
                    displayName = p.Substring(index + 1);
                }
            }

        }
        FtpFileSystemInfo TranslateTo(string info)
        {
            string[] infos = info.SplitAny(' ');
            if (infos[0] == "drw-rw-rw-")
            {
                if (infos[8] == "." || infos[8] == "..")
                {
                    return null;
                }
                else
                {
                    return new FtpFolderInfo()
                    {
                        BaseUri = this.BaseUri,
                        ParentUri = this.ParentUri + DisplayName + '/',
                        DisplayName = infos[8],
                        Size = 0
                    };
                }
            }
            else
            {
                return new FtpFileInfo()
                {
                    BaseUri = this.BaseUri,
                    ParentUri = this.ParentUri + DisplayName + '/',
                    DisplayName = infos[8],
                    Size = long.Parse(infos[4])
                };
            }
        }
    }
    public class FtpFileInfo : FtpFileSystemInfo
    {
        internal FtpFileInfo()
        {
        }

        public override FtpFileType FileType => FtpFileType.File;
        public string Extension => GetExtension(DisplayName);
        public void Download(string uri, bool isFullName = false)
        {
            string fileName;
            if (!isFullName)
            {
                fileName = uri + ParentUri + DisplayName;
            }
            else
            {
                fileName = uri;
            }
            string d = System.IO.Path.GetDirectoryName(fileName);
            if (!Directory.Exists(d))
            {
                Directory.CreateDirectory(d);
            }
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(Uri));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.UseBinary = true;
                request.UsePassive = false;
                request.Credentials = new NetworkCredential(BaseUri.UserID, BaseUri.Password);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream ftpstream = response.GetResponseStream())
                    {
                        long cl = response.ContentLength;
                        int bufferSize = 2048;
                        int readCount;
                        byte[] buffer = new byte[bufferSize];
                        readCount = ftpstream.Read(buffer, 0, bufferSize);
                        while (readCount > 0)
                        {
                            stream.Write(buffer, 0, readCount);
                            readCount = ftpstream.Read(buffer, 0, bufferSize);
                        }

                    }
                }
            }
        }
        private string GetExtension(string displayName)
        {
            if (displayName.Contains('.'))
            {
                string[] t = displayName.Split('.');
                return "." + t[t.Length - 1];
            }
            else
            {
                return "";
            }
        }
    }


    public class FtpBaseUri
    {
        public FtpBaseUri(string serverIP, string userID = "", string password = "")
        {
            ServerIP = serverIP;
            UserID = userID;
            Password = password;
        }
        public FtpBaseUri() { }
        public string ServerIP { get; set; } = "";
        public string UserID { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public enum FtpFileType
    {
        Folder,
        File
    }
    public static class Extension
    {
        public static string[] SplitAny(this string obj, params char[] separator)
        {
            List<string> result = new List<string>();
            int last = -1;
            for (int i = 0; i < obj.Length; i++)
            {
                char current = obj[i];
                if (separator.Contains(current))
                {
                    if (i - last > 1)
                    {
                        result.Add(obj.Substring(last + 1, i - last - 1));
                    }
                    last = i;
                }
            }
            result.Add(obj.Substring(last + 1));
            return result.ToArray();
        }
    }
}
