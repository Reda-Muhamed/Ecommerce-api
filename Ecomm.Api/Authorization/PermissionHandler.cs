using Microsoft.AspNetCore.Authorization;

namespace Ecomm.Api.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if(context.User==null || !context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }
            var permissions = context.User.Claims
                .Any(c=> c.Type == "permission" && c.Value == requirement.Permission);
            if(permissions)
             {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
