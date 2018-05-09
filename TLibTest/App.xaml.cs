using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TLibTest
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public int Var_int { get; set; } = 2;
        public string Var_string { get; set; } = "Hello";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Serializer serializer = new Serializer(this, "App.xml", new List<string>() { nameof(Var_int), nameof(Var_string) });
        }
    }
}
