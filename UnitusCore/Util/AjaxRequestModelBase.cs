using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using AutoMapper;
using UnitusCore.Controllers;
using UnitusCore.Results;

namespace UnitusCore.Util
{
    public class AjaxRequestModelBase
    {
        //TODO AntiFogery should be enabled at Release
        public static bool NoCheckAntiFogery = true;
        public string ValidationToken { get; set; }

        private bool TryGetModelError(UnitusApiController controller,out IHttpActionResult response)
        {
            if (controller.ModelState.IsValid)
            {
                response = null;
                return false;
            }
            else
            {
                IEnumerable<object> validationObjects = controller.ModelState.Keys.Select((key) =>
                {
                    ModelState value;
                    object stateInfo = null;
                    if (controller.ModelState.TryGetValue(key, out value))
                    {
                        if (value.Errors.Count != 0)
                        {
                            stateInfo =
                                new
                                {
                                    ValueName = key,
                                    State = "Error",
                                    ErrorCount = value.Errors.Count,
                                    Errors = value.Errors.Select(e => e.ErrorMessage)
                                };
                        }
                        else
                        {
                            stateInfo = new {ValueName = key, State = "Valid", Value = value.Value.AttemptedValue};
                        }
                    }
                    else
                    {
                        stateInfo = new {ValueName = key, State = "Empty"};
                    }
                    return stateInfo;
                }

                    );

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content =new StringContent(Json.Encode(validationObjects),new UnicodeEncoding(),"application/json")
                });
                //return true;
            }
        }

        public IHttpActionResult OnValidToken<T>(UnitusApiController controller,T arg,Func<T,IHttpActionResult> f) where T :AjaxRequestModelBase
        {
            IHttpActionResult getErrorContent = null;
            if (TryGetModelError(controller, out getErrorContent)) return getErrorContent;
            if (NoCheckAntiFogery) return f(arg);//for debug
            string[] tokens = arg.ValidationToken.Split(':');
            try
            {
                AntiForgery.Validate(tokens[0].Trim(),tokens[1].Trim());
                return f(arg);
            }
            catch (Exception)
            {
                return new StatusCodeResult(HttpStatusCode.Forbidden,controller);
            }
        }

        public async Task<IHttpActionResult> OnValidToken<T>(UnitusApiController controller, T arg, Func<T, Task<IHttpActionResult>> f) where T : AjaxRequestModelBase
        {
            IHttpActionResult getErrorContent = null;
            if (TryGetModelError(controller, out getErrorContent)) return getErrorContent;
            if (NoCheckAntiFogery) return await f(arg);//for debug
            string[] tokens = arg.ValidationToken.Split(':');
            try
            {
                AntiForgery.Validate(tokens[0].Trim(), tokens[1].Trim());
                return await f(arg);
            }
            catch (Exception)
            {
                return new StatusCodeResult(HttpStatusCode.Forbidden, controller);
            }
        } 
    }

    public static class AjaxRequestExtension
    {
        public static IHttpActionResult OnValidToken<T>(this UnitusApiController controller, T arg,
            Func<T, IHttpActionResult> f) where T : AjaxRequestModelBase
        {
            return arg.OnValidToken(controller, arg, f);
        }

        public static async Task<IHttpActionResult> OnValidToken<T>(this UnitusApiController controller, T arg,
    Func<T, Task<IHttpActionResult>> f) where T : AjaxRequestModelBase
        {
            return await arg.OnValidToken(controller, arg, f);
        }

        public static IHttpActionResult OnValidToken<T>(this UnitusApiController controller, T arg,
            Func<T, IHttpActionResult> f,Func<T,HashSet<string>,bool> vFunc) where T : AjaxRequestModelBase
        {
            return arg.OnValidToken(controller, arg, (r) =>
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

        public static async Task<IHttpActionResult> OnValidToken<T>(this UnitusApiController controller, T arg,
    Func<T, Task<IHttpActionResult>> f, Func<T, HashSet<string>, bool> vFunc) where T : AjaxRequestModelBase
        {
            return await arg.OnValidToken(controller, arg,async (r) =>
            {
                HashSet<string> err = new HashSet<string>();
                if (vFunc(r, err))
                {
                    return await f(r);
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


        public static async Task<IHttpActionResult> OnValidToken<T>(this UnitusApiController controller, T arg,
    Func<T, Task<IHttpActionResult>> f, Func<T, HashSet<string>, Task<bool>> vFunc) where T : AjaxRequestModelBase
        {
            return await arg.OnValidToken(controller, arg, async (r) =>
            {
                HashSet<string> err = new HashSet<string>();
                if (await vFunc(r, err))
                {
                    return await f(r);
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

        public static async Task<IHttpActionResult> OnValidToken(this UnitusApiController controller, string validationToken, Func<Task<IHttpActionResult>> f)
        {
            if (AjaxRequestModelBase.NoCheckAntiFogery) return await f();//for debug
            string[] tokens = validationToken.Split(':');
            try
            {
                AntiForgery.Validate(tokens[0].Trim(), tokens[1].Trim());
                return await f();
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

        public static async Task<IHttpActionResult> OnValidToken(this UnitusApiController controller,
            string validationtoken, Func<Task<IHttpActionResult>> fp, Func<HashSet<string>, bool> vfunc)
        {
            return await OnValidToken(controller, validationtoken, async () =>
            {
                HashSet<string> ErrorSet = new HashSet<string>();
                if (vfunc(ErrorSet))
                {
                    return await fp();
                }
                else
                {
                    string errorMsg = "";
                    foreach (var str in ErrorSet)
                    {
                        errorMsg += str + "\n";
                    }
                    return controller.JsonResult(new ResultContainer() { Success = false, ErrorMessage = errorMsg });
                }
            });
         
        }
    }
}