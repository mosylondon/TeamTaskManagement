using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManagement.Application.DTOs.Auth
{
    public class AuthDto
    {
        public record RegisterRequest(string Email, string FirstName, string LastName, string Password);
        public record LoginRequest(string Email, string Password);
        public record AuthResponse(string Token, UserDto User);

        public record UserDto(Guid Id, string Email, string FirstName, string LastName);
    }
}
