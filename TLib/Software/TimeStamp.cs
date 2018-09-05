using System;

namespace TLib.Software
{
    public static class TimeStamp
    {
        /// <summary>
        /// 返回yyyy-mm-dd-hh-mm-ss-misd格式的当前时间
        /// </summary>
        public static string Now
        {
            get
            {
                var d = DateTime.Now.ToLocalTime();
                return $"{d.Year}-{d.Month}-{d.Day}_{d.Hour}-{d.Minute}-{d.Second}-{d.Millisecond}";
            }
        }
        /// <summary>
        /// 返回yyyy-mm-dd-hh-mm-ss-misd格式的时间
        /// </summary>
        /// <returns></returns>
        public static string TLibFormat(DateTime dateTime)
        {
            var d = dateTime;
            return $"{d.Year}-{d.Month}-{d.Day}_{d.Hour}-{d.Minute}-{d.Second}-{d.Millisecond}";
        }
    }
}
