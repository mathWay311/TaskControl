using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TaskControl.DAL.Entity;
using TaskControl.Models;
using TaskControl.Service;
using TaskControl.Service.DTO;

namespace TaskControl.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITaskService _service;
        private readonly IMapper _mapper;
        public HomeController(ILogger<HomeController> logger, ITaskService service, IMapper mapper)
        {
            _logger = logger;
            _service = service;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var tasks = _mapper.Map<List<TaskDto>, List<TaskViewModel>>(_service.TaskIndex()) ;

            var TaskIndexVM = new TaskIndexViewModel
            {
                Tasks = tasks
            };
            ViewBag.Json = JsonSerializer.Serialize(TaskModelUtils.GetTreeJson(tasks));

            return View(TaskIndexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CreateTask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask([Bind(BindString.bindString)] TaskViewModel taskVM)
        {
            if (ModelState.IsValid)
            {
                _service.TaskCreate(_mapper.Map<TaskViewModel, TaskDto>(taskVM));
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public ActionResult CreateSubTask(int id)
        {
            var parentTask = _mapper.Map<TaskDto, TaskViewModel>(_service.Find(id));

            if (parentTask == null) return NotFound();

            ViewData["ParentModel"] = parentTask;

            return PartialView("_PartialCreateSubTask");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSubTask([Bind(BindString.bindString)] TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                _service.SubTaskCreate(_mapper.Map<TaskViewModel, TaskDto>(task));
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        public IActionResult TaskDetails(int id)
        {
            var task = _mapper.Map < TaskDto, TaskViewModel> (_service.Find(id));

            if (task == null) return NotFound();

            Dictionary<string, TimeSpan> times = _service.SubTaskTime(id);
            
            TaskDetailViewModel taskVM = new TaskDetailViewModel
            {
                task = task,
                AddEstimatedTime = times["Estimated"],
                AddElapsedTime = times["Elapsed"]
            };
            
            return PartialView("_PartialDetails", taskVM);
        }

        public ActionResult TaskEdit(int id)
        {
            var task = _mapper.Map<TaskDto, TaskViewModel>(_service.Find(id));
            return PartialView("_PartialTaskEdit", task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskEdit(int id, [Bind(BindString.bindString)] TaskViewModel task)
        {
            if(ModelState.IsValid)
            {
                var TaskDto = _mapper.Map<TaskViewModel, TaskDto>(task);
                await _service.Edit(id, TaskDto);

                return RedirectToAction(nameof(Index)); ;
            }
            else
            {
                return(NotFound());
            }
        }

        public ActionResult TaskDelete(int id)
        {

            var taskToDelete = _mapper.Map<TaskDto, TaskViewModel> (_service.Find(id));
            
            if (taskToDelete == null) return NotFound();

            var childrenTasks = _mapper.Map<List<TaskDto>, List<TaskViewModel>>( _service.AllChildrenOfTask(id));

            var TaskDelVM = new TaskDeleteViewModel
            {
                parentModel = taskToDelete,
                ChildModels = childrenTasks
            };

            return PartialView("_PartialDelete", TaskDelVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskDelete(int id, [FromBody] TaskViewModel task)
        { 
            try
            {
                if (ModelState.IsValid)
                {
                    if (id != task.ID) return NotFound();

                    await _service.Delete(id);
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> StartTask(int id)
        {
            await _service.ChangeStatus(id, _mapper.Map<Models.TaskStatus, Service.DTO.TaskStatus>(Models.TaskStatus.InProgress));
            return (ActionResult)TaskDetails(id);
        }

        public async Task<ActionResult> PauseTask(int id)
        {
            await _service.ChangeStatus(id, _mapper.Map<Models.TaskStatus, Service.DTO.TaskStatus>(Models.TaskStatus.Paused));
            return (ActionResult)TaskDetails(id);
        }

        public async Task<ActionResult> EndTask(int id)
        {
            await _service.ChangeStatus(id, _mapper.Map<Models.TaskStatus, Service.DTO.TaskStatus>(Models.TaskStatus.Complete));
            return (ActionResult)TaskDetails(id);
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}