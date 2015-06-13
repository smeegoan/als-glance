using System;
using System.Collections.Generic;
using ALS.Glance.DataAgents.ALS.Glance.Models;

namespace ALS.Glance.Web.Models
{
    public class PatientsViewModel
    {
         public double AgeMax { get; set; }

        public double AgeMin { get; set; }

        public IEnumerable<DPatient> Patients { get; set; }
    }
}