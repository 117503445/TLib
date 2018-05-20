using System;
using TLib;
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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TLib.Software;
using TLib.UI.WPF_MessageBox;

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
            Serializer serializer = new Serializer(this, "1.xml", new List<string>() {  nameof(Var_list_People)});

            //nameof(Var_bool), nameof(D), nameof(Var_list_int),
        }
        //
        public static int Var_int { get; set; } = 2;
        public static string Var_string { get; set; } = "Hello";
        public static bool Var_bool { get; set; } = false;
        public static List<int> Var_list_int { get; set; } = new List<int> { 2, 3, 4, 5 };
        public static List<People> Var_list_People { get; set; } = new List<People>() { new People() { Age = 1 } };
        public static People Var_people { get; set; } = new People() { Age=23};
        public static SerializableDictionary<string, int> D { get; set; } = new SerializableDictionary<string, int>()
        {
            { "0",2}
            ,{ "1",4}
        };
        private void BtnDebug0_Click(object sender, RoutedEventArgs e)
        {
            Var_int += 1;
            Var_string = $"Hello:{DateTime.Now}";
            Var_bool = !Var_bool;
            Var_list_People[0].Age++;
            for (int i = 0; i < Var_list_int.Count; i++)
            {
                Var_list_int[i] += 1;
            }
            Var_people.Age++;
        }

        private void BtnDebug1_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(Var_int);
            //Console.WriteLine(Var_string);
            //Console.WriteLine(Var_bool);
            //Var_list_int.ForEach((item) =>
            //{
            //    Console.WriteLine(item);
            //});
            Console.WriteLine(Var_list_People[0].Age);
            //Console.WriteLine(Var_people.Age);
        }

        private async void BtnDebug2_Click(object sender, RoutedEventArgs e)
        {
            var i = await WdMessageBox.Display(content: "", Btn1text: "确认", Btn2text: "取消");
            Console.WriteLine(i);
        }

        private void BtnDebug3_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("233");
        }
    }
   [Serializable]
    public class People
    {
        public int Age { get; set; }
    }

}
