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
using System.Windows.Shapes;

namespace TLib
{
    /// <summary>
    /// WdMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class WdMessageBox : Window
    {
        public WdMessageBox(string title = "消息", string content = "消息", string Btn0text = "", string Btn1text = "", string Btn2text = "")
        {
            InitializeComponent();
            Title = title;
            FmMain.Navigate(new PgMessageDefault(content));
            if (Btn0text == "")
            {
                Btn0.Visibility = Visibility.Collapsed;
            }
            else
            {
                Btn0.Content = Btn0text;
            }
            if (Btn1text == "")
            {
                Btn1.Visibility = Visibility.Collapsed;
            }
            else
            {
                Btn1.Content = Btn1text;
            }
            if (Btn2text == "")
            {
                Btn2.Visibility = Visibility.Collapsed;
            }
            else
            {
                Btn2.Content = Btn2text;
            }

        }
        public static async Task<int> Display(string title = "消息", string content = "消息", string Btn0text = "", string Btn1text = "", string Btn2text = "")
        {
            WdMessageBox wd = new WdMessageBox(title, content, Btn0text, Btn1text, Btn2text);
            wd.Show();
            while (wd.Result == -1)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }
            wd.Close();
            return wd.Result;
        }

        public int Result { get; set; } = -1;

        private void Btn0_Click(object sender, RoutedEventArgs e)
        {
            Result = 0;
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            Result = 1;
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            Result = 2;
        }
    }
}
