using System;
using System.Collections.Generic;
using System.Web.OData.Query;
using ALS.Glance.Api.Helpers;
using ALS.Glance.Api.Helpers.Binder;
using ALS.Glance.Api.Properties;
using ALS.Glance.Core.Cache;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Core.Security;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace ALS.Glance.Api.Controllers
{
    public class FactsController : ODataController,
        ODataGet<Facts>.WithKeyAndOptions<long>
    {
        private readonly IODataQueryOptionsBinder _binder;
        private readonly Settings _settings;
        private readonly IALSUnitOfWork _uow;

        public FactsController(IUnitOfWorkFactory unitOfWorkFactory, IODataQueryOptionsBinder binder,Settings settings)
        {
            _binder = binder;
            _settings = settings;
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        #region ODataGet

        [EnableQuery, EnableCors, ApiAuthorize(Roles.Admin,Roles.User)]
        public IQueryable<Facts> Get(ODataQueryOptions<Facts> options)
        {
            var parameters = _binder.BindFilter(options.Filter, ex =>
            {
                throw new HttpResponseException(Request.CreateBadRequestResponse(ex.Message));
            });

              var patientId = parameters.SingleOrDefault("PatientId", () =>
            {
                throw new HttpResponseException(Request.CreateNotImplementedResponse());
            });

            if (!string.IsNullOrEmpty(patientId))
            {
                var cache = new ResponseCache<IEnumerable<Facts>>(false, DefaultCacheTime.Long, _settings.ResponseCacheEnabled, _settings.ResponseCacheDefaultShortTimeInMinutes, _settings.ResponseCacheDefaultLongTimeInMinutes);
                var facts = cache.GetValue(Request);
                if (facts == null)
                {
                    var id = Convert.ToInt64(patientId);
                    facts = _uow.IndexedFacts.GetAll().Where(e => e.PatientId == id).ToArray();
                    cache.SetValue(Request, facts);
                }
                return facts.AsQueryable();
            }
            return _uow.IndexedFacts.GetAll();
        }

        [EnableQuery, ApiAuthorize(Roles.Admin, Roles.User)]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, ODataQueryOptions<Facts> options, CancellationToken ct)
        {
            var entity = await _uow.IndexedFacts.GetByIdAsync(key, ct);
            if (entity == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { entity }.AsQueryable()));
        }

        #endregion


    }
}