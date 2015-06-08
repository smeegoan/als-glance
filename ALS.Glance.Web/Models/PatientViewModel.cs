using System;
using System.Collections.Generic;

namespace ALS.Glance.Web.Models
{
    public class PatientViewModel
    {
        public long Id { get; set; }
        public short YearMax { get; set; }

        public short YearMin { get; set; }

        public IEnumerable<Tuple<string,string>> Muscles { get; set; }
    }
}