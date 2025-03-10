using System;
using Business.Exceptions;
using Integration.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        IAuthorizationService authService = context.HttpContext.RequestServices.GetService<IAuthorizationService>()!;

        if (!authService.ValidateToken())
        {
            throw ApiExceptions.AE401;
        }
    }
}
