
using System.Data.Entity;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Security.Repositories.Interfaces;

namespace ALS.Glance.UoW.Security.Repositories.Implementation
{
    public class SiteUserRepository : EFRepository<SiteUser, string>, ISiteUserRepository
    {
        public SiteUserRepository(DbContext context)
            : base(context)
        { }
    }
}
