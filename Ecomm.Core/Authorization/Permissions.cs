using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Authorization
{
    public static class Permissions
    {
       
        public static readonly IReadOnlyList<string> All = new[]
        {
            Products.Read,
            Products.Create,
            Products.Update,
            Products.Delete,
           
            Orders.Read,
            Orders.Create,
            Orders.Update,
            Orders.Cancel,

            Users.Read,
            Users.Manage,

            Reviews.Read,
            Reviews.Create,
            Reviews.Delete,
            Products.Approve,
            Products.Reject,
            Products.AttributeManage,
        };

        public static class Products
        {
            public const string Read = "Products.Read";
            public const string Create = "Products.Create";
            public const string Update = "Products.Update";
            public const string Delete = "Products.Delete";
            public const string Approve = "Products.Approve";
            public const string Reject = "Products.Reject";
            public const string AttributeManage = "Products.AttributeManage";
        }

        public static class Orders
        {
            public const string Read = "Orders.Read";
            public const string Create = "Orders.Create";
            public const string Update = "Orders.Update";
            public const string Cancel = "Orders.Cancel";
        }

        public static class Users
        {
            public const string Read = "Users.Read";
            public const string Manage = "Users.Manage";
        }

        public static class Reviews
        {
            public const string Read = "Reviews.Read";
            public const string Create = "Reviews.Create";
            public const string Delete = "Reviews.Delete";
        }
    }

}
