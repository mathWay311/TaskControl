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

namespace TaskControl.Models
{
    public enum TaskStatus { Assigned, InProgress, Paused, Complete};
    
    public class TaskModel
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string TaskExecutors { get; set; }
        public DateTime RegistrationDate { get; set; }
        public TaskStatus taskStatus { get; set; }
        public DateTime EndDate { get; set; }
        public int? ParentID { get; set; }
        public DateTime EstimatedEndDate { get; set; }
    }

    public static class TaskModelUtils
    {
        public static Dictionary<TaskStatus, List<NavigationLink>> navLinks = new Dictionary<TaskStatus, List<NavigationLink>>
        {
            {
                TaskStatus.Assigned, new List<NavigationLink>
                {
                    new NavigationLink("Редактировать", "Edit"),
                    new NavigationLink("Детали", "Details"),
                    new NavigationLink("Удалить", "Delete"),
                    new NavigationLink("Начать", "StartTask"),
                    new NavigationLink("Завершить", "EndTask")
                }
            },
            {
                TaskStatus.InProgress, new List<NavigationLink>
                {
                    new NavigationLink("Редактировать", "Edit"),
                    new NavigationLink("Детали", "Details"),
                    new NavigationLink("Удалить", "Delete"),
                    new NavigationLink("Приостановить", "PauseTask"),
                    new NavigationLink("Завершить", "EndTask")
                }
            },
            {
                TaskStatus.Paused, new List<NavigationLink>
                {
                    new NavigationLink("Редактировать", "Edit"),
                    new NavigationLink("Детали", "Details"),
                    new NavigationLink("Удалить", "Delete"),
                    new NavigationLink("Возобновить", "StartTask"),
                    new NavigationLink("Завершить", "EndTask")
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
            }
            
        }

    

    }


}
