using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Application.Interfaces;
using TeamTaskManagement.Application.Interfaces.Services;
using static TeamTaskManagement.Application.DTOs.Auth.AuthDto;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Exceptions;

namespace TeamTaskManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IPasswordService passwordService, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new DomainException("User with this email already exists");

            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = _passwordService.HashPassword(request.Password)
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var userDto = new UserDto(createdUser.Id, createdUser.Email, createdUser.FirstName, createdUser.LastName);
            var token = _jwtService.GenerateToken(userDto);

            return new AuthResponse(token, userDto);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                throw new DomainException("Invalid email or password");

            var userDto = new UserDto(user.Id, user.Email, user.FirstName, user.LastName);
            var token = _jwtService.GenerateToken(userDto);

            return new AuthResponse(token, userDto);
        }
    }
}
