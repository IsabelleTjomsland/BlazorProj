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

            var result = shifts.Select(s => new ShiftWithEmployeesDto
            {
                ShiftID = s.ShiftID,
                ShiftDate = s.ShiftDate,
                StartTime = FormatTime(s.StartTime),
                EndTime = FormatTime(s.EndTime),
                Description = s.Description,
                Employees = s.Schedules
                    .Where(sc => sc.Employee != null)
                    .Select(sc => new EmployeeCreateDto
                    {
                        EmployeeID = sc.Employee!.EmployeeID,
                        FullName = $"{sc.Employee.FirstName} {sc.Employee.LastName}",
                        Role = sc.Employee.Role
                    })
                    .ToList()
            }).ToList();

            return Ok(result);
        }

        private string FormatTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }
    }
    

    public class ShiftWithEmployeesDto
    {
        public int ShiftID { get; set; }
        public DateTime ShiftDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<EmployeeCreateDto> Employees { get; set; } = new List<EmployeeCreateDto>();
    }

    public class EmployeeCreateDto
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        
    }
}
