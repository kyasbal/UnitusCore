using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitusCore.Util
{
    public static class DateTimeHelper
    {
        public static readonly DateTime UNIX_EPOCH=new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);
        public static long ToUnixTime(this DateTime date)
        {
            double nowTick = (date.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
            return (long) nowTick;
        }

        public static long ToDateCode(this DateTime date)
        {
            DateTime timeEliminated=new DateTime(date.Year,date.Month,date.Day,0,0,0);
            return ToUnixTime(timeEliminated);
        }
    }
}