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
            Categories.Read,
            Categories.Create,
            Categories.Update,
            Categories.Delete,

            Orders.Read,
            Orders.Create,
            Orders.Update,
            Orders.Cancel,

            Users.Read,
            Users.Manage,
            Brands.Read,
            Brands.Create,
            Brands.Update,
            Brands.Delete,

            Reviews.Read,
            Reviews.Create,
            Reviews.Delete,
            Products.Approve,
            Products.Reject,
            Products.AttributeManage,
        };

        public static class Brands
        {
            public const string Read = "Brands.Read";
            public const string Create = "Brands.Create";
            public const string Update = "Brands.Update";
            public const string Delete = "Brands.Delete";
        }
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
        public static class Categories
        {
            public const string Read = "Categories.Read";
            public const string Create = "Categories.Create";
            public const string Update = "Categories.Update";
            public const string Delete = "Categories.Delete";
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
