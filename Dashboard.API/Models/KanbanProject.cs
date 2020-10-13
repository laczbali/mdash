using System.Collections.Generic;

namespace Dashboard.API.Models
{
    public class KanbanProject
    {
        public int Id { get; set; }
        public User ParentUser {get; set;}
        public string Name {get; set;}
        public string Notes {get; set;}
        public ICollection<KanbanStory> Stories {get; set;}
    }
}