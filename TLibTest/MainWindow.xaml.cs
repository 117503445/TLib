﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
using System.Xml.Serialization;

namespace TLibTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Serializer serializer = new Serializer(this, "1.xml", new List<string>() { nameof(Var_int), nameof(Var_string) });
            //serializer.LstVarName.AddRange(new string[] { nameof(Var_int), nameof(Var_string) });
            //serializer.Add(ref var_int);
            //serializer.List.Add(ref var_int);
            //ref var_string
        }

        public int Var_int { get; set; } = 2;
        public string Var_string { get; set; } = "Hello";

        private void BtnDebug0_Click(object sender, RoutedEventArgs e)
        {
            Var_int += 1;
            Var_string = $"Hello:{DateTime.Now}";
        }

        private void BtnDebug1_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(Var_int);
            Console.WriteLine(Var_string);
        }

        private void BtnDebug2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDebug3_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    /// <summary>
    /// 无污染无公害的序列化器
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public List<string> LstVarName { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public List<object> LstOldValue { get; set; }
        public Dictionary<string, object> Variable { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 引用,exp:MainWindow
        /// </summary>
        private readonly object reference;
        /// <summary>
        /// XML文件路径
        /// </summary>
        private readonly string file_XML = string.Empty;
        /// <summary>
        /// 创建序列化器,并通过Load方法加载已保存的值
        /// </summary>
        /// <param name="reference">填写this以传递引用</param>
        /// <param name="file_XML">XML文件路径</param>
        /// <param name="lstVarName">属性列表</param>
        public Serializer(object reference, string file_XML, List<string> lstVarName)
        {
            this.file_XML = file_XML;
            this.reference = reference;
            LstVarName = lstVarName;
            LstOldValue = new List<object>();
            Load();
            DispatcherTimer timer = new DispatcherTimer
            {
                IsEnabled = true,
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (s, e) =>
            {
                Type type = reference.GetType();
                for (int i = 0; i < LstVarName.Count; i++)
                {
                    PropertyInfo pi = type.GetProperty(LstVarName[i]);
                    object value = pi.GetValue(reference, null);
                    if (LstOldValue.Count < i + 1)
                    {
                        LstOldValue.Add(value);
                    }
                    else
                    {
                        if (!LstOldValue[i].Equals(value))
                        {
                            LstOldValue[i] = value;
                            Save();
                            //Console.WriteLine($"Change!value={value}");
                        }
                        else
                        {
                            LstOldValue[i] = value;
                        }

                    }
                }

            };
        }
        /// <summary>
        /// 保存至XML文件
        /// </summary>
        public void Save()
        {
            using (FileStream fs = new FileStream(file_XML, FileMode.Create, FileAccess.Write))
            {
                //在进行XML序列化的时候，在类中一定要有无参数的构造方法(要使用typeof获得对象类型)
                XmlSerializer xml = new XmlSerializer(typeof(List<object>));
                xml.Serialize(fs, LstOldValue);
            }
        }
        /// <summary>
        /// 从XML文件读取
        /// </summary>
        public void Load()
        {
            if (!File.Exists(file_XML))
            {
                return;
            }
            using (FileStream fs = new FileStream(file_XML, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<object>));
                LstOldValue = (List<object>)xml.Deserialize(fs);
            }
            for (int i = 0; i < LstVarName.Count; i++)
            {
                Type type = reference.GetType();
                PropertyInfo pi = type.GetProperty(LstVarName[i]);
                object value = pi.GetValue(reference, null);
                pi.SetValue(reference, LstOldValue[i]);
            }
        }
    }
}
