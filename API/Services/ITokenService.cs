using API.Models;

namespace API.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}