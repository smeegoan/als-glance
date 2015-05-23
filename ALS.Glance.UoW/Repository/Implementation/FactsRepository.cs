using System.Data.Entity;
using ALS.Glance.Models;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Repository.Interface;

namespace ALS.Glance.UoW.Repository.Implementation
{
    public class FactsRepository : EFRepository<Facts, long>, IFactsRepository
    {
        public FactsRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}