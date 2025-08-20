using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Entities.Enums;


namespace TeamTaskManagement.Application.Interfaces
{
    public interface ITeamUserRepository
    {
        Task<TeamUser> AddUserToTeamAsync(Guid userId, Guid teamId, TeamRole role = TeamRole.Member);
        Task<TeamUser?> GetTeamUserAsync(Guid userId, Guid teamId);
        Task<bool> IsUserInTeamAsync(Guid userId, Guid teamId);
    }
}
