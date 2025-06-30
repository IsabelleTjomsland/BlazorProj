using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bemanning_System.Backend.Data;
using Bemanning_System.Backend.Models;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Bemanning_System.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftsController : ControllerBase
    {
        private readonly StaffingContext _context;

        public ShiftsController(StaffingContext context)
        {
            _context = context;
        }

        // GET: api/shifts
        [HttpGet]
        public async Task<IActionResult> GetShiftsWithEmployees()
        {
            var shifts = await _context.Shifts
                .Include(s => s.Schedules)
                    .ThenInclude(sc => sc.Employee)
                .ToListAsync();

            var result = shifts.Select(s =>
            {
                var shiftType = GetShiftType(s.StartTime);
                var color = GetColorForShiftType(shiftType);

                return new ShiftWithEmployeesDto
                {
                    ShiftID = s.ShiftID,
                    ShiftDate = s.ShiftDate,
                    StartTime = FormatTime(s.StartTime),
                    EndTime = FormatTime(s.EndTime),
                    Description = s.Description,
                    ShiftType = shiftType,
                    Color = color,
                    Employees = s.Schedules
                        .Where(sc => sc.Employee != null)
                        .Select(sc => new EmployeeCreateDto
                        {
                            EmployeeID = sc.Employee!.EmployeeID,
                            FullName = $"{sc.Employee.FirstName} {sc.Employee.LastName}",
                            Role = sc.Employee.Role
                        })
                        .ToList()
                };
            }).ToList();

            return Ok(result);
        }

        private string FormatTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }

        private string GetShiftType(TimeSpan startTime)
        {
            if (startTime.Hours >= 5 && startTime.Hours < 12)
                return "Morgon"; // 05:00 - 12:00
            else if (startTime.Hours >= 12 && startTime.Hours < 17)
                return "Dag";    // 12:00 - 17:00
            else if (startTime.Hours >= 17 && startTime.Hours < 22)
                return "Kväll";  // 17:00 - 22:00
            else
                return "Natt";   // 22:00 - 05:00
        }

        private string GetColorForShiftType(string shiftType)
        {
            return shiftType switch
            {
                "Morgon" => "#FFD700", // Guldgul
                "Dag" => "#87CEFA",    // Ljusblå
                "Kväll" => "#FF8C00",  // Mörkorange
                "Natt" => "#2F4F4F",   // Mörkgrå
                _ => "#FFFFFF",        // Vit fallback
            };
        }
    }

    public class ShiftWithEmployeesDto
    {
        public int ShiftID { get; set; }
        public DateTime ShiftDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ShiftType { get; set; } = string.Empty; // 👈 Typ av skift
        public string Color { get; set; } = string.Empty;     // 👈 Färgkod baserat på typ
        public List<EmployeeCreateDto> Employees { get; set; } = new List<EmployeeCreateDto>();
    }

    public class EmployeeCreateDto
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
