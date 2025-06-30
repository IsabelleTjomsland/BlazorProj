using System.Text.Json.Serialization;

namespace Bemanning_System.Backend.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }

        public int EmployeeID { get; set; }
        
        [JsonIgnore]
        public Employee? Employee { get; set; }

        public int ShiftID { get; set; }

        [JsonIgnore]
        public Shift? Shift { get; set; }
        
        // Ta bort alla kolumner som inte finns i Schedules-tabellen
        // dvs ingen Date, StartTime, EndTime eller Description här
    }
}
