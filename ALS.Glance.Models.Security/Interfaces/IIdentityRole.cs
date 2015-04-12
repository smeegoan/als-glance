using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.Models.Security.Interfaces
{
    public interface IIdentityRole<out TKey, TUserRole> : IRole<TKey> where TUserRole : IIdentityUserRole<TKey>
    {

        // Summary:
        //     Navigation property for users in the role
        ICollection<TUserRole> Users { get; }
    }
}
