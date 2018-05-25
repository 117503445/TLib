using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TLib.Software;

using System.IO;

using System.Net;
using System.Xml.Serialization;

namespace Wpf_ftp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<TftpFileInfo> Infos { get; set; } = new List<TftpFileInfo>();
        static int n = 0;
        public MainWindow()
        {
            InitializeComponent();

        }
        void Foreach(FtpFolderInfo info)
        {
            n++;
            if (n > 2)
            {
                return;
            }
            //Console.WriteLine(">>>>>>>>" + info.Uri);
            foreach (var item in info.GetFtpFiles())
            {
                var t = new TftpFileInfo();
                t.SetFromCHT(item);
                Infos.Add(t);
                Console.WriteLine(item.Uri);
                Console.WriteLine(item.Size);
                //if (FileExtensions.Pictures.Contains(item.Extension.ToLower()))
                //{
                //    item.Download(AppDomain.CurrentDomain.BaseDirectory + "Pic/");
                //    Console.WriteLine("download");

                //}
            }
            foreach (var item in info.GetFtpFolders())
            {
                if (n > 2)
                {
                    return;
                }
                Foreach(item);

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Serializer serializer = new Serializer(this, "1.xml", new List<string>() { nameof(Infos) });
            FtpFolderInfo ftpFolderInfo = new FtpFolderInfo(new FtpBaseUri("192.168.8.215"));
            Foreach(ftpFolderInfo);
            TftpFileInfo.Save("1.xml",Infos);

        }
    }

    public class FileExtensions
    {
        public static string[] Pictures => new[] { ".jpg", ".png", ".bmp", ".jpeg", ".tif", ".tiff" };
    }
    [Serializable]
    public class TftpFileInfo
    {
        public static void Save(string file_XML, List<TftpFileInfo> infos)
        {
            using (FileStream fs = new FileStream(file_XML, FileMode.Create, FileAccess.Write))
            {
                //在进行XML序列化的时候，在类中一定要有无参数的构造方法(要使用typeof获得对象类型)
                XmlSerializer xml = new XmlSerializer(infos.GetType());
                xml.Serialize(fs, infos);
            }
        }
        public static List<TftpFileInfo> Load(string file_XML)
        {
            using (FileStream fs = new FileStream(file_XML, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer xml = new XmlSerializer(typeof(TftpFileInfo));
                return  (List<TftpFileInfo>)xml.Deserialize(fs);
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
        //public DateTime LastAccessTime { get; set; }
        public long Size { get; set; }
        //public object Tag { get; set; }
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
        public string ServerIP { get; private set; }
        public string UserID { get; private set; }
        public string Password { get; private set; }
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
