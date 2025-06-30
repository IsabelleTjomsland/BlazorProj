using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bemanning_System.Backend.Models;
using Bemanning_System.Backend.Data;

namespace Bemanning_System.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly StaffingContext _context;

        public SchedulesController(StaffingContext context)
        {
            _context = context;
        }

        // GET: api/schedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAllSchedules()
        {
            return await _context.Schedules.ToListAsync();
        }

        // GET: api/schedules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            return schedule;
        }

        // POST: api/schedules
        [HttpPost]
        public async Task<ActionResult<Schedule>> CreateSchedule([FromBody] Schedule schedule)
        {
            if (schedule.EmployeeID <= 0 || schedule.ShiftID <= 0)
            {
                return BadRequest("Både employeeID och shiftID måste anges och vara större än 0.");
            }

            var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeID == schedule.EmployeeID);
            if (!employeeExists)
            {
                return BadRequest($"Employee med ID {schedule.EmployeeID} finns inte.");
            }

            var shiftExists = await _context.Shifts.AnyAsync(s => s.ShiftID == schedule.ShiftID);
            if (!shiftExists)
            {
                return BadRequest($"Shift med ID {schedule.ShiftID} finns inte.");
            }

            schedule.ScheduleID = 0;

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.ScheduleID }, schedule);
        }
    }
}
