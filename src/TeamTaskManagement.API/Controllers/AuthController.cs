using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Application.Interfaces;
using TeamTaskManagement.Application.Interfaces.Services;
using TeamTaskManagement.Domain.Exceptions;
using static TeamTaskManagement.Application.DTOs.Auth.AuthDto;

namespace TeamTaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, IUserRepository userRepository, ILogger<AuthController> logger)
        {
            _authService = authService;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                _logger.LogInformation("User registered successfully: {Email}", request.Email);
                return Ok(response);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("Registration failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                _logger.LogInformation("User logged in successfully: {Email}", request.Email);
                return Ok(response);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("Login failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
