using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Application.DTOs.Tasks;
using TeamTaskManagement.Application.Interfaces;
using TeamTaskManagement.Application.Interfaces.Services;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Exceptions;

namespace TeamTaskManagement.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(ITaskRepository taskRepository, ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<TaskDto>> GetTeamTasksAsync(Guid teamId, Guid userId)
        {
            if (!await _teamRepository.IsUserInTeamAsync(userId, teamId))
                throw new DomainException("Access denied. User is not a member of this team.");

            var tasks = await _taskRepository.GetTeamTasksAsync(teamId);
            return tasks.Select(MapToDto);
        }

        public async Task<TaskDto> CreateTaskAsync(Guid teamId, CreateTaskRequest request, Guid createdByUserId)
        {
            if (!await _teamRepository.IsUserInTeamAsync(createdByUserId, teamId))
                throw new DomainException("Access denied. User is not a member of this team.");

            if (request.AssignedToUserId.HasValue && !await _teamRepository.IsUserInTeamAsync(request.AssignedToUserId.Value, teamId))
                throw new DomainException("Cannot assign task to user who is not a team member.");

            var task = new TaskEntity
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                AssignedToUserId = request.AssignedToUserId,
                CreatedByUserId = createdByUserId,
                TeamId = teamId
            };

            var createdTask = await _taskRepository.CreateAsync(task);
            return MapToDto(createdTask);
        }

        public async Task<TaskDto> UpdateTaskAsync(Guid taskId, UpdateTaskRequest request, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new DomainException("Task not found");

            if (!await _teamRepository.IsUserInTeamAsync(userId, task.TeamId))
                throw new DomainException("Access denied. User is not a member of this team.");

            if (request.AssignedToUserId.HasValue && !await _teamRepository.IsUserInTeamAsync(request.AssignedToUserId.Value, task.TeamId))
                throw new DomainException("Cannot assign task to user who is not a team member.");

            task.Title = request.Title;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.AssignedToUserId = request.AssignedToUserId;
            task.UpdatedAt = DateTime.UtcNow;

            var updatedTask = await _taskRepository.UpdateAsync(task);
            return MapToDto(updatedTask);
        }

        public async Task<TaskDto> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequest request, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new DomainException("Task not found");

            if (!await _teamRepository.IsUserInTeamAsync(userId, task.TeamId))
                throw new DomainException("Access denied. User is not a member of this team.");

            task.Status = request.Status;
            task.UpdatedAt = DateTime.UtcNow;

            var updatedTask = await _taskRepository.UpdateAsync(task);
            return MapToDto(updatedTask);
        }

        public async Task DeleteTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new DomainException("Task not found");

            if (!await _teamRepository.IsUserInTeamAsync(userId, task.TeamId))
                throw new DomainException("Access denied. User is not a member of this team.");

            await _taskRepository.DeleteAsync(taskId);
        }

        private static TaskDto MapToDto(TaskEntity task)
        {
            return new TaskDto(
                task.Id,
                task.Title,
                task.Description,
                task.DueDate,
                task.Status,
                task.CreatedAt,
                task.AssignedToUserId,
                task.AssignedToUser?.FirstName + " " + task.AssignedToUser?.LastName,
                task.CreatedByUserId,
                task.CreatedByUser.FirstName + " " + task.CreatedByUser.LastName,
                task.TeamId
            );
        }
    }
}
