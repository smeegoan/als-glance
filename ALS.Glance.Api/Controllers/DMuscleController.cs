using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Helpers.Cache;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.Api.Controllers
{
    public class DMuscleController : ODataController,
        ODataGet<DMuscle>.WithKey<long>
    {
        private readonly IALSUnitOfWork _uow;
        public DMuscleController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        [EnableQuery, EnableCors, ApiAuthorize(Roles.Admin, Roles.Application, Roles.User)]
        public IQueryable<DMuscle> Get()
        {
            var cache = new ResponseCache<IEnumerable<DMuscle>>(false, DefaultCacheTime.Long);
            var muscles = cache.GetValue(Request) ;
            if (muscles == null)
            {
                muscles = _uow.Muscles.GetAll().ToArray();
                cache.SetValue(Request, muscles);
            }
            return muscles.AsQueryable();
        }

        [EnableQuery, ApiAuthorize(Roles.Admin, Roles.Application, Roles.User)]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var muscle = await _uow.Muscles.GetByIdAsync(key, ct);
            if (muscle == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { muscle }.AsQueryable()));
        }


    }
}