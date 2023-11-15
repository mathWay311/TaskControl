using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskControl.Service.DTO;

namespace TaskControl_Service
{
    public interface ITaskService
    {
        public List<TaskDto> getAllTask();

        public Task TaskCreate(TaskDto task);
        public Task SubTaskCreate(TaskDto task);
        public Task Update(int id, TaskDto task);
        public Task Delete(int id);


        public Task ChangeStatus(int id, TaskControl.Service.DTO.TaskStatus status);

        public TaskDto Find(int id);
        public List<TaskDto> getAllChildrenOfTask(int id);
        public Dictionary<string, TimeSpan> calculateSubTaskTime(int id);

    }
}
