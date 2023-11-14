using System.Collections.Generic;

namespace TaskControl.Models
{
    public class JsTreeModel
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string state { get; set; }
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
        public string li_attr { get; set; }
        public string a_attr { get; set; }
        public string type { get; set; }


    }
    public static class JsTreeUtils 
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
                    type = statusToIconType[item.taskStatus]
                });
            }
            return nodes;
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
    }
    
}
