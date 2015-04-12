using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALS.Glance.Models.Core;

namespace ALS.Glance.Models
{
    public class DPatient : ModelWithAllMeta<long>
    {
        public string PatientId { get; set; }
        public string Sex { get; set; }
        public string Name { get; set; }


        public DateTimeOffset BornOn { get; set; }


        public DateTimeOffset DiagnosedOn { get; set; }

        public virtual ICollection<Fact> Fact { get; set; }


        public DPatient()
        {
            Fact = new List<Fact>();
        }
    }
}
