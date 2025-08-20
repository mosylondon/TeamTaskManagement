using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Application.DTOs.Teams;
using TeamTaskManagement.Application.Interfaces;
using TeamTaskManagement.Application.Interfaces.Services;
using TeamTaskManagement.Domain.Entities.Enums;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Exceptions;

namespace TeamTaskManagement.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITeamUserRepository _teamUserRepository;

        public TeamService(ITeamRepository teamRepository, IUserRepository userRepository, ITeamUserRepository teamUserRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _teamUserRepository = teamUserRepository;
        }

        public async Task<TeamDto> CreateTeamAsync(CreateTeamRequest request, Guid createdByUserId)
        {
            var team = new Team
            {
                Name = request.Name,
                Description = request.Description,
                CreatedByUserId = createdByUserId
            };

            var createdTeam = await _teamRepository.CreateAsync(team);

            // Add the creator as team admin
            await _teamUserRepository.AddUserToTeamAsync(createdByUserId, createdTeam.Id, TeamRole.Admin);

            return new TeamDto(createdTeam.Id, createdTeam.Name, createdTeam.Description, createdTeam.CreatedAt);
        }

        public async Task<TeamDto> AddUserToTeamAsync(Guid teamId, AddUserToTeamRequest request, Guid requestingUserId)
        {
            // Check if requesting user is in the team and has permission
            var requestingUserTeam = await _teamUserRepository.GetTeamUserAsync(requestingUserId, teamId);
            if (requestingUserTeam == null || requestingUserTeam.Role != TeamRole.Admin)
                throw new DomainException("Access denied. Only team admins can add users.");

            // Check if user to add exists
            if (!await _userRepository.ExistsAsync(request.UserId))
                throw new DomainException("User not found");

            // Check if user is already in team
            if (await _teamUserRepository.IsUserInTeamAsync(request.UserId, teamId))
                throw new DomainException("User is already a member of this team");

            await _teamUserRepository.AddUserToTeamAsync(request.UserId, teamId);

            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
                throw new DomainException("Team not found");

            return new TeamDto(team.Id, team.Name, team.Description, team.CreatedAt);
        }

        public async Task<IEnumerable<TeamDto>> GetUserTeamsAsync(Guid userId)
        {
            var teams = await _teamRepository.GetUserTeamsAsync(userId);
            return teams.Select(t => new TeamDto(t.Id, t.Name, t.Description, t.CreatedAt));
        }
    }
}
