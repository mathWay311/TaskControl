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
    public enum TaskStatus
    {
        [Display(Name = "Назначена")]
        Assigned,

        [Display(Name = "Выполняется")]
        InProgress,

        [Display(Name = "Приостановлена")]
        Paused,

        [Display(Name = "Завершена")]
        Complete
    };

    public static class TaskModelUtils
    {
        public static IList<JsTreeModel> GetTreeJson(List<TaskViewModel> tasks)
        {
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
            return nodes;
        }
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

   


}
