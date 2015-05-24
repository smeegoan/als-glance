using System;
using System.Collections.Generic;

namespace ALS.Glance.Web.Models
{
    public class PatientViewModel
    {
        public long Id { get; set; }

        public IEnumerable<Tuple<string,string>> Muscles { get; set; }
    }
}