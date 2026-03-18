
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
        public int WorkTaskTypeId { get; set; }
        public int AssignedUserId { get; set; }
    }
}
