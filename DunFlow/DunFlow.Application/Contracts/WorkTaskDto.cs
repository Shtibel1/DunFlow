using DunFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Contracts
{
    public class WorkTaskDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public TaskType Type { get; set; }
        public int CurrentStatus { get; set; }
        public bool IsClosed { get; set; }
        public int AssignedUserId { get; set; }
    }
}
