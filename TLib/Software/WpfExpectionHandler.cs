using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TLib.UI.WpfMessageBox;

namespace TLib.Software
{
    /// <summary>
    /// WPF程序的异常处理
    /// </summary>
    public static class WpfExpectionHandler
    {
        /// <summary>
        /// 对捕获的异常进行额外处理
        /// </summary>
        public static event EventHandler<Exception> ExpectionCatched;
        /// <summary>
        /// WPF_ExpectionHandler.HandleExpection(Current,AppDomain.CurrentDomain);
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appDomain"></param>
        public static void HandleExpection(Application app, AppDomain appDomain)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (appDomain == null)
            {
                throw new ArgumentNullException(nameof(appDomain));
            }
            app.DispatcherUnhandledException += App_DispatcherUnhandledException;
            appDomain.UnhandledException += AppDomain_UnhandledException;
        }
        private static async void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is Exception exception)
                {
                    Logger.WriteException(exception, "试图恢复非UI异常");
                    ExpectionCatched?.Invoke(null, exception);
                }
            }
#pragma warning disable CA1031 // 不捕获常规异常类型
            catch (Exception)
#pragma warning restore CA1031 // 不捕获常规异常类型
            {
                Logger.WriteLine("Error,不可恢复的非UI异常");
                await WdMessageBox.Display("消息", "很遗憾,我们遇到了一个无法挽回的错误,程序即将关闭,希望联系开发人员以改善程序质量", "确认").ConfigureAwait(false);
            }
        }
        private static async void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Logger.WriteException(e.Exception, "试图恢复UI异常");
                e.Handled = true;
                ExpectionCatched?.Invoke(null, e.Exception);
            }
#pragma warning disable CA1031 // 不捕获常规异常类型
            catch (Exception)
#pragma warning restore CA1031 // 不捕获常规异常类型
            {
                Logger.WriteLine("Error,不可恢复的UI异常", "Bad");
                await WdMessageBox.Display("消息", "很遗憾,我们遇到了一个无法挽回的错误,程序即将关闭,希望联系开发人员以改善程序质量", "确认").ConfigureAwait(false);
            }
        }
    }
}
