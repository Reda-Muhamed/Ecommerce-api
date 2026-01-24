using Ecomm.Core.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Ecomm.Api.Authorization
{
    // represents extension methods for adding permission-based authorization
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // add policies based on permissions
                foreach (var permission in Permissions.All)
                {
                    options.AddPolicy(permission, policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission)));
                }

                
            });
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            return services;
        }
    }
}
