using System;

namespace TLib.Software
{
    /// <summary>
    /// 生成yyyy-mm-dd-hh-mm-ss-misd式的时间戳
    /// </summary>
    public static class TimeStamp
    {
        /// <summary>
        /// 返回yyyy-mm-dd-hh-mm-ss-misd格式的当前时间
        /// </summary>
        public static string Now
        {
            get
            {
                return standardFormatDatetime(DateTime.Now.ToLocalTime());
            }
        }
        /// <summary>
        /// 返回yyyy-mm-dd-hh-mm-ss-misd格式的时间
        /// </summary>
        /// <returns></returns>
        public static string standardFormatDatetime(DateTime dateTime)
        {
            var d = dateTime;
            return $"{d.Year}-{d.Month}-{d.Day}_{d.Hour}-{d.Minute}-{d.Second}-{d.Millisecond}";
        }
    }
}
