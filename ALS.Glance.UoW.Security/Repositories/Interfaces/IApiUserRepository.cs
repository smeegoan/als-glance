using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW.Core;


namespace ALS.Glance.UoW.Security.Repositories.Interfaces
{
    public interface IApiUserRepository : IRepository<ApiUser, string>
    {
        Task<ApiUser> GetByUserNameAsync(string userName, CancellationToken ct);
    }
}
