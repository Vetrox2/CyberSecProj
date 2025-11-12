using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace backend.Filters
{
    public class AuthenticationFilter : IAsyncActionFilter
    {
        static readonly string[] ActionsWithoutAuth = ["Login", "GetSettings", "VerifyRecaptcha"];

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionName = context.RouteData.Values["action"]?.ToString();

            if (ActionsWithoutAuth.Contains(actionName))
            {
                await next();
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "User ID header is required" });
                return;
            }

            if (!Guid.TryParse(userIdHeader.ToString(), out var userId))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid User ID format" });
                return;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "CustomAuth");
            context.HttpContext.User.AddIdentity(identity);

            await next();
        }
    }
}
