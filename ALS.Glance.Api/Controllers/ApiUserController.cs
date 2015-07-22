using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Api.Helpers;
using ALS.Glance.Api.Helpers.ODataInterfaces;
using ALS.Glance.Api.Models;
using ALS.Glance.Api.Properties;
using ALS.Glance.Api.Security;
using ALS.Glance.Api.Security.Extensions;
using ALS.Glance.Api.Security.Filters;
using ALS.Glance.Core.Security;
using ALS.Glance.Models.Security.Implementations;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.Api.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    public class ApiUserController : ODataController, ODataGet<ApiUser>.WithKey<string>
    {
          private readonly IALSUnitOfWork _uow;

        public ApiUserController(IUnitOfWorkFactory unitOfWorkFactory)
        {
             _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [EnableQuery, ApiAuthorize(Roles.Admin)]
        public IQueryable<ApiUser> Get()
        {
            return _uow.Security.BaseIdentities.GetAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [EnableQuery,
        ApiAuthorize(Roles.Admin, Roles.User),
        Permission(Role = Roles.User, ClaimType = ClaimTypes.Name, MustOwn = "key")]
        public async Task<IHttpActionResult> Get([FromODataUri] string key, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var user = await _uow.Security.GetUserManager<ApiUser>().FindByNameAsync(key);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
    }
}