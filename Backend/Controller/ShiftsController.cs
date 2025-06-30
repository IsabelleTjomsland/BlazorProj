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
        public async Task<IActionResult> GetAllShifts()
        {
            var shiftsWithEmployees = await _context.Shifts
                .Include(s => s.Schedules)
                    .ThenInclude(sc => sc.Employee)
                .ToListAsync();

            var shiftDtos = shiftsWithEmployees
                .SelectMany(s => s.Schedules.Select(sc =>
                {
                    var shiftType = GetShiftType(s.StartTime);
                    return new ShiftDto
                    {
                        ShiftID = s.ShiftID,
                        ShiftDate = s.ShiftDate,
                        StartTime = FormatTime(s.StartTime),
                        EndTime = FormatTime(s.EndTime),
                        Description = s.Description,
                        EmployeeName = $"{sc.Employee.FirstName} {sc.Employee.LastName}",
                        Role = sc.Employee.Role,
                        ShiftType = shiftType,
                        Color = GetShiftColor(shiftType) // 🎨 Färg
                    };
                }))
                .ToList();

            return Ok(shiftDtos);
        }

        private string FormatTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }

        private string GetShiftType(TimeSpan startTime)
        {
            if (startTime.Hours >= 6 && startTime.Hours < 14)
                return "Dag";
            if (startTime.Hours >= 14 && startTime.Hours < 22)
                return "Kväll";
            return "Natt";
        }

        private string GetShiftColor(string shiftType)
        {
            return shiftType switch
            {
                "Dag" => "#A8E6CF",    // Grön
                "Kväll" => "#FFD3B6",  // Orange
                "Natt" => "#DCE3FF",   // Blå/lila
                _ => "#FFFFFF"         // Vit
            };
        }
    }
}
