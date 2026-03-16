using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Contracts
{
    public class TaskTypeMetadataDto
    {
        public int TypeValue { get; set; }
        public string TaskType { get; set; }
        public int FinalStatus { get; set; }
        public IDictionary<int, string> Statuses { get; set; } 
    }
}
