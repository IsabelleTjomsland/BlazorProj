public class DayScheduleDto
{
    public string Weekday { get; set; } = string.Empty; // "Måndag", "Tisdag", ...
    public DateTime Date { get; set; }                  // 2025-06-29
    public List<ShiftDto> Shifts { get; set; } = new();
}
