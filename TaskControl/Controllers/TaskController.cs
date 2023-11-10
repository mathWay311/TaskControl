using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TaskControl.Models;


namespace TaskControl.Controllers
{
    
    public class TaskController : Controller
    {
        private TaskDBContext _context = new TaskDBContext();
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

            var task = _context.Task.Find(id);

            if (task == null) return NotFound();

            return View(task);
        }

        // POST: TaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, [Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EstimatedEndDate,ParentID")] TaskModel task)
        {
            
            try
            {
                _context.Task.Remove(task);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
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

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
