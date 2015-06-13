using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Security;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.Api.Controllers
{
    public class EMGController : ODataController,
        ODataGet<EMG>.WithKey<long>
    {
        private readonly IALSUnitOfWork _uow;
        public EMGController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        [EnableQuery, CorsPolicy]
        public IQueryable<EMG> Get()
        {
            return _uow.EMGs.GetAll();
        }

        [EnableQuery, CorsPolicy]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var entity = await _uow.EMGs.GetByIdAsync(key, ct);
            if (entity == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { entity }.AsQueryable()));
        }
    }
}