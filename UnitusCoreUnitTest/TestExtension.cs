using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitusCoreUnitTest
{
    public static class TestExtension
    {
        public static string CombineLines(this IEnumerable<string> args)
        {
            string ret = "";
            foreach (var s in args)
            {
                ret += s + "\n";
            }
            return ret;
        }
    }
}
