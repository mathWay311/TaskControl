using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskControl.DAL;
using TaskControl.Service.DTO;
using TaskControl_Service;

namespace TaskControl.Service
{ 
    public class TaskService : ITaskService
    {
        private readonly TaskDBContext _context;
        private readonly ILogger<TaskService> _logger;
        private readonly IMapper _mapper;   

        public TaskService(ILogger<TaskService> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _context = new TaskDBContext();
        }
        public Task TaskCreate(TaskDto task)
        {
            task.ParentID = null;
            task.RegistrationDate = DateTime.Now;

            var entity = _mapper.Map<TaskDto, DAL.Entity.Task>(task);

            _logger.LogInformation("Creating new task entity");
            _context.Task.Add(entity);

            return _context.SaveChangesAsync();
        }

        public Task SubTaskCreate(TaskDto task)
        {
            task.ID = 0;
            task.RegistrationDate = DateTime.Now;

            var entity = _mapper.Map<TaskDto, DAL.Entity.Task>(task);
            _context.Task.Add(entity);

            _logger.LogInformation("Creating new subtask entity");
            _context.Task.Add(entity);

            return _context.SaveChangesAsync();
        }

        public List<TaskDto> getAllTask()
        {
            var tasks = _mapper.Map<List<DAL.Entity.Task>, List<TaskDto>>(_context.Task.ToList());
            return tasks;
        }

        public TaskDto Find(int id)
        {
            var task = _mapper.Map<DAL.Entity.Task, TaskDto>(_context.Task.Find(id));
            return task;
        }

        public Task Update(int id, TaskDto task)
        {
            var entity = _mapper.Map<TaskDto, DAL.Entity.Task>(task);
            entity.ID = id;
            _context.Update(entity);
            return _context.SaveChangesAsync();
        }

        public Task Delete(int id)
        {
                var task = _mapper.Map<TaskDto, DAL.Entity.Task> (Find(id));
                var childrenTasks = _mapper.Map<List<TaskDto>, List <DAL.Entity.Task>> (getAllChildrenOfTask(id));

                foreach (var childTask in childrenTasks)
                {
                    _context.Task.Remove(childTask);
                }
                _context.Task.Remove(task);
                return _context.SaveChangesAsync();
        }

        public Task ChangeStatus(int id, DTO.TaskStatus status)
        {
            DateTime endDate = DateTime.Now;

            var task = Find(id);

            switch (status)
            {
                case DTO.TaskStatus.Complete:
                    var childrenTasks = getAllChildrenOfTask(id);
                    foreach(var childTask in childrenTasks)
                    {
                        if (childTask.taskStatus != DTO.TaskStatus.InProgress && childTask.taskStatus != DTO.TaskStatus.Complete)
                        {
                            return System.Threading.Tasks.Task.FromResult<object>(null);

                        }
                        childTask.taskStatus = DTO.TaskStatus.Complete;
                        childTask.EndDate = endDate;
                    }
                    foreach (var childTask in childrenTasks)
                    {
                        var childEntity = _mapper.Map<TaskDto, DAL.Entity.Task>(childTask);
                        _context.Update(childEntity);
                    }
                    if (task.taskStatus == DTO.TaskStatus.InProgress)
                    {
                        task.taskStatus = DTO.TaskStatus.Complete;
                        task.EndDate = endDate;
                    }
                        
                    else
                    {
                        return System.Threading.Tasks.Task.FromResult<object>(null);
                    }
                    break;

                case DTO.TaskStatus.Paused:
                    if (task.taskStatus == DTO.TaskStatus.InProgress)
                        task.taskStatus = DTO.TaskStatus.Paused;
                    break;
                case DTO.TaskStatus.InProgress:
                    if (task.taskStatus != DTO.TaskStatus.Complete)
                        task.taskStatus = DTO.TaskStatus.InProgress;
                    break;  
            }
            var entity = _mapper.Map<TaskDto, DAL.Entity.Task>(task);

            _context.Update(entity);
            return _context.SaveChangesAsync(); 
        }

        public List<TaskDto> getAllChildrenOfTask(int id)
        {
            List<TaskDto> ChildrenTasks = new List<TaskDto>();
            Queue<TaskDto> toVisit = new Queue<TaskDto>();

            var parentTask = Find(id);
            var allTasks = _mapper.Map<List<DAL.Entity.Task>, List<TaskDto>> ((from ts in _context.Task where id != ts.ID select ts).ToList());

            toVisit.Enqueue(parentTask);
            TaskDto currentNode = toVisit.Peek();

            while (toVisit.Count != 0)
            {
                currentNode = toVisit.Peek();
                foreach (TaskDto task in allTasks)
                {
                    if (task.ParentID == currentNode.ID)
                    {
                        ChildrenTasks.Add(task);
                        toVisit.Enqueue(task);
                    }
                }
                toVisit.Dequeue();
            }
            return ChildrenTasks;
        }

        public Dictionary<string, TimeSpan> calculateSubTaskTime(int id)
        {
            var parentTask = Find(id);
            var childTasks = getAllChildrenOfTask(id);
            Dictionary<string, TimeSpan> times = new Dictionary<string, TimeSpan>
            {
                { "Estimated", TimeSpan.FromSeconds(0)},
                { "Elapsed", TimeSpan.FromSeconds(0)}
            };

            TimeSpan totalAdditionalEstimatedTime = TimeSpan.FromSeconds(0);

            foreach (var childTask in childTasks)
            {
                totalAdditionalEstimatedTime += childTask.EstimatedEndDate - childTask.RegistrationDate;
            }
            times["Estimated"] += totalAdditionalEstimatedTime;

            if (parentTask.taskStatus == DTO.TaskStatus.Complete)
            {
                TimeSpan totalAdditionalElapsedTime = TimeSpan.FromSeconds(0);
                foreach (var childTask in childTasks)
                {
                    totalAdditionalElapsedTime += childTask.RegistrationDate - (DateTime)childTask.EndDate;
                }
                times["Elapsed"] += totalAdditionalElapsedTime;
            }

            return times;
        }

    }
}


