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

        public ALSGlanceDA( string apiUrl)
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
                        var pharmacyCardQuery = container.DPatient;

                        var pharmacyCard = await pharmacyCardQuery.GetAllPagesAsync();

                        return pharmacyCard;
                    },
                   ct);
        }
    }
}
