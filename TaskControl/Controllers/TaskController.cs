using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TaskControl.Models;


namespace TaskControl.Controllers
{
    
    public class TaskController : Controller
    {

        private readonly TaskDBContext _context = new TaskDBContext();

        // GET: TaskController
        public ActionResult Index()
        {
            _context.Database.EnsureCreated();
            var tasks = from task in _context.Task select task;

            var TaskIndexVM = new TaskViewModel
            {
                Tasks = new List<TaskModel>(tasks.ToList())
            };
            return View(TaskIndexVM);
        }
        
        // GET: TaskController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            
            TaskModel task = _context.Task.Find(id);

            if (task == null) return NotFound();

            return View(task);
        }

        // GET: TaskController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EstimatedEndDate,ParentID")] TaskModel task)
        {
            
            if (ModelState.IsValid)
            {
                task.ParentID = null;
                task.RegistrationDate = DateTime.Now;
                _context.Task.Add(task);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
            
        }

        // GET: TaskController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var task = _context.Task.Find(id);

            if (task == null) return NotFound();
         
            return View(task);
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,taskStatus,EstimatedEndDate,ParentID")] TaskModel task)
        {
            if (id != task.ID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TaskController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var taskToDelete = _context.Task.Find(id);

            if (taskToDelete == null) return NotFound();

           
            var allTasks = (from task in _context.Task select task).ToList<TaskModel>();

            List<TaskModel> childrenTaskModels = TaskModelUtils.AllChildrenOfTask(allTasks, taskToDelete);

            var TaskDelVM = new TaskDeleteViewModel
            {
                parentModel = taskToDelete,
                ChildModels = new List<TaskModel>(childrenTaskModels)
            };

            return View(TaskDelVM);
        }

        // POST: TaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, [Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EstimatedEndDate,ParentID")] TaskModel task)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id != task.ID) return NotFound();

                    var allTasks = (from ts in _context.Task select ts).ToList<TaskModel>();
                    List<TaskModel> childrenTaskModels = TaskModelUtils.AllChildrenOfTask(allTasks, task);

                    _context.Task.Remove(task);
                    foreach (var childTask in childrenTaskModels)
                    {
                        _context.Task.Remove(childTask);
                    }
                    await _context.SaveChangesAsync();
                   
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }


        public ActionResult StartTask(int? id)
        {
            if (id == null) return NotFound();

            var task = _context.Task.Find(id);

            if (task == null) return NotFound();
            
            if (task.taskStatus != Models.TaskStatus.Complete)
                task.taskStatus = Models.TaskStatus.InProgress;

            _context.Update(task);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public ActionResult PauseTask(int? id)
        {
            if (id == null) return NotFound();

            var task = _context.Task.Find(id);

            if (task == null) return NotFound();

            if (task.taskStatus == Models.TaskStatus.InProgress)
                task.taskStatus = Models.TaskStatus.Paused;

            _context.Update(task);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public ActionResult EndTask(int? id)
        {
            if (id == null) return NotFound();

            var task = _context.Task.Find(id);

            if (task == null) return NotFound();

            task.taskStatus = Models.TaskStatus.Complete;
            task.EndDate = DateTime.Now;

            _context.Update(task);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public ActionResult CreateSubTask(int? id)
        {

            if (id == null) return NotFound();

            TaskModel parentModel = _context.Task.Find(id);

            if (parentModel == null) return NotFound();

            ViewData["ParentModel"] = parentModel;
            return View();
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSubTask([Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EstimatedEndDate,ParentID")] TaskModel task)
        {
            if (ModelState.IsValid)
            {
                task.ID = 0;
                task.RegistrationDate = DateTime.Now;
                _context.Task.Add(task);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(task);

        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
