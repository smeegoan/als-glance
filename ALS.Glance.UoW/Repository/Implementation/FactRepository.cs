using System.Data.Entity;
using ALS.Glance.Models;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Repository.Interface;

namespace ALS.Glance.UoW.Repository.Implementation
{
    public class FactRepository : EFRepository<Fact, long>, IFactRepository
    {
        public FactRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}