using System.Data.Entity;
using ALS.Glance.Models;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Repository.Interface;

namespace ALS.Glance.UoW.Repository.Implementation
{
    public class MuscleRepository : EFRepository<DMuscle, long>, IMuscleRepository
    {
        public MuscleRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}