public class ShiftDto
{
    public int ShiftID { get; set; }
    public DateTime ShiftDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string ShiftType { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty; // 👈 Ny egenskap för färg
}
