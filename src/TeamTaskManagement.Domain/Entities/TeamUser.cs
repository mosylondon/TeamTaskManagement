using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Domain.Common;
using TeamTaskManagement.Domain.Entities.Enums;

namespace TeamTaskManagement.Domain.Entities
{
    public class TeamUser : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
        public TeamRole Role { get; set; } //= TeamRole.Member;
        public virtual User User { get; set; } = null!;
        public virtual Team Team { get; set; } = null!;
    }
}
