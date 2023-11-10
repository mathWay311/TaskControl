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
        public ActionResult Create([Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EndDate,ParentID")] TaskModel task)
        {
            try
            {
                _context.Add(task);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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
        public ActionResult Edit(int id, [Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EndDate,ParentID")] TaskModel task)
        {
            if (id != task.ID) return NotFound();

            try
            {
                _context.Update(task);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch 
            {
                return RedirectToAction(nameof(Index));
            }
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
        public ActionResult Delete(int id, [Bind("ID,TaskName,Description,TaskExecutors,RegistrationDate,TaskStatus,EndDate,ParentID")] TaskModel task)
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



        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
