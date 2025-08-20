using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManagement.Application.DTOs.Tasks
{
 
    public record TaskDto(
    Guid Id,
    string Title,
    string Description,
    DateTime? DueDate,
    TaskStatus Status,
    DateTime CreatedAt,
    Guid? AssignedToUserId,
    string? AssignedToUserName,
    Guid CreatedByUserId,
    string CreatedByUserName,
    Guid TeamId
     );

    public record CreateTaskRequest(
        string Title,
        string Description,
        DateTime? DueDate,
        Guid? AssignedToUserId
    );

    public record UpdateTaskRequest(
        string Title,
        string Description,
        DateTime? DueDate,
        Guid? AssignedToUserId
    );

    public record UpdateTaskStatusRequest(TaskStatus Status);
  
}
