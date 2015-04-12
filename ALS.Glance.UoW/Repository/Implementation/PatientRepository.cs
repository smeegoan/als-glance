using System;
using System.Data.Entity;
using ALS.Glance.Models;
using ALS.Glance.UoW.EF;
using ALS.Glance.UoW.Repository.Interface;

namespace ALS.Glance.UoW.Repository.Implementation
{
    public class PatientRepository : EFRepository<DPatient, long>, IPatientRepository
    {
        public PatientRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}