namespace Bemanning_System.Backend.Dtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Role { get; set; } = null!; // t.ex. Runner
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SystemRole { get; set; } = null!; // t.ex. Admin eller Employee
    }
}
