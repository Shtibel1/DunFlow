using DunFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.Entities
{
    public class WorkTask
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public required string Title { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }

        public TaskType Type { get; set; }

        public int CurrentStatus { get; set; }

        public bool IsClosed { get; set; }

        public int AssignedUserId { get; set; }
        public AppUser? AssignedUser { get; set; }

        public string CustomFieldsJson { get; set; } = "{}";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
