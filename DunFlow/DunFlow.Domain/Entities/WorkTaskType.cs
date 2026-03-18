using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.Entities
{
    public class WorkTaskType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public ICollection<WorkTaskStatus> Statuses { get; set; } = new List<WorkTaskStatus>();
        public ICollection<WorkTask> WorkTasks { get; set; } = new List<WorkTask>();
    }
}
