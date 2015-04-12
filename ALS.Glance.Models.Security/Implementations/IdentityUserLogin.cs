
using ALS.Glance.Models.Security.Interfaces;

namespace ALS.Glance.Models.Security.Implementations
{
    public class IdentityUserLogin : IIdentityUserLogin<string>
    {
        // Summary:
        //     The login provider for the login (i.e. facebook, google)
        public virtual string LoginProvider { get; set; }
        //
        // Summary:
        //     Key representing the login for the provider
        public virtual string ProviderKey { get; set; }
        //
        // Summary:
        //     User Id for the user who owns this login
        public virtual string UserId { get; set; }
    }
}
