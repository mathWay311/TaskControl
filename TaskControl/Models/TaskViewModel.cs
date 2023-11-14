using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskControl.Models
{
    public static  class BindString
    {
        public const string bindString = "ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EstimatedEndDate,ParentID";
    }
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
    public class TaskIndexViewModel
    {
        public List<TaskViewModel> Tasks { get; set; }
    }

    public class TaskDetailViewModel
    {
        public TaskViewModel task { get; set; }
        public TimeSpan AddEstimatedTime { get; set; }
        public TimeSpan AddElapsedTime { get; set; }
    }


    public class SubTaskCreateViewModel
    {
        public TaskViewModel parentModel { get; set; }
    }

    public class TaskDeleteViewModel
    {
        public TaskViewModel parentModel { get; set; }
        public List<TaskViewModel> ChildModels { get; set; }
    }
}
