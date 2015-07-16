using System;
using ALS.Glance.Api.Helpers.ODataInterfaces;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Core.Security;
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
    public class FactController : ODataController,
        ODataGet<Fact>.WithKey<long>
    {
        private readonly IALSUnitOfWork _uow;

        public FactController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        #region ODataGet

        [EnableQuery, EnableCors, ApiAuthorize(Roles.Admin, Roles.User)]
        public IQueryable<Fact> Get()
        {
            return _uow.Facts.GetAll();
        }

        [EnableQuery, ApiAuthorize(Roles.Admin, Roles.User)]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var entity = await _uow.Facts.GetByIdAsync(key, ct);
            if (entity == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { entity }.AsQueryable()));
        }

        #endregion

    }
}