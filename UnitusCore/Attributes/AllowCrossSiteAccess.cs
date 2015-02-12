using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Http.Filters;

namespace UnitusCore.Attributes
{

    [Flags]
    public enum AccessFrom
    {
        LocalHost=0x01,
        Unitus=0x02,
        All=0x04
    }
}