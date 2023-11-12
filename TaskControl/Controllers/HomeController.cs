using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TaskControl.Models;
namespace TaskControl.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            var _context = new TaskController().GetDbContext();
            _context.Database.EnsureCreated();
            var tasks = from task in _context.Task select task;

            var TaskIndexVM = new TaskViewModel
            {
                Tasks = new List<TaskModel>(tasks.ToList())
            };

            IList<JsTreeModel> nodes = new List<JsTreeModel>();
            foreach (var item in tasks)
            {
                nodes.Add(new JsTreeModel
                {
                    id = item.ID.ToString(),
                    parent = item.ParentID == null ? "#" : item.ParentID.ToString(),
                    text = item.TaskName,
                    opened = true,
                    type = TaskModelUtils.statusToIconType[item.taskStatus]
                });
            }
            ViewBag.Json = JsonSerializer.Serialize(nodes);

            return View(TaskIndexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CreateTask()
        {
            var _taskController = new TaskController();
            return _taskController.Create();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask([Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EstimatedEndDate,ParentID")] TaskModel task)
        {
            var _taskController = new TaskController(); 
            return _taskController.Create(task);
        }

        public IActionResult TaskDetails(int? id)
        {
            var _taskController = new TaskController();
            var _context = _taskController.GetDbContext();

            if (id == null) return NotFound();

            TaskModel task = _context.Task.Find(id);

            if (task == null) return NotFound();
            

            var allTasks = (from ts in _context.Task where id != ts.ID select ts).ToList<TaskModel>();
            var childrenTasks = TaskModelUtils.AllChildrenOfTask(allTasks, task);

            Dictionary<string, TimeSpan> time = TaskModelUtils.SubTaskTime(childrenTasks, task);
            
            TaskDetailViewModel taskVM = new TaskDetailViewModel
            {
                task = task,
                AddEstimatedTime = time["Estimated"],
                AddElapsedTime = time["Elapsed"]
            };
            
            return PartialView("_PartialDetails", taskVM);
        }

        public ActionResult TaskDelete(int? id)
        {
            var _taskController = new TaskController();
            return _taskController.Delete(id);
        }

        // POST: TaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskDelete(int id, [Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EstimatedEndDate,ParentID")] TaskModel task)
        {
            var _taskController = new TaskController();

            return await _taskController.Delete(id, task);
        }

        public ActionResult CreateSubTask(int? id)
        {
            var _taskController = new TaskController();
            return _taskController.CreateSubTask(id);
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSubTask([Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EstimatedEndDate,ParentID")] TaskModel task)
        {
            var _taskController = new TaskController();
            return _taskController.CreateSubTask(task);
        }

        public async Task<ActionResult> StartTask(int? id)
        {
            var _taskController = new TaskController();
            return await _taskController.StartTask(id);
        }

        public async Task<ActionResult> PauseTask(int? id)
        {
            var _taskController = new TaskController();
            return await _taskController.PauseTask(id);
        }

        public async Task<ActionResult> EndTask(int? id)
        {
            var _taskController = new TaskController();
            return await _taskController.EndTask(id);
        }

        public async Task<ActionResult> TaskEdit(int? id)
        {
            var _taskController = new TaskController();
            return await _taskController.Edit(id);
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskEdit(int id, [Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,taskStatus,EstimatedEndDate,ParentID")] TaskModel task)
        {
            var _taskController = new TaskController();
            return await _taskController.Edit(id, task);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}