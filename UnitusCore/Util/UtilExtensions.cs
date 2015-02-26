using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace UnitusCore.Util
{
    public static class UtilExtensions
    {
        public static Guid ToValidGuid(this string guidSource)
        {
            Guid result;
            if (!Guid.TryParse(guidSource, out result))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            else
            {
                return result;
            }
        }

        public static string ToHashCode(this string source)
        {
            MD5CryptoServiceProvider md5=new MD5CryptoServiceProvider();
            byte[] hash=md5.ComputeHash(Encoding.Unicode.GetBytes(source));
            md5.Clear();
            return BitConverter.ToString(hash).ToLower().Replace("-", "");
        }

        public static string ToCommaDividedString(this IEnumerable<string> strs)
        {
            string result = "";
            var enumerator = strs.GetEnumerator();
            bool hasNext = enumerator.MoveNext();
            while (hasNext)
            {
                result += enumerator.Current;
                hasNext = enumerator.MoveNext();
                if (hasNext)
                {
                    result += ",";
                }
            }
            return result;
        }

        private static Dictionary<string, string> ReplaceTables = new Dictionary<string, string>() { { "#", "Sharp" }, { "/", "Slash" } }; 

        public static string ToSafeForTable(this string args)
        {
            foreach (KeyValuePair<string, string> replacePair in ReplaceTables)
            {
                args = args.Replace(replacePair.Key, replacePair.Value);
            }
            return args;
        }
    }
}