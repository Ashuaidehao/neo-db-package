using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo
{
    public static class Extensions
    {
        /// <summary>
        /// text string is null or whitespaces=>true
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNull(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }


        /// <summary>
        /// text string is not null nor whitespaces=>true
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool NotNull(this string text)
        {
            return !IsNull(text);
        }
    }
}
