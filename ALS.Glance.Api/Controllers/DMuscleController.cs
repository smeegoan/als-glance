﻿using System;
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
    public class DMuscleController : ODataController,
        ODataGet<DMuscle>.WithKey<long>
    {
        private readonly IALSUnitOfWork _uow;
        public DMuscleController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>();
        }

        [EnableQuery, CorsPolicy]
        public IQueryable<DMuscle> Get()
        {
            var cache = new ResponseCache(false, DefaultCacheTime.Long);
            var muscles = cache.GetValue(Request) as IEnumerable<DMuscle>;
            if (muscles == null)
            {
                muscles = _uow.Muscles.GetAll().ToArray();
                cache.SetValue(Request, muscles);
            }
            return muscles.AsQueryable();
        }

        [EnableQuery]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var muscle = await _uow.Muscles.GetByIdAsync(key, ct);
            if (muscle == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { muscle }.AsQueryable()));
        }


    }
}