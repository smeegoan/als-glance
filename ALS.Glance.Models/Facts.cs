using System;
using ALS.Glance.Models.Core;

namespace ALS.Glance.Models
{

        public class Facts: Model<long>
        {

            public decimal AUC { get; set; } // AUC


            public DateTime Date { get; set; } // date


            public byte Day { get; set; } // Day


            public string DayInMonth { get; set; } // DayInMonth


            public byte Month { get; set; } // Month


            public string MonthName { get; set; } // MonthName


            public short Year { get; set; } // Year


            public string DayOfWeek { get; set; } // DayOfWeek


            public string DayOfWeekName { get; set; } // DayOfWeekName


            public string Weekday { get; set; } // Weekday


            public string MonthInYear { get; set; } // MonthInYear


            public byte Quarter { get; set; } // Quarter


            public string QuarterInYear { get; set; } // QuarterInYear


            public string MuscleAbbreviation { get; set; } // MuscleAbbreviation


            public string Muscle { get; set; } // Muscle


            public string PatientId { get; set; } // PatientId


            public string Name { get; set; } // Name


            public string Sex { get; set; } // Sex


            public DateTimeOffset BornOn { get; set; } // BornOn


            public DateTimeOffset DiagnosedOn { get; set; } // DiagnosedOn


            public short Hour { get; set; } // Hour


            public string TimeOfDay { get; set; } // TimeOfDay
        }

    

}
