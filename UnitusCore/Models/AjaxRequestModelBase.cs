using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace UnitusCore.Models
{
    public class AjaxRequestModelBase
    {
        public string ValidationToken { get; set; }

        public async Task<IHttpActionResult> OnValidToken<T>(ApiController controller,T arg,Func<T,IHttpActionResult> f) where T :AjaxRequestModelBase
        {
            string[] tokens = arg.ValidationToken.Split(':');
            try
            {
                AntiForgery.Validate(tokens[0].Trim(),tokens[1].Trim());
                return f(arg);
            }
            catch (Exception)
            {
                return new StatusCodeResult(HttpStatusCode.BadRequest,controller);
            }
        }
    }

    public static class AjaxRequestExtension
    {
        public static async Task<IHttpActionResult> OnValidToken<T>(this ApiController controller, T arg,
            Func<T, IHttpActionResult> f) where T : AjaxRequestModelBase
        {
            return await arg.OnValidToken(controller, arg, f);
        }

        public static string GetAjaxValidToken()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null,out cookieToken,out formToken);
            return cookieToken + ":" + formToken;
        }

    }
}