namespace Bemanning_System.Frontend.DTO;

public class ShiftWithEmployeesDto
{
    public int ShiftID { get; set; }
    public DateTime ShiftDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // List of employees for this shift
    public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
}

public class EmployeeDto
{
    public int EmployeeID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty; // ✅ Added token to fix login error
}
