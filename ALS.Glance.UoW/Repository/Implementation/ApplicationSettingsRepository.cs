using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.Models;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Repository.Interface;

namespace ALS.Glance.UoW.Repository.Implementation
{
    public class ApplicationSettingsRepository : EFRepository<ApplicationSettings, long>, IApplicationSettingsRepository
    {
        public ApplicationSettingsRepository(DbContext context)
            : base(context)
        {
        }

        public async Task<ApplicationSettings> GetByUserIdAndApplicationIdAsync(string userId, string applicationId, CancellationToken ct)
        {
            return await GetAll().SingleOrDefaultAsync(
                e => e.UserId == userId && e.ApplicationId == applicationId, ct);
        }

        public ApplicationSettings GetByUserIdAndApplicationId(string userId, string applicationId)
        {
            return GetAll().SingleOrDefault(
                e => e.UserId == userId && e.ApplicationId == applicationId);
        }
    }
}
