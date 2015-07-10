using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Core.Security;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.Api.Controllers
{
    public class DDateController:ODataController,
        ODataGet<DDate>.WithKey<long>
    {
        private readonly IALSUnitOfWork _uow;
        public DDateController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>(); 
        }

        [EnableQuery, ApiAuthorize(Roles.Admin, Roles.User)]
        public IQueryable<DDate> Get()
        {
            return _uow.Dates.GetAll();
        }

        [EnableQuery, ApiAuthorize(Roles.Admin, Roles.User)]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var entity = await _uow.Dates.GetByIdAsync(key, ct);
            if (entity == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { entity }.AsQueryable()));
        }
    }
}