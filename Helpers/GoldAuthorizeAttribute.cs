﻿using Accounting.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GoldHelpers.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GoldAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            /*ILoggerFactory? factory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            ILogger<AuthorizeAttribute>? logger = factory.CreateLogger<AuthorizeAttribute>();
            AuthorizeAttribute accountingAuthAttr = new(logger, (IAuthentication)new AuthenticationService());*/
            AuthorizeAttribute accountingAuthAttr = new AuthorizeAttribute();
            accountingAuthAttr.OnAuthorization(context);
        }
    }
}