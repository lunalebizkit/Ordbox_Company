using Ordbox.Api.Extension;
using Ordbox.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Ordbox.Api.Filter
{
    public class AllowAccessAttribute : ActionFilterAttribute
    {
        public EPermission[] Permission { get; set; }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var isLogin = actionContext.HttpContext.User.Identity.IsAuthenticated;

            if (!isLogin)
            {
                actionContext.Result = new ContentResult { Content = "403", StatusCode = 401 };
            }
            else
            {
                if (!HasPermission(actionContext.HttpContext.User.GetPermission()))
                {
                    actionContext.Result = new ContentResult { Content = "403", StatusCode = 401 };
                }
            }

            base.OnActionExecuting(actionContext);
        }

        private bool HasPermission(int[] permission)
        {
            return Permission.Any(r => permission.Contains((int)r));
        }
    }
}