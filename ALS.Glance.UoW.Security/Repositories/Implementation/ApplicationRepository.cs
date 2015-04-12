
using System.Data.Entity;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Security.Repositories.Interfaces;


namespace ALS.Glance.UoW.Security.Repositories.Implementation
{
    public class ApplicationRepository : EFRepository<ApplicationUser, string>, IApplicationRepository
    {
        public ApplicationRepository(DbContext context)
            : base(context)
        { }
    }
}
