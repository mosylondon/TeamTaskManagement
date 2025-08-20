using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManagement.Application.DTOs.Teams
{

    public record TeamDto(Guid Id, string Name, string Description, DateTime CreatedAt);
    public record CreateTeamRequest(string Name, string Description);
    public record AddUserToTeamRequest(Guid UserId);
}
