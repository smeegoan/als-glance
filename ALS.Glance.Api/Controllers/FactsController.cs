using System;
using ALS.Glance.Api.Security;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.UoW.Core.Exceptions;

namespace ALS.Glance.Api.Controllers
{
    public class FactsController : ODataController,
        ODataGet<Facts>.WithKey<long>
    {
        private readonly IALSUnitOfWork _uow;

        public FactsController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        #region ODataGet

        [EnableQuery, CorsPolicy]
        public IQueryable<Facts> Get()
        {
            return _uow.IndexedFacts.GetAll();
        }

        [EnableQuery]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var entity = await _uow.IndexedFacts.GetByIdAsync(key, ct);
            if (entity == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { entity }.AsQueryable()));
        }

        #endregion

    }
}