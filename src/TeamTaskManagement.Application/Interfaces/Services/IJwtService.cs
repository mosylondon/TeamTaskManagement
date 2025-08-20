using static TeamTaskManagement.Application.DTOs.Auth.AuthDto;

namespace TeamTaskManagement.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserDto user);
        Guid? GetUserIdFromToken(string token);
    }
}
