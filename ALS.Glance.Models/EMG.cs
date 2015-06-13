using System;
using ALS.Glance.Models.Core;

namespace ALS.Glance.Models
{
    public class EMG : ModelWithAllMeta<long>
    {
        public virtual DateTime Date { get; set; }
        public virtual DPatient Patient { get; set; }
        public virtual long PatientId { get; set; }

        public virtual string Data { get; set; }

    }
}
