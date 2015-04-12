
using ALS.Glance.Models.Security.Interfaces;

namespace ALS.Glance.Models.Security.Implementations
{
    public class IdentityUserRole : IIdentityUserRole<string>
    {
        public virtual string UserId { get; set; }

        /// <summary>
        ///     RoleId for the role
        /// </summary>
        public virtual string RoleId { get; set; }
    }
}
