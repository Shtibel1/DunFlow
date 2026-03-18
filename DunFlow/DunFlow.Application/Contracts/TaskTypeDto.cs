using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Contracts
{
    public class TaskTypeDto
    {
        public int TypeValue { get; set; }
        public required string TaskType { get; set; }
        public int FinalStatus { get; set; }
        public List<WorkTaskStatusDto> Statuses { get; set; } = new();
    }
}
