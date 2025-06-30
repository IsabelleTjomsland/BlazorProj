public class WeekScheduleDto
{
    public int WeekNumber { get; set; }
    public List<DayScheduleDto> Days { get; set; } = new();
}
