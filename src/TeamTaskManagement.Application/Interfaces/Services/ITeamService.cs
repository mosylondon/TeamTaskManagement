using TeamTaskManagement.Application.DTOs.Teams;

namespace TeamTaskManagement.Application.Interfaces.Services
{
    public interface ITeamService
    {
        Task<TeamDto> CreateTeamAsync(CreateTeamRequest request, Guid createdByUserId);
        Task<TeamDto> AddUserToTeamAsync(Guid teamId, AddUserToTeamRequest request, Guid requestingUserId);
        Task<IEnumerable<TeamDto>> GetUserTeamsAsync(Guid userId);
    }
}
