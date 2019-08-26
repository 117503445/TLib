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

namespace TLib.UI.WpfMessageBox
{
    /// <summary>
    /// WdMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class WdMessageBox : Window
    {
        /// <summary>
        /// 初始化 WdMessageBox,一般建议使用静态的 Display 方法
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="Btn0text"></param>
        /// <param name="Btn1text"></param>
        /// <param name="Btn2text"></param>
        public WdMessageBox(string title = "消息", string content = "消息", string Btn0text = null, string Btn1text = null, string Btn2text = null)
        {
            InitializeComponent();
            Title = title;
            FmMain.Navigate(new PgMessageDefault(content));
            var texts = new string[] { Btn0text, Btn1text, Btn2text };
            var btns = new Button[] { Btn0, Btn1, Btn2 };
            for (int i = 0; i < texts.Length; i++)
            {
                if (string.IsNullOrEmpty(texts[i]))
                {
                    btns[i].Visibility = Visibility.Collapsed;
                }
                else
                {
                    btns[i].Content = texts[i];
                }
            }
        }
        /// <summary>
        /// 相当于 Message.Show
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="Btn0text"></param>
        /// <param name="Btn1text"></param>
        /// <param name="Btn2text"></param>
        /// <returns></returns>
        public static async Task<int> Display(string title = "消息", string content = "消息", string Btn0text = "", string Btn1text = "", string Btn2text = "")
        {
            WdMessageBox wd = new WdMessageBox(title, content, Btn0text, Btn1text, Btn2text);
            wd.Show();
            while (wd.Result == -1)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200)).ConfigureAwait(false);
            }
            wd.Dispatcher.Invoke(() =>
            {
                wd.Close();
            });
            return wd.Result;
        }
        /// <summary>
        /// 按钮选择结果
        /// </summary>
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
