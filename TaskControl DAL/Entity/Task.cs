using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskControl.DAL.Entity
{
    public enum TaskStatus
    {
        Assigned,

        InProgress,

        Paused,

        Complete
    };
    public class Task
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
