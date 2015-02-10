using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;

namespace UnitusCore.Util
{
    public class AjaxRequestModelBase
    {
        //TODO AntiFogery should be enabled at Release
        public static bool NoCheckAntiFogery = true;
        public string ValidationToken { get; set; }

        public async Task<IHttpActionResult> OnValidToken<T>(ApiController controller,T arg,Func<T,IHttpActionResult> f) where T :AjaxRequestModelBase
        {
            if (NoCheckAntiFogery) return f(arg);//for debug
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

        public static async Task<IHttpActionResult> OnValidToken(this ApiController controller, string validationToken,Func<IHttpActionResult> f)
        {
            if (AjaxRequestModelBase.NoCheckAntiFogery) return f();//for debug
            string[] tokens = validationToken.Split(':');
            try
            {
                AntiForgery.Validate(tokens[0].Trim(), tokens[1].Trim());
                return f();
            }
            catch (Exception)
            {
                return new StatusCodeResult(HttpStatusCode.BadRequest, controller);
            }
        }
    }
}