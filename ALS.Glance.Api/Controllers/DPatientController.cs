using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Helpers;
using ALS.Glance.Api.Helpers.Cache;
using ALS.Glance.Api.Models;
using ALS.Glance.Api.Security;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.Api.Controllers
{
    public class DPatientController : ODataController,
        ODataGet<DPatient>.WithKey<long>
    {
        private readonly IALSUnitOfWork _uow;
        public DPatientController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        [EnableQuery, CorsPolicy]
        public IQueryable<DPatient> Get()
        {
            var cache = new ResponseCache(false, DefaultCacheTime.Long);
            var patients = cache.GetValue(Request) as IEnumerable<DPatient>;
            if (patients == null)
            {
                patients = _uow.Patients.GetAll().ToArray();
                cache.SetValue(Request, patients);
            }
            return patients.AsQueryable();
        }

        [EnableQuery]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var entity = await _uow.Patients.GetByIdAsync(key, ct);
            if (entity == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { entity }.AsQueryable()));
        }

        [HttpGet]
        public IHttpActionResult GetYearBounds([FromODataUri] long key)
        {
            var cache = new ResponseCache(false, DefaultCacheTime.Long);
            var bounds = cache.GetValue(Request) as YearBounds;
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

        [HttpGet]
        public IHttpActionResult GetAgeBounds()
        {
            var cache = new ResponseCache(false, DefaultCacheTime.Long);
            var bounds = cache.GetValue(Request) as AgeBounds;
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