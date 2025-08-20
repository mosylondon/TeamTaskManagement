using TeamTaskManagement.Domain.Entities;


namespace TeamTaskManagement.Application.Interfaces
{
    public interface ITeamRepository
    {
        Task<Team?> GetByIdAsync(Guid id);
        Task<Team> CreateAsync(Team team);
        Task<IEnumerable<Team>> GetUserTeamsAsync(Guid userId);
        Task<bool> IsUserInTeamAsync(Guid userId, Guid teamId);
    }
}
