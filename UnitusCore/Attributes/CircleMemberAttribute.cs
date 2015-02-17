using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Attributes
{
    public class CircleMemberAttribute:UnitusActionFilterAttribute
    {
        private string _circleIdField;

        public CircleMemberAttribute(string circleIdField="CircleId")
        {
            this._circleIdField = circleIdField;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ApplyCheckResult(actionContext);
            base.OnActionExecuting(actionContext);
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            ApplyCheckResult(actionContext);
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        private async void ApplyCheckResult(HttpActionContext actionContext)
        {
            if (!await IsValid(actionContext))
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("You have no enough authority to access this api");
                actionContext.Response = response;
            }
        }

        private async Task<bool> IsValid(HttpActionContext context)
        {
            if (!IsAdmin(context))
            {
                return true;
            }
            else
            {
                object circleArg = context.ActionArguments[_circleIdField];
                if (circleArg == null)
                {
                    return false;
                }
                else
                {
                    return await IsCircleMember((string) circleArg,context);
                }
            }
            
        }




        private async Task<bool> IsCircleMember(string circleId,HttpActionContext context)
        {
            var dbContext = GetDbContext(context);
            Circle circle =await Circle.FromIdAsync(dbContext, circleId);
            if(circle==null)return false;
            var memberState = dbContext.Entry(circle).Collection(a => a.Members);
            if(!memberState.IsLoaded)memberState.Load();
            foreach (MemberStatus member in circle.Members)
            {
                var targetUserStatus = dbContext.Entry(member).Reference(a => a.TargetUser);
                if(!targetUserStatus.IsLoaded)targetUserStatus.Load();
                var applicationUserStatus = dbContext.Entry(member.TargetUser).Reference(a => a.ApplicationUser);
                if(!applicationUserStatus.IsLoaded)applicationUserStatus.Load();

                if (member.TargetUser.ApplicationUser.Id.Equals(GetCurrentUser(context).Id)) return true;
            }
            return false;
        }
    }

    public class CircleAdminAttribute:UnitusActionFilterAttribute
    {
        private readonly string _circleIdField;

        public CircleAdminAttribute(string circleIdField="CircleId")
        {
            _circleIdField = circleIdField;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ApplyCheckResult(actionContext);
            base.OnActionExecuting(actionContext);
        }
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            ApplyCheckResult(actionContext);
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        private async void ApplyCheckResult(HttpActionContext actionContext)
        {
            if (!await IsValid(actionContext))
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("You have no enough authority to access this api");
                actionContext.Response = response;
            }
        }

        private async Task<bool> IsValid(HttpActionContext context)
        {
            if (IsAdmin(context)) return true;
            else
            {
                Circle circle = await GetCircleFromArgument(context);
                return circle != null && await IsCircleAdmin(context, circle);
            }
        }

        private async Task<Circle> GetCircleFromArgument(HttpActionContext context)
        {
            var argObject = context.ActionArguments[_circleIdField];
            if (argObject == null) return null;
            else
            {
                return await Circle.FromIdAsync(GetDbContext(context), (string) argObject);
            }
        }

        private async Task<bool> IsCircleAdmin(HttpActionContext context,Circle circle)
        {
            if (GetCurrentUser(context) == null) return false;
            var adminState = GetDbContext(context).Entry(circle).Collection(a => a.Administrators);
            if(!adminState.IsLoaded)await adminState.LoadAsync();
            return circle.Administrators.Contains(GetCurrentUser(context));
        }
    }
}