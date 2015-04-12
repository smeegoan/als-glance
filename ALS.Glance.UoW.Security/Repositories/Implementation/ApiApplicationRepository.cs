using System.Data.Entity;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Security.Repositories.Interfaces;

namespace ALS.Glance.UoW.Security.Repositories.Implementation
{
    public class ApiApplicationRepository : EFRepository<ApiApplicationUser, string>, IApiApplicationRepository
    {
        public ApiApplicationRepository(DbContext context)
            : base(context)
        {
        }
    }
}
