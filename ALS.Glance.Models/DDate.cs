﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALS.Glance.Models.Core;

namespace ALS.Glance.Models
{
    public class DDate : ModelWithAllMeta<long>
    {     
        public DateTime Date { get; set; }

        public byte Day { get; set; }

        public string DayInMonth { get; set; }

        public byte Month { get; set; }

        public string MonthName { get; set; }

        public short Year { get; set; }

        public string DayOfWeek { get; set; }

        public string DayOfWeekName { get; set; }

        public string Weekday { get; set; }

        public string MonthInYear { get; set; }

        public byte Quarter { get; set; }

        public string QuarterInYear { get; set; }

        public virtual ICollection<Fact> Fact { get; set; }

        public DDate()
        {
            Fact = new List<Fact>();
        }
    }
}