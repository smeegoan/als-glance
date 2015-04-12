

using System.Data.Entity;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.Models.Security.Interfaces;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.UoW.Security
{
    public class SecurityUserStore<TUser> : CustomUserStore<TUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>,
        IUserStore<TUser> where TUser : class, IIdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>, IUser
    {
        public SecurityUserStore(DbContext context)
            : base(context)
        {

        }
    }
}
