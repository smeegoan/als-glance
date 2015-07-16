using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Helpers.ODataInterfaces;
using ALS.Glance.Api.Properties;
using ALS.Glance.Core.Cache;
using ALS.Glance.Api.Models;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Core.Security;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.Api.Controllers
{
    public class DPatientController : ODataController,
        ODataGet<DPatient>.WithKey<long>
    {
        private readonly Settings _settings;
        private readonly IALSUnitOfWork _uow;
        public DPatientController(IUnitOfWorkFactory unitOfWorkFactory,Settings settings)
        {
            _settings = settings;
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        [EnableQuery, EnableCors, ApiAuthorize(Roles.Admin, Roles.User)]
        public IQueryable<DPatient> Get()
        {
            var cache = new ResponseCache<IEnumerable<DPatient>>(false, DefaultCacheTime.Long, _settings.ResponseCacheEnabled, _settings.ResponseCacheDefaultShortTimeInMinutes, _settings.ResponseCacheDefaultLongTimeInMinutes);
            var patients = cache.GetValue(Request) ;
            if (patients == null)
            {
                patients = _uow.Patients.GetAll().ToArray();
                cache.SetValue(Request, patients);
            }
            return patients.AsQueryable();
        }

        [EnableQuery, ApiAuthorize(Roles.Admin, Roles.User)]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var cache = new ResponseCache<DPatient>(false, DefaultCacheTime.Long, _settings.ResponseCacheEnabled, _settings.ResponseCacheDefaultShortTimeInMinutes, _settings.ResponseCacheDefaultLongTimeInMinutes);
            var entity = cache.GetValue(Request);
            if (entity == null)
            {
                entity = await _uow.Patients.GetByIdAsync(key, ct);
                if (entity == null)
                    return NotFound();
                cache.SetValue(Request, entity);
            }
            return Ok(SingleResult.Create(new[] { entity }.AsQueryable()));
        }

        [HttpGet, ApiAuthorize(Roles.Admin, Roles.User)]
        public IHttpActionResult GetYearBounds([FromODataUri] long key)
        {
            var cache = new ResponseCache<YearBounds>(false, DefaultCacheTime.Long, _settings.ResponseCacheEnabled, _settings.ResponseCacheDefaultShortTimeInMinutes, _settings.ResponseCacheDefaultLongTimeInMinutes);
            var bounds = cache.GetValue(Request) ;
            if (bounds == null)
            {
                var years = _uow.Facts.GetAll()
                   .Where(e => e.PatientId == key)
                   .Select(e => e.Date.Year)
                   .Distinct();

                bounds = new YearBounds
                {
                    Min = years.Min(),
                    Max = years.Max()
                };
                cache.SetValue(Request, bounds);
            }
            return Ok(bounds);
        }

        [HttpGet, ApiAuthorize(Roles.Admin, Roles.User)]
        public IHttpActionResult GetAgeBounds()
        {
            var cache = new ResponseCache<AgeBounds>(false, DefaultCacheTime.Long, _settings.ResponseCacheEnabled, _settings.ResponseCacheDefaultShortTimeInMinutes, _settings.ResponseCacheDefaultLongTimeInMinutes);
            var bounds = cache.GetValue(Request);
            if (bounds == null)
            {
                var today = DateTime.Now.Year;
                var patients = _uow.Patients
                    .GetAll()
                    .GroupBy(e => e.Sex)
                    .Select(grp => grp.Average(e => today - e.BornOn.Year));

                bounds = new AgeBounds
                {
                    Min = Math.Floor(patients.Min()),
                    Max = Math.Ceiling(patients.Max())
                };
                cache.SetValue(Request, bounds);
            }
            return Ok(bounds);
        }
    }
}