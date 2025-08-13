using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using BCrypt.Net;
using Bemanning_System.Backend.Models;
using Bemanning_System.Backend.Dtos;
using Bemanning_System.Backend.Data;

namespace Bemanning_System.Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly StaffingContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(StaffingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Hämta user från Users-tabellen
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Fel e-post eller lösenord" });
                }

                // Hämta kopplad employee för att få för- och efternamn
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserID == user.UserID);

                // Om employee finns, använd hela namnet, annars ta mejl-prefix
                var displayName = employee != null
                    ? $"{employee.FirstName} {employee.LastName}"
                    : user.Email.Split('@')[0];

                var token = GenerateJwtToken(user, displayName);

                return Ok(new
                {
                    Token = token,
                    SystemRole = user.SystemRole,
                    Position = employee?.Role
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return StatusCode(500, new { message = "Ett internt serverfel uppstod.", error = ex.Message });
            }
        }

        // ✅ REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { message = "E-postadressen används redan" });
            }

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                SystemRole = string.IsNullOrWhiteSpace(registerDto.SystemRole) ? "Employee" : registerDto.SystemRole
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var employee = new Employee
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = registerDto.Role,
                Email = registerDto.Email,
                UserID = user.UserID
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Användare registrerad" });
        }

        // ✅ JWT TOKEN-GENERERING med namn
        private string GenerateJwtToken(User user, string displayName)
        {
            var systemRole = string.IsNullOrWhiteSpace(user.SystemRole) ? "Employee" : user.SystemRole;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, displayName), // 👈 Hela namnet i JWT
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, systemRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
