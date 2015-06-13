using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.DataAgents.ALS.Glance.Api.Models;
using ALS.Glance.DataAgents.ALS.Glance.Models;
using ALS.Glance.DataAgents.Interfaces;
using ALS.Glance.Models.Core;

namespace ALS.Glance.DataAgents.Implementations
{
    public class ALSGlanceDA : IALSGlanceDA
    {
        private readonly string _apiUrl;

        public ALSGlanceDA(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public async Task<IEnumerable<DPatient>> GetPatientsAsync(WebApiCredentials credentials, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await WebApiODataContainer.Using(_apiUrl, credentials)
               .ExecuteAuthenticated(
                    async container =>
                    {
                        ct.ThrowIfCancellationRequested();
                        var query = container.DPatient;

                        return await query.GetAllPagesAsync();
                    },
                   ct);
        }

        public async Task<ApplicationSettings> GetSettingsAsync(WebApiCredentials credentials, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await WebApiODataContainer.Using(_apiUrl, credentials)
               .ExecuteAuthenticated(
                     container =>
                     {
                         ct.ThrowIfCancellationRequested();
                         var query =
                             container.ApplicationSettings.Where(
                                 e => e.ApplicationId == credentials.ApplicationId && e.UserId == credentials.UserName);
                         return query.SingleOrDefault();
                     },
                   ct);
        }

        public virtual async Task<AgeBounds> GetAgeBoundsAsync(WebApiCredentials credentials, CancellationToken ct)
        {
            return await WebApiODataContainer.Using(_apiUrl, credentials)
                .GetAuthenticatedAsync<AgeBounds>(
                    container => string.Format("{0}/.GetAgeBounds", container.DPatient.ToString()),
                    exception => { },
                    ct);
        }

        public virtual async Task<YearBounds> GetYearBoundsAsync(WebApiCredentials credentials, long patientId, CancellationToken ct)
        {
            return await WebApiODataContainer.Using(_apiUrl, credentials)
                .GetAuthenticatedAsync<YearBounds>(
                    container => string.Format("{0}({1})/.GetYearBounds", container.DPatient.ToString(), patientId),
                    exception => { },
                    ct);

        }

        public async Task<IEnumerable<DMuscle>> GetMusclesAsync(WebApiCredentials credentials, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await WebApiODataContainer.Using(_apiUrl, credentials)
                .ExecuteAuthenticated(
                    async container =>
                    {
                        ct.ThrowIfCancellationRequested();
                        var query = container.DMuscle;

                        return await query.GetAllPagesAsync();
                    },
                    ct);
        }
    }
}
