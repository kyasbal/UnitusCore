using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace UnitusCore.Attributes
{
    public class AllowCrossSiteAccessAttribute : ActionFilterAttribute
    {
        private static readonly int[] AllowLocalPorts =new int[]
        {
            8888,3672
        };

        private AccessFrom from;

        public AllowCrossSiteAccessAttribute(AccessFrom accessFrom)
        {
            this.from = accessFrom;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            string fromString = "";
            if (from.HasFlag(AccessFrom.All))
            {
                fromString = "*";
            }
            else
            {
                if (from.HasFlag(AccessFrom.Unitus))
                {
                    fromString += "http://unitus.azurewebsites.com ";
                }
                if(from.HasFlag(AccessFrom.LocalHost))
                {
                    foreach (var port in AllowLocalPorts)
                    {
                        fromString += "http://localhost:"+port+" ";
                    }
                }
            }
            actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin",fromString);
            base.OnActionExecuted(actionExecutedContext);
        }
    }
    [Flags]
    public enum AccessFrom
    {
        LocalHost=0x01,
        Unitus=0x02,
        All=0x04
    }
}