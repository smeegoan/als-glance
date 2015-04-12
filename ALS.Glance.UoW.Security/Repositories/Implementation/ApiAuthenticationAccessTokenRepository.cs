using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Security.Repositories.Interfaces;
using System.Data.Entity;

namespace ALS.Glance.UoW.Security.Repositories.Implementation
{
    public class ApiAuthenticationAccessTokenRepository : EFRepository<ApiAuthenticationAccessToken, long>, IApiAuthenticationAccessTokenRepository
    {
        public ApiAuthenticationAccessTokenRepository(DbContext context)
            : base(context)
        {
        }
    }
}
