
namespace DunFlow.Application.Contracts
{
    public class WorkTaskDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int WorkTaskTypeId { get; set; }
        public string? WorkTaskTypeName { get; set; }
        public int CurrentStatus { get; set; }
        public bool IsClosed { get; set; }
        public int AssignedUserId { get; set; }
    }
}
