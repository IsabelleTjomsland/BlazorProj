namespace Bemanning_System.Backend.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!; // Jobbroll (valfritt att flytta bort)
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string SystemRole { get; set; } = null!; // Ex: Admin, Employee

        // Lägg till denna rad:
        public Employee? Employee { get; set; }
    }
}
