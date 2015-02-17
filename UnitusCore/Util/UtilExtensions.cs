using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    }
}