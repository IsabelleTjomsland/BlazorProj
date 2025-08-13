namespace Bemanning_System.Backend.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Email { get; set; } = null!;
        
        // 🔐 
        public string PasswordHash { get; set; } = null!;

        public int UserID { get; set; } // Foreign Key
        public User User { get; set; } = null!;

        public ICollection<Schedule> Schedule { get; set; } = new List<Schedule>();
    }
}
