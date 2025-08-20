using TeamTaskManagement.Application.DTOs.Tasks;

namespace TeamTaskManagement.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTeamTasksAsync(Guid teamId, Guid userId);
        Task<TaskDto> CreateTaskAsync(Guid teamId, CreateTaskRequest request, Guid createdByUserId);
        Task<TaskDto> UpdateTaskAsync(Guid taskId, UpdateTaskRequest request, Guid userId);
        Task<TaskDto> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequest request, Guid userId);
        Task DeleteTaskAsync(Guid taskId, Guid userId);
    }
}
