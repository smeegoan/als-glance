using System;
using ALS.Glance.Models;
using ALS.Glance.UoW.Core;

namespace ALS.Glance.UoW.Repository.Interface
{
    public interface IPatientRepository : IRepository<DPatient, long>
    {

    }
}