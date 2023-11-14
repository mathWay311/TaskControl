using System;

namespace TaskControl.Models
{
    public class TaskDetailViewModel
    {
        public TaskViewModel task { get; set; }
        public TimeSpan AddEstimatedTime { get; set; }
        public TimeSpan AddElapsedTime { get; set; }
    }
}
