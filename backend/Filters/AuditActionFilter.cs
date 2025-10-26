using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend.Filters
{
    public class AuditActionFilter(IAuditLogService auditLogService, IUserService userService) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();

            var actionType = GetActionType(context);
            if (actionType == null)
                return;

            var successState = IsSuccessStatusCode(executedContext);
            var userId = await ExtractUserId(context, executedContext);
            if (userId == null)
                return;

            await auditLogService.LogAsync(userId.Value, actionType, successState);
        }

        private async Task<Guid?> ExtractUserId(ActionExecutingContext context, ActionExecutedContext executedContext)
        {
            var actionName = context.RouteData.Values["action"]?.ToString();

            if (actionName == "Login" && executedContext.Result is OkObjectResult okResult)
            {
                return okResult.Value is User u ? u.Id : null;
            }

            var userIdClaim = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                return Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
            }

            if (!(context.ActionArguments.TryGetValue("dto", out var dtoObj) && dtoObj is LoginDto loginDto))
            {
                return null;
            }

            var user = await userService.GetByLoginAsync(loginDto.Login);
            return user?.Id;
        }

        private static string? GetActionType(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();
            var httpMethod = context.HttpContext.Request.Method;

            return controllerName switch
            {
                "Users" => AuditActionTypeMappers.UsersControllerActionMapper(httpMethod, actionName ?? ""),
                _ => null
            };
        }

        private static bool IsSuccessStatusCode(ActionExecutedContext context)
        {
            if (context.Exception != null && !context.ExceptionHandled)
                return false;

            if (context.Result is ObjectResult objectResult)
            {
                return objectResult.StatusCode >= 200 && objectResult.StatusCode < 300;
            }

            if (context.Result is StatusCodeResult statusCodeResult)
            {
                return statusCodeResult.StatusCode >= 200 && statusCodeResult.StatusCode < 300;
            }

            if (context.Result is NoContentResult or CreatedAtActionResult or OkObjectResult or OkResult)
                return true;

            if (context.Result is ForbidResult or NotFoundResult or NotFoundObjectResult or ConflictObjectResult)
                return false;

            var statusCode = context.HttpContext.Response.StatusCode;
            return statusCode >= 200 && statusCode < 300;
        }
    }

    static partial class AuditActionTypeMappers
    {
        public static string? UsersControllerActionMapper(string httpMethod, string actionName)
            => (httpMethod, actionName) switch
            {
                ("GET", "GetAll") => AuditActionType.UserGetAll,
                ("GET", "Get") => AuditActionType.UserGetById,
                ("POST", "Create") => AuditActionType.UserCreate,
                ("PUT", "Update") => AuditActionType.UserEdit,
                ("DELETE", "Delete") => AuditActionType.UserDelete,
                ("POST", "ChangePassword") => AuditActionType.UserPasswordChange,
                ("POST", "SetPasswordByAdmin") => AuditActionType.UserPasswordSetByAdmin,
                ("POST", "SetPasswordByUser") => AuditActionType.UserPasswordSetByUser,
                ("POST", "Login") => AuditActionType.UserLogin,
                ("POST", "GenerateOneTimePassword") => AuditActionType.GenerateOneTimePassword,
                _ => null
            };
    }
}
