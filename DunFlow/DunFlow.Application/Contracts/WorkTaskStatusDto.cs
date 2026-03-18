using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Contracts
{
    public class WorkTaskStatusDto
    {
        public int StatusValue { get; set; }
        public required string DisplayName { get; set; }
    }
}
