using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TLib.UI.WPF_MessageBox;

namespace TLib.Software
{
    /// <summary>
    /// 把你交给我,我帮你处理异常
    /// </summary>
    public static class WPF_ExpectionHandler
    {
        public static event EventHandler<Exception> ExpectionCatched;
        /// <summary>
        /// WPF_ExpectionHandler.HandleExpection(Current,AppDomain.CurrentDomain);
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appDomain"></param>
        public static void HandleExpection(Application app, AppDomain appDomain)
        {
            app.DispatcherUnhandledException += App_DispatcherUnhandledException;
            appDomain.UnhandledException += AppDomain_UnhandledException;
        }
        private static async void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is Exception exception)
                {
                    Logger.WriteException(exception, false, "试图恢复非UI异常");
                    ExpectionCatched?.Invoke(null, exception);
                }

            }
            catch (Exception)
            {
                Logger.Write("Error,不可恢复的非UI异常", "Bad");
                await WdMessageBox.Display("消息", "很遗憾,我们遇到了一个无法挽回的错误,程序即将关闭,希望联系开发人员以改善程序质量", Btn2text: "确认");
            }
        }
        private static async void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Logger.WriteException(e.Exception, false, "试图恢复UI异常");
                e.Handled = true;
                ExpectionCatched?.Invoke(null, e.Exception);
            }
            catch (Exception)
            {
                Logger.Write("Error,不可恢复的UI异常", "Bad");
                await WdMessageBox.Display("消息", "很遗憾,我们遇到了一个无法挽回的错误,程序即将关闭,希望联系开发人员以改善程序质量", Btn2text: "确认");
            }
        }
    }
}
