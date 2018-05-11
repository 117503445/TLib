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

        public static int Var_int { get; set; } = 2;
        public static string Var_string { get; set; } = "Hello";

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

        private async void BtnDebug2_Click(object sender, RoutedEventArgs e)
        {
            var i = await WdMessageBox.Display(content: "", Btn1text: "确认", Btn2text: "取消");
            Console.WriteLine(i);
        }

        private void BtnDebug3_Click(object sender, RoutedEventArgs e)
        {

        }
    }


}
