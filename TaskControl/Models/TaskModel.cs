using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TaskControl.Models
{
    public enum TaskStatus { Assigned, InProgress, Paused, Complete};
    public class TaskModel
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string TaskExecutors { get; set; }
        public DateTime RegistrationDate { get; set; }
        public TaskStatus taskStatus { get; set; }
        public DateTime EndDate { get; set; }
        public int ParentID { get; set; }
    }

    public class TaskDBContext : DbContext
    {
         public DbSet<TaskModel> Task { get; set; }
    }
}
