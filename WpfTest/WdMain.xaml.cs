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
using TLib.UI.WpfMessageBox;
namespace WpfTest
{
    /// <summary>
    /// WdMain.xaml 的交互逻辑
    /// </summary>
    public partial class WdMain : Window
    {
        public WdMain()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateButtons(10);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="nums"></param>
        private void CreateButtons(int nums)
        {
            int buttonWidth = 100;
            for (int i = 0; i < nums; i++)
            {
                Button button = new Button
                {
                    Content = i,
                    Tag = i,
                    Width = buttonWidth
                };
                button.Click += BtnTest_Click;
                SpMain.Children.Add(button);
            }
            Width = buttonWidth * nums;
        }

        private async void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)sender).Tag.ToString());
            Console.WriteLine($"TestIndex={index}");
            switch (index)
            {
                case 0:
                    var result = await WdMessageBox.Display("消息", "消息", "OK");
                    Console.WriteLine(result);
                    break;
                default:
                    Console.WriteLine("未提供测试方法");
                    break;
            }
        }

    }
}
