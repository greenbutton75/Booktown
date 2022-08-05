namespace Identity.Helpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Identity.Models;

[AttributeUsage(AttributeTargets.Method)]
public class CustomAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
       if (string.IsNullOrEmpty(Convert.ToString(context.HttpContext.Request.Headers["CustomerName"])) || string.IsNullOrEmpty(Convert.ToString(context.HttpContext.Request.Headers["CustomerEmail"])))  
        {
            // not logged in
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}