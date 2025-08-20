using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.API.Extensions;
using TeamTaskManagement.Application.DTOs.Teams;
using TeamTaskManagement.Application.Interfaces.Services;
using TeamTaskManagement.Domain.Exceptions;

namespace TeamTaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
        {
            _teamService = teamService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var team = await _teamService.CreateTeamAsync(request, userId);
                _logger.LogInformation("Team created successfully: {TeamName} by user {UserId}", team.Name, userId);
                return CreatedAtAction(nameof(GetUserTeams), team);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{teamId:guid}/users")]
        public async Task<ActionResult<TeamDto>> AddUserToTeam(Guid teamId, [FromBody] AddUserToTeamRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var team = await _teamService.AddUserToTeamAsync(teamId, request, userId);
                _logger.LogInformation("User {UserId} added to team {TeamId}", request.UserId, teamId);
                return Ok(team);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetUserTeams()
        {
            var userId = User.GetUserId();
            var teams = await _teamService.GetUserTeamsAsync(userId);
            return Ok(teams);
        }
    }
}
