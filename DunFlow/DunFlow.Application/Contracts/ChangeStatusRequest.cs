using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Contracts
{
    public class ChangeStatusRequest
    {
        public int TargetStatus { get; set; }
        public int NextAssignedUserId { get; set; }
        public string? CustomFieldsJson { get; set; }
    }
}
