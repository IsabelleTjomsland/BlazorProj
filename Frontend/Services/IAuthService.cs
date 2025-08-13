using System.Threading.Tasks;
using Bemanning_System.Frontend.DTO;

namespace Bemanning_System.Frontend.Services
{
    public interface IAuthService
    {
        Task<EmployeeDto?> LoginAsync(string email, string password);
    }
}
