using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using UnitusCore.Results;

namespace UnitusCore.Attributes
{
    public class ApiAuthorizedAttribute:AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                ReasonPhrase = "Unauthorized API Access",
                Content = new ObjectContent(typeof(ResultContainer),ResultContainer.GenerateFaultResult("Unauthorized"), new JsonMediaTypeFormatter())
            };
            
        }
    }
}