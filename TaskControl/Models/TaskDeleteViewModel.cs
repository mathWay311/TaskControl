using System.Collections.Generic;

namespace TaskControl.Models
{
    public class TaskDeleteViewModel
    {
        public TaskViewModel parentModel { get; set; }
        public List<TaskViewModel> ChildModels { get; set; }
    }
}
