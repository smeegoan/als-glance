using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Security.Repositories.Interfaces;

namespace ALS.Glance.UoW.Security.Repositories.Implementation
{
    public class ApiUserRepository : EFRepository<ApiUser, string>, IApiUserRepository
    {
        public ApiUserRepository(DbContext context)
            : base(context)
        {
        }

        public async Task<ApiUser> GetByUserNameAsync(string userName, CancellationToken ct)
        {
            return await GetAll().SingleOrDefaultAsync(e => e.UserName == userName, ct);
        }
    }
}
