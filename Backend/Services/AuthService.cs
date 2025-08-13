using System.Data;
using Microsoft.Data.SqlClient; // För SqlConnection
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dapper; // För Dapper
using BCrypt.Net; // För lösenordshashning
using Bemanning_System.Backend.Models;
namespace Bemanning_System.Backend.Services;
public class AuthService
{
    private readonly IDbConnection _connection;

    public AuthService(IConfiguration config)
    {
        _connection = new Microsoft.Data.SqlClient.SqlConnection(config.GetConnectionString("DefaultConnection"));
    }

    public async Task<Employee?> LoginAsync(string email, string password)
    {
        string sql = "SELECT * FROM Employees WHERE Email = @Email";
        var employee = await _connection.QueryFirstOrDefaultAsync<Employee>(sql, new { Email = email });

        if (employee == null || !BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
            return null;

        return employee;
    }

    public async Task<bool> RegisterAsync(Employee newEmployee, string password)
    {
        string hash = BCrypt.Net.BCrypt.HashPassword(password);

        string sql = @"INSERT INTO Employees (FirstName, LastName, Role, Email, PasswordHash)
                       VALUES (@FirstName, @LastName, @Role, @Email, @PasswordHash)";

        int rows = await _connection.ExecuteAsync(sql, new
        {
            newEmployee.FirstName,
            newEmployee.LastName,
            newEmployee.Role,
            newEmployee.Email,
            PasswordHash = hash
        });

        return rows > 0;
    }
}
