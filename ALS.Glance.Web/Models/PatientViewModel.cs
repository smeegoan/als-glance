using System.Collections.Generic;
using ALS.Glance.DataAgents.ALS.Glance.Models;

namespace ALS.Glance.Web.Models
{
    public class PatientViewModel
    {
        public string Settings { get; set; }
        public long Id { get; set; }
        public short YearMax { get; set; }

        public short YearMin { get; set; }

        public IEnumerable<DMuscle> Muscles { get; set; }
    }
}