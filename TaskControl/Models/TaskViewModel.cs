using System;
using System.ComponentModel.DataAnnotations;

namespace TaskControl.Models
{
    public enum TaskStatus
    {
        [Display(Name = "Назначена")]
        Assigned,

        [Display(Name = "Выполняется")]
        InProgress,

        [Display(Name = "Приостановлена")]
        Paused,

        [Display(Name = "Завершена")]
        Complete
    };
    public class TaskViewModel
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
