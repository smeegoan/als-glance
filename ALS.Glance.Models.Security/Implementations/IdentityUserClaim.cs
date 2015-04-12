using ALS.Glance.Models.Security.Interfaces;

namespace ALS.Glance.Models.Security.Implementations
{
    public class IdentityUserClaim : IIdentityUserClaim<string>
    {
        // Summary:
        //     Claim type
        public virtual string ClaimType { get; set; }
        //
        // Summary:
        //     Claim value
        public virtual string ClaimValue { get; set; }
        //
        // Summary:
        //     Primary key
        public virtual int Id { get; set; }
        //
        // Summary:
        //     User Id for the user who owns this login
        public virtual string UserId { get; set; }
    }
}
