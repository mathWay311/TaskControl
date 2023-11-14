
namespace TaskControl.Service.DTO
{
    public enum TaskStatus
    {
        Assigned,

        InProgress,

        Paused,

        Complete
    };
    public class TaskDto
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public string? Description { get; set; }
        public string? TaskExecutors { get; set; }
        public DateTime RegistrationDate { get; set; }
        public TaskStatus taskStatus { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ParentID { get; set; }
        public DateTime EstimatedEndDate { get; set; }
    }
}
