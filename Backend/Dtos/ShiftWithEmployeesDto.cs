public class ShiftWithEmployeesDto
{
    public int ShiftID { get; set; }
    public DateTime ShiftDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Lista av anställda som jobbar på detta skift
    public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
}

public class EmployeeDto
{
    public int EmployeeID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
