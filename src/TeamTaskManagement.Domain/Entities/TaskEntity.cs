using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTaskManagement.Domain.Common;
//using TeamTaskManagement.Domain.Entities.Enums;

namespace TeamTaskManagement.Domain.Entities
{
    public class TaskEntity : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; } //= TaskStatus.Pending;
        public Guid? AssignedToUserId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid TeamId { get; set; }
        public virtual User? AssignedToUser { get; set; }
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual Team Team { get; set; } = null!;
    }
}
