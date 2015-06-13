using ALS.Glance.Models;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Repository.Interface;
using System.Data.Entity;

namespace ALS.Glance.UoW.Repository.Implementation
{
    public class EMGRepository : EFRepository<EMG, long>, IEMGRepository
    {
        public EMGRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

    }
}