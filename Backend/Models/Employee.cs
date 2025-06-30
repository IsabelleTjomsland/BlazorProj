
namespace Bemanning_System.Backend.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Email { get; set; } = null!;

        // Navigation property - schema (scheman) där denna anställd är kopplad
        public ICollection<Schedule> Schedule { get; set; } = new List<Schedule>();

    }
}
