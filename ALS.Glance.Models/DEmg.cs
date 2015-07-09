using System.Collections.Generic;
using ALS.Glance.Models.Core;

namespace ALS.Glance.Models
{
    public class DEmg : ModelWithAllMeta<long>
    {

        public string Data { get; set; }

        public virtual ICollection<Fact> Fact { get; set; }


        public DEmg()
        {
            Fact = new List<Fact>();
        }
    }
}
