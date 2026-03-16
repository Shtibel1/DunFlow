using DunFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Contracts
{
    public class CreateTaskRequest
    {
        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TaskType Type { get; set; }
        public int AssignedUserId { get; set; }
    }
}
