using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.API.Extensions;
using TeamTaskManagement.Application.DTOs.Tasks;
using TeamTaskManagement.Application.Interfaces.Services;
using TeamTaskManagement.Domain.Exceptions;

namespace TeamTaskManagement.API.Controllers
{
  
    [ApiController]
    [Route("api")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet("teams/{teamId:guid}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTeamTasks(Guid teamId)
        {
            try
            {
                var userId = User.GetUserId();
                var tasks = await _taskService.GetTeamTasksAsync(teamId, userId);
                return Ok(tasks);
            }
            catch (DomainException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("teams/{teamId:guid}/tasks")]
        public async Task<ActionResult<TaskDto>> CreateTask(Guid teamId, [FromBody] CreateTaskRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var task = await _taskService.CreateTaskAsync(teamId, request, userId);
                return CreatedAtAction(nameof(GetTeamTasks), new { teamId }, task);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("tasks/{taskId:guid}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(Guid taskId, [FromBody] UpdateTaskRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var task = await _taskService.UpdateTaskAsync(taskId, request, userId);
                return Ok(task);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("tasks/{taskId:guid}/status")]
        public async Task<ActionResult<TaskDto>> UpdateTaskStatus(Guid taskId, [FromBody] UpdateTaskStatusRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var task = await _taskService.UpdateTaskStatusAsync(taskId, request, userId);
                return Ok(task);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("tasks/{taskId:guid}")]
        public async Task<ActionResult> DeleteTask(Guid taskId)
        {
            try
            {
                var userId = User.GetUserId();
                await _taskService.DeleteTaskAsync(taskId, userId);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
