using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Models;

namespace UnitusCore.Controllers
{
    public class GithubController : Controller
    {
        private string authorizeUrl =
            "https://github.com/login/oauth/authorize?client_id={0}&scope=repo:status,user&redirect_uri={1}&state={2}";

        private string accessTokenObtainUrl = "https://github.com/login/oauth/access_token";

        private Dictionary<string, GithubApplicationKey> ApplicationKeys = new Dictionary<string, GithubApplicationKey>()
        {
            {"DEBUG", new GithubApplicationKey("90a14fa554f58795c58b", "038426bee3d13ca096fd1a478b5ac5ca86e84a5b")},
            {"RELEASE",new GithubApplicationKey("9ee00b1085561f4f6ec4","4a72770f6d802a2f451036dc5994e8df4540da07")}
        };


        public ApplicationUserManager UserManager
        {
            get { return Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        public IAuthenticationManager AuthenticationContext
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public ApplicationDbContext ApplicationDbSession
        {
            get { return Request.GetOwinContext().Get<ApplicationDbContext>(); }
        }


        private GithubApplicationKey CurrentApplicationKey
        {
            get
            {
#if DEBUG
                return ApplicationKeys["DEBUG"];
#else
                return ApplicationKeys["RELEASE"];
#endif
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Authorize()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return Redirect(string.Format(authorizeUrl, CurrentApplicationKey.ApplicationId,
                Url.Action("AuthorizeCallback", "Github", null, Request.Url.Scheme), cookieToken + ":" + formToken));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> AuthorizeCallback(GithubAuthorizeRequest callback)
        {
            var crlfToken = callback.state;
            string[] tokens = crlfToken.Split(':');
            if (tokens.Length != 2)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            else
            {
                try
                {
                    AntiForgery.Validate(tokens[0].Trim(), tokens[1].Trim());
                    var accessToken = await ObtainAccessToken(callback.code);
                    var user = UserManager.FindByName(User.Identity.Name);
                    user.GithubAccessToken = GithubAccessTokenModel.FromJson(accessToken).access_token;
                    await ApplicationDbSession.SaveChangesAsync();
                    return RedirectToAction("Index", "Home",new DashboardRequest(new DashboardInformation("success","Github連携","Github連携に成功しました。")));
                }
                catch (Exception antifogeryError)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
            }
        }

        private async Task<string> ObtainAccessToken(string code)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", CurrentApplicationKey.ApplicationId),
                new KeyValuePair<string, string>("client_secret", CurrentApplicationKey.ApplicationSecretKey),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri",
                    Url.Action("AuthorizeCallback", "Github", null, Request.Url.Scheme)),
            });
            var response = await client.PostAsync(accessTokenObtainUrl, requestContent);
            HttpContent responseContent = response.Content;
            //レスポンスを読む処理
            string responseString = "";
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                responseString = await reader.ReadToEndAsync();
            }
            return responseString;
        }

        private class GithubApplicationKey
        {
            public GithubApplicationKey(string applicationId, string applicationSecretKey)
            {
                ApplicationId = applicationId;
                ApplicationSecretKey = applicationSecretKey;
            }

            public string ApplicationId { get; set; }

            public string ApplicationSecretKey { get; set; }
        }

        private class GithubAccessTokenModel
        {
            public static GithubAccessTokenModel FromJson(string jsonCode)
            {
                return System.Web.Helpers.Json.Decode<GithubAccessTokenModel>(jsonCode);
            }

            public string access_token { get; set; }

            public string scope { get; set; }

            public string token_type { get; set; }
        }
    }

    public class GithubAuthorizeRequest
    {
        public string state { get; set; }

        public string code { get; set; }
    }
}
