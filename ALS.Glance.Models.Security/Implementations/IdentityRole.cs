using System.Collections.Generic;
using ALS.Glance.Models.Core;
using ALS.Glance.Models.Security.Interfaces;

namespace ALS.Glance.Models.Security.Implementations
{
    public class IdentityRole : Model<string>, IIdentityRole<string, IdentityUserRole>
    {
        private readonly HashSet<IdentityUserRole> _users;
        public string Name { get; set; }

        public IdentityRole()
        {
            _users= new HashSet<IdentityUserRole>();
        }

        public IdentityRole(string name)
        {
            Name = name;
            Id = Name;
            _users= new HashSet<IdentityUserRole>();
        }

        public ICollection<IdentityUserRole> Users
        {
            get { return _users; }
        }
    }
}
