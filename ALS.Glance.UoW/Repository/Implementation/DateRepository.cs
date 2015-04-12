using ALS.Glance.Models;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Repository.Interface;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace ALS.Glance.UoW.Repository.Implementation
{
    public class DateRepository : EFRepository<DDate, long>, IDateRepository
    {
        public DateRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

    }
}