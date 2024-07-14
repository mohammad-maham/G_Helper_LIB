using Accounting.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GoldHelpers.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GoldAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            AuthorizeAttribute accountingAuthAttr = new AuthorizeAttribute();
            accountingAuthAttr.OnAuthorization(context);
        }
    }
}