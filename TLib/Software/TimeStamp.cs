using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLib.Software
{
    public static class TimeStamp
    {
        public static string Now
        {
            get
            {
                var d = DateTime.Now.ToLocalTime();

                return $"{d.Year}-{d.Month}-{d.Day}_{d.Hour}-{d.Minute}-{d.Second}-{d.Millisecond}";
            }
        }
    }
}
