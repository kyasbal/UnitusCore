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

        private void ApplyCheckResult(HttpActionContext actionContext)
        {
            if (!IsValid(actionContext))
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("You have no enough authority to access this api");
                actionContext.Response = response;
            }
        }

        private bool IsValid(HttpActionContext context)
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
                    Guid circleId = Guid.Parse((string)circleArg);
                    if (!IsCircleMember(circleId, context))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            
        }




        private bool IsCircleMember(Guid circleId,HttpActionContext context)
        {
            var dbContext = GetDbContext(context);
            Circle circle = dbContext.Circles.Find(circleId);
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

        private void ApplyCheckResult(HttpActionContext actionContext)
        {
            if (!IsValid(actionContext))
            {
                var response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("You have no enough authority to access this api");
                actionContext.Response = response;
            }
        }

        private bool IsValid(HttpActionContext context)
        {
            if (IsAdmin(context)) return true;
            else
            {
                Circle circle = GetCircleFromArgument(context);
                return circle != null && IsCircleAdmin(context, circle);
            }
        }

        private Circle GetCircleFromArgument(HttpActionContext context)
        {
            var argObject = context.ActionArguments[_circleIdField];
            if (argObject == null) return null;
            else
            {
                Guid circleId = Guid.Parse((string) argObject);
                return GetDbContext(context).Circles.Find(circleId);
            }
        }

        private bool IsCircleAdmin(HttpActionContext context,Circle circle)
        {
            if (GetCurrentUser(context) == null) return false;
            var adminState = GetDbContext(context).Entry(circle).Collection(a => a.Administrators);
            if(!adminState.IsLoaded)adminState.Load();
            return circle.Administrators.Contains(GetCurrentUser(context));
        }
    }
}