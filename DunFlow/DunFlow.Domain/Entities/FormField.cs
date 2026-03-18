using System.ComponentModel.DataAnnotations;

namespace DunFlow.Domain.Entities
{
    public class FormField
    {
        public int Id { get; set; }

        public int WorkTaskStatusId { get; set; }
        public WorkTaskStatus? WorkTaskStatus { get; set; }

        [Required]
        [StringLength(100)]
        public required string Key { get; set; }

        [Required]
        [StringLength(100)]
        public required string Label { get; set; }

        [Required]
        [StringLength(50)]
        public required string ControlType { get; set; }

        public bool IsRequired { get; set; }
    }
}
