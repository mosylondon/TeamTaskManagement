using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.API.Extensions;
using TeamTaskManagement.Application.Interfaces;
using static TeamTaskManagement.Application.DTOs.Auth.AuthDto;

namespace TeamTaskManagement.API.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = User.GetUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return NotFound();

            var userDto = new UserDto(user.Id, user.Email, user.FirstName, user.LastName);
            return Ok(userDto);
        }
    }
}
