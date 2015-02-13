using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using UnitusCore.Controllers;
using UnitusCore.Results;

namespace UnitusCore.Util
{
    public class AjaxRequestModelBase
    {
        //TODO AntiFogery should be enabled at Release
        public static bool NoCheckAntiFogery = true;
        public string ValidationToken { get; set; }

        public async Task<IHttpActionResult> OnValidToken<T>(UnitusApiController controller,T arg,Func<T,IHttpActionResult> f) where T :AjaxRequestModelBase
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
        public static async Task<IHttpActionResult> OnValidToken<T>(this UnitusApiController controller, T arg,
            Func<T, IHttpActionResult> f) where T : AjaxRequestModelBase
        {
            return await arg.OnValidToken(controller, arg, f);
        }

        public static async Task<IHttpActionResult> OnValidToken<T>(this UnitusApiController controller, T arg,
            Func<T, IHttpActionResult> f,Func<T,HashSet<string>,bool> vFunc) where T : AjaxRequestModelBase
        {
            return await arg.OnValidToken(controller, arg, (r) =>
            {
                HashSet<string> err = new HashSet<string>();
                if (vFunc(r, err))
                {
                    return f(r);
                }
                else
                {
                    string errorMsg = "";
                    foreach (var str in err)
                    {
                        errorMsg += str + "\n";
                    }
                    return controller.JsonResult(new ResultContainer() { Success = false, ErrorMessage = errorMsg });
                }
            });
        }

        public static string GetAjaxValidToken()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null,out cookieToken,out formToken);
            return cookieToken + ":" + formToken;
        }

        public static async Task<IHttpActionResult> OnValidToken(this UnitusApiController controller, string validationToken,Func<IHttpActionResult> f)
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

        public static async Task<IHttpActionResult> OnValidToken(this UnitusApiController controller, string validationToken, Func<IHttpActionResult> f,Func<HashSet<string>,bool> vFunc)
        {
            return await OnValidToken(controller,validationToken, () =>
            {
                HashSet<string> ErrorSet=new HashSet<string>();
                if (vFunc(ErrorSet))
                {
                    return f();
                }
                else
                {
                    string errorMsg = "";
                    foreach (var str in ErrorSet)
                    {
                        errorMsg += str + "\n";
                    }
                    return controller.JsonResult(new ResultContainer() {Success = false, ErrorMessage = errorMsg});
                }
            });
        }
    }
}