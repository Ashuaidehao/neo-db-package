using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo;

namespace MyNep5Tracker
{
    public static class Extensions
    {
        /// <summary>
        /// convert to hex string without "Ox"
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string ToHexString(this UIntBase address)
        {
            return address.ToArray().Reverse().ToHexString();
        }

        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromTimestampMS(this ulong timestamp)
        {
            return unixEpoch.AddMilliseconds(timestamp);
        }

    }
}
