using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALS.Glance.Models.Core;

namespace ALS.Glance.Models
{
    public class DMuscle : ModelWithAllMeta<long>
    {
        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public virtual ICollection<Fact> Fact { get; set; }


        public DMuscle()
        {
            Fact = new List<Fact>();
        }
    }
}
