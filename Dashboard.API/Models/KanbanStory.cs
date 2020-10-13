using System.Collections.Generic;

namespace Dashboard.API.Models
{
    public class KanbanStory
    {
        public int Id { get; set; }
        public KanbanProject ParentProject {get; set;}
        public string Name {get; set;}
        public string Notes {get; set;}
        public ICollection<KanbanTask> Tasks {get; set;}
    }
}