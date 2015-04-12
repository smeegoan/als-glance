using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.UoW.Security.Repositories.Interfaces
{
    public interface IBaseIdentityRepository : IRepository<IdentityUser, string>
    {
    }
}
