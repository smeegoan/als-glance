
using ALS.Glance.Models.Security;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.UoW.Security.Repositories.Interfaces
{
    public interface IApiAuthenticationTokenRepository : IRepository<ApiAuthenticationToken, long>
    {
    }
}
