using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.Entities
{
    public class WorkTaskStatus
    {
        public int Id { get; set; }

        public int WorkTaskTypeId { get; set; }
        public WorkTaskType? WorkTaskType { get; set; }

        public int StatusValue { get; set; }

        [Required]
        [StringLength(100)]
        public required string DisplayName { get; set; }

        public bool IsFinal { get; set; }

        public ICollection<FormField> FormFields { get; set; } = new List<FormField>();
    }
}
