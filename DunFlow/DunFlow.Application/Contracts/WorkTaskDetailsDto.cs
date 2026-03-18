using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Contracts
{
    public class WorkTaskDetailsDto : WorkTaskDto
    {
        public string? CustomFieldsJson { get; set; } 
        public DateTime CreatedAt { get; set; }
        public bool CanBeClosed { get; set; }
    }
}
