using Microsoft.AspNetCore.Authorization;

namespace Ecomm.Api.Authorization
{
    // Represents a permission requirement for authorization
    public class PermissionRequirement:IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
