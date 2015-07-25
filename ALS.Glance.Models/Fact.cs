﻿using ALS.Glance.Models.Core;

namespace ALS.Glance.Models
{
    public class Fact : Model<long>
    {
        public decimal AUC { get; set; }


        public long DateId { get; set; }


        public long MuscleId { get; set; }


        public long PatientId { get; set; }

        public long? EmgId { get; set; }
        public long TimeId { get; set; }
        public virtual DEmg EMG { get; set; }
        public virtual DDate Date { get; set; }
        public virtual DMuscle Muscle { get; set; }
        public virtual DPatient Patient { get; set; }
        public virtual DTime Time { get; set; }
    }
}
