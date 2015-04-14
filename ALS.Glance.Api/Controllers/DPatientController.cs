﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using ALS.Glance.Models;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.Api.Controllers
{
    public class DPatientController:ODataController,
        ODataGet<DPatient>.WithKey<long>
    {
        private readonly IALSUnitOfWork _uow;
        public DPatientController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uow = unitOfWorkFactory.Get<IALSUnitOfWork>(); 
        }

        [EnableQuery]
        public IQueryable<DPatient> Get()
        {
            return _uow.Patients.GetAll();
        }
       
        [EnableQuery]
        public async Task<IHttpActionResult> Get([FromODataUri] long key, CancellationToken ct)
        {
            var entity = await _uow.Patients.GetByIdAsync(key, ct);
            if (entity == null)
                return NotFound();
            return Ok(SingleResult.Create(new[] { entity }.AsQueryable()));
        }
    }
}