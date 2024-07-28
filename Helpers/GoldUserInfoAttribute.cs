using Accounting.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GoldHelpers.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GoldUserInfoAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            UserInfoAttribute userInfoAttribute = new UserInfoAttribute();
            userInfoAttribute.OnActionExecuting(context);
        }
    }
}
