using System;
using System.Collections.Generic;

namespace ALS.Glance.Web.Models
{
    public class PatientViewModel
    {
        public long Id { get; set; }

        public IEnumerable<Tuple<int,bool>> Years { get; set; }

        public IEnumerable<Tuple<string,string,bool>> Muscles { get; set; }
    }
}