using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bemanning_System.Frontend.DTO;

namespace Bemanning_System.Frontend.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EmployeeDto?> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EmployeeDto>();
            }

            return null;
        }
    }
}
