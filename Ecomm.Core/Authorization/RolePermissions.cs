using Ecomm.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Authorization
{
    public static class RolePermissions
    {
        public static IReadOnlyList<string> GetPermissionsForRole(string role)
        {
            return role switch
            {
                RolesEnum.Admin => AdminPermissions(),
                RolesEnum.Seller => SellerPermissions(),
                RolesEnum.Customer => CustomerPermissions(),
                _ => Array.Empty<string>()
            };
        }
        private static IReadOnlyList<string> AdminPermissions()
        {
            // Admin has ALL permissions
            return Permissions.All;
        }

        private static IReadOnlyList<string> SellerPermissions()
        {
            return
            [
                Permissions.Products.Read,
                Permissions.Products.Create,
                Permissions.Products.Update,

                Permissions.Orders.Read,

                Permissions.Reviews.Read
            ];
        }

        private static IReadOnlyList<string> CustomerPermissions()
        {
            return [
            
                Permissions.Products.Read,

                Permissions.Orders.Create,
                Permissions.Orders.Read,

                Permissions.Reviews.Create,
                Permissions.Reviews.Read,
                Permissions.Reviews.Delete
            ];
        }
    
    }
}
