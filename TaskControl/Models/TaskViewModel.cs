using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskControl.Models
{
    public class TaskViewModel
    {
        public List<TaskModel> Tasks { get; set; }
    }

    public class TaskDetailViewModel
    {
        public TaskModel task { get; set; }
        public TimeSpan AddEstimatedTime { get; set; }
        public TimeSpan AddElapsedTime { get; set; }
    }


    public class SubTaskCreateViewModel
    {
        public TaskModel parentModel { get; set; }
    }

    public class TaskDeleteViewModel
    {
        public TaskModel parentModel { get; set; }
        public List<TaskModel> ChildModels { get; set; }
    }
}
