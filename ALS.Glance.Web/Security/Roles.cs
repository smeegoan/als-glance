using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALS.Glance.Web.Security
{
    public static class Roles
    {
        public static readonly string[] AuthorizedRoles = { AdminRole, UserRole };

        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public static bool IsRoleAuthorized(this string role)
        {
            return AuthorizedRoles.Contains(role);
        }

    }
}