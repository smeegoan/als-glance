using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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


        public async Task<IEnumerable<int>> GetFactYearsAsync(WebApiCredentials credentials, long patientId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await WebApiODataContainer.Using(_apiUrl, credentials)
               .ExecuteAuthenticated(
                     container =>
                     {
                         ct.ThrowIfCancellationRequested();
                         var years = container.Fact.Expand(e => e.Date).Where(f => f.Patient.Id == patientId);

                         return years.ToArray().Select(e => (int)e.Date.Year).Distinct();
                     },
                   ct);
        }


        public async Task<IEnumerable<Tuple<string, string>>> GetMusclesAsync(WebApiCredentials credentials, long patientId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await WebApiODataContainer.Using(_apiUrl, credentials)
               .ExecuteAuthenticated(
                     container =>
                     {
                         ct.ThrowIfCancellationRequested();
                         var facts = container.Fact.Expand(e => e.Muscle).Where(f => f.Patient.Id == patientId);

                         return facts.ToArray().Select(e => new Tuple<string, string>(e.Muscle.Abbreviation, e.Muscle.Name)).Distinct();
                     },
                   ct);
        }
    }
}
