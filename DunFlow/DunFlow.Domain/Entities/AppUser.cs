using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public required string FullName { get; set; }
        public ICollection<WorkTask> AssignedTasks { get; set; } = new List<WorkTask>();
    }
}
