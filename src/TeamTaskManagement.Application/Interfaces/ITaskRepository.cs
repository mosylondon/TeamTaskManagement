using TeamTaskManagement.Domain.Entities;


namespace TeamTaskManagement.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<TaskEntity>> GetTeamTasksAsync(Guid teamId);
        Task<TaskEntity> CreateAsync(TaskEntity task);
        Task<TaskEntity> UpdateAsync(TaskEntity task);
        Task DeleteAsync(Guid id);
       
    }
}
