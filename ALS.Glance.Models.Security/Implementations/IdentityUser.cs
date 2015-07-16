using System;
using System.Collections.Generic;
using ALS.Glance.Models.Core;
using ALS.Glance.Models.Core.Interfaces;
using ALS.Glance.Models.Security.Interfaces;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.Models.Security.Implementations
{

    public class IdentityUser : Model<string>, IIdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>, IUser,
        IHaveCreatedMeta, IHaveUpdatedMeta
    {

        public IdentityUser()
        {
            Id = Guid.NewGuid().ToString();
            Roles = new HashSet<IdentityUserRole>();
            Claims = new HashSet<IdentityUserClaim>();
            Logins = new HashSet<IdentityUserLogin>();
        }


        // Summary:
        //     Used to record failures for the purposes of lockout
        public int AccessFailedCount { get; set; }
        //
        // Summary:
        //     Email
        public string Email { get; set; }
        //
        // Summary:
        //     True if the email is confirmed, default is false
        public bool EmailConfirmed { get; set; }

        //
        // Summary:
        //     Is lockout enabled for this user
        public bool LockoutEnabled { get; set; }
        //
        // Summary:
        //     The salted/hashed form of the user password
        public string PasswordHash { get; set; }
        //
        // Summary:
        //     PhoneNumber for the user
        public string PhoneNumber { get; set; }
        //
        // Summary:
        //     True if the phone number is confirmed, default is false
        public bool PhoneNumberConfirmed { get; set; }
        //
        // Summary:
        //     A random value that should change whenever a users credentials have changed
        //     (password changed, login removed)
        public string SecurityStamp { get; set; }
        //
        // Summary:
        //     Is two factor enabled for the user
        public bool TwoFactorEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }
        //
        // Summary:
        //     User name
        public string UserName { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public virtual ICollection<IdentityUserRole> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin> Logins { get; set; }
    }
}
