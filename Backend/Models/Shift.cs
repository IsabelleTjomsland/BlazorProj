using System;
using System.Collections.Generic;

namespace Bemanning_System.Backend.Models
{
    public class Shift
    {
        public int ShiftID { get; set; }
        public DateTime ShiftDate { get; set; }

        // Ändrat från string till TimeSpan för att matcha databastypen
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string Description { get; set; } = string.Empty;

        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
