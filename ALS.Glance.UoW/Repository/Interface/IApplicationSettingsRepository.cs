using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.Models;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.UoW.Repository.Interface
{
    public interface IApplicationSettingsRepository : IRepository<ApplicationSettings, long>
    {
        Task<ApplicationSettings> GetByUserIdAndApplicationIdAsync(
            string userId, string applicationId, CancellationToken ct);

        ApplicationSettings GetByUserIdAndApplicationId(string userId, string applicationId);
    }
}
