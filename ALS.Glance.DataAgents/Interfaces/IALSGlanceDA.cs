using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.DataAgents.ALS.Glance.Api.Models;
using ALS.Glance.DataAgents.ALS.Glance.Models;
using ALS.Glance.DataAgents.Core.Interfaces;
using ALS.Glance.Models.Core;

namespace ALS.Glance.DataAgents.Interfaces
{
    public interface IALSGlanceDA : IDataAgent
    {
        Task<IEnumerable<DPatient>> GetPatientsAsync(WebApiCredentials credentials, CancellationToken ct);

        Task<AgeBounds> GetAgeBoundsAsync(WebApiCredentials credentials, CancellationToken ct);

        Task<YearBounds> GetYearBoundsAsync(WebApiCredentials credentials, long patientId, CancellationToken ct);

        Task<IEnumerable<Tuple<string,string>>> GetMusclesAsync(WebApiCredentials credentials, long patientId, CancellationToken ct);
    }
}
