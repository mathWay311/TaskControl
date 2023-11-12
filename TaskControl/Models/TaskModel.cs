using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore;
using System.ComponentModel.DataAnnotations;

namespace TaskControl.Models
{
    public enum TaskStatus { 
        [Display(Name="Назначена")]
        Assigned,

        [Display(Name = "Выполняется")]
        InProgress,

        [Display(Name = "Приостановлена")]
        Paused,

        [Display(Name = "Завершена")]
        Complete
    };
    
    public class TaskModel
    {
        public int ID { get; set; }
        [Required]
        [StringLength(60, ErrorMessage = "Название задачи должно быть длиной от {2} до {1} символов.", MinimumLength = 1)]
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string TaskExecutors { get; set; }
        public DateTime RegistrationDate { get; set; }
        public TaskStatus taskStatus { get; set; }
        public DateTime EndDate { get; set; }
        public int? ParentID { get; set; }
        [TaskModelUtils.DateTimeValidation(ErrorMessage = "LOL")]
        public DateTime EstimatedEndDate { get; set; }
    }

    public static class TaskModelUtils
    {
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
        public class DateTimeValidation : ValidationAttribute
        {

            public string GetErrorMessage() =>
                $"Дата должна быть не раньше текущей";

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var date = ((DateTime)value!).Date;

                if (date < DateTime.Now)
                {
                    return new ValidationResult("Something went wrong"); ;
                }

                return ValidationResult.Success;
            }



        }

        public static List<TaskModel> AllChildrenOfTask(List<TaskModel> tasks, TaskModel parentTask)
        {
            List<TaskModel> ChildrenTasks = new List<TaskModel>();
            Queue<TaskModel> toVisit = new Queue<TaskModel>();

            toVisit.Enqueue(parentTask);
            TaskModel currentNode = toVisit.Peek();

            while (toVisit.Count != 0)
            {
                currentNode = toVisit.Peek();
                foreach (TaskModel task in tasks)
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

        public static Dictionary<string, TimeSpan> SubTaskTime (List<TaskModel> childTasks, TaskModel parentTask)
        {
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

            if (parentTask.taskStatus == Models.TaskStatus.Complete)
            {
                TimeSpan totalAdditionalElapsedTime = TimeSpan.FromSeconds(0);
                foreach (var childTask in childTasks)
                {
                    totalAdditionalElapsedTime += childTask.RegistrationDate - childTask.EndDate;
                }
                times["Elapsed"] += totalAdditionalElapsedTime;
            }

            return times;
        }


        public static Dictionary<TaskStatus, string> statusToIconType = new Dictionary<TaskStatus, string>
        {
            {
                TaskStatus.Assigned, "assigned"
            },
            {
                TaskStatus.InProgress, "inprogress"
            },
            {
                TaskStatus.Paused, "paused"
            },
            {
                TaskStatus.Complete, "complete"
            },
        };

        public static Dictionary<TaskStatus, List<NavigationLink>> navLinks = new Dictionary<TaskStatus, List<NavigationLink>>
        {
            {
                TaskStatus.Assigned, new List<NavigationLink>
                {
                    new NavigationLink("Редактировать", "Edit"),
                    new NavigationLink("Детали", "Details"),
                    new NavigationLink("Удалить", "Delete"),
                    new NavigationLink("Начать", "StartTask"),
                    new NavigationLink("Создать подзадачу", "CreateSubTask")
                }
            },
            {
                TaskStatus.InProgress, new List<NavigationLink>
                {
                    new NavigationLink("Редактировать", "Edit"),
                    new NavigationLink("Детали", "Details"),
                    new NavigationLink("Удалить", "Delete"),
                    new NavigationLink("Приостановить", "PauseTask"),
                    new NavigationLink("Завершить", "EndTask"),
                    new NavigationLink("Создать подзадачу", "CreateSubTask")
                }
            },
            {
                TaskStatus.Paused, new List<NavigationLink>
                {
                    new NavigationLink("Редактировать", "Edit"),
                    new NavigationLink("Детали", "Details"),
                    new NavigationLink("Удалить", "Delete"),
                    new NavigationLink("Возобновить", "StartTask"),
                    new NavigationLink("Создать подзадачу", "CreateSubTask")
                }
            },
            {
                TaskStatus.Complete, new List<NavigationLink>
                {
                    new NavigationLink("Детали", "Details"),
                    new NavigationLink("Удалить", "Delete"),
                }
            }
        };
    }


    public class NavigationLink
    {
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public NavigationLink(string linkText, string actionName)
        {
            LinkText = linkText;
            ActionName = actionName;
        }
    }

    public class TaskDBContext : DbContext
    {
        public DbSet<TaskModel> Task { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlite(connectionString);
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
            
        }

    

    }


}
