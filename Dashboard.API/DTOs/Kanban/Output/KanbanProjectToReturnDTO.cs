using System.Collections.Generic;

namespace Dashboard.API.DTOs
{
    public class KanbanProjectToReturnDTO
    {
        public int Id;
        public string Name;
        public string Notes;
        public ICollection<KanbanStoryToReturnDTO> Stories {get; set;}
    }
}