using ALS.Glance.Models.Security;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Security.Repositories.Interfaces;
using System.Data.Entity;

namespace ALS.Glance.UoW.Security.Repositories.Implementation
{
    public class ApiAuthenticationTokenRepository : EFRepository<ApiAuthenticationToken, long>, IApiAuthenticationTokenRepository
    {
        public ApiAuthenticationTokenRepository(DbContext context)
            : base(context)
        { }
    }
}
