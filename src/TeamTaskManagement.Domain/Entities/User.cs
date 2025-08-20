using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Domain.Common;

namespace TeamTaskManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public virtual ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
        public virtual ICollection<TaskEntity> AssignedTasks { get; set; } = new List<TaskEntity>();
        public virtual ICollection<TaskEntity> CreatedTasks { get; set; } = new List<TaskEntity>();
    }
}
