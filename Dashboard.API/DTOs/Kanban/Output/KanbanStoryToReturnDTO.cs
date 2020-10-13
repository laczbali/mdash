using System.Collections.Generic;

namespace Dashboard.API.DTOs
{
    public class KanbanStoryToReturnDTO
    {
        public int Id;
        public string Name {get; set;}
        public string Notes {get; set;}
        public ICollection<KanbanTaskToReturnDTO> Tasks {get; set;}
    }
}