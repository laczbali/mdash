using System;
using Dashboard.API.Models;

namespace Dashboard.API.DTOs
{
    public class KanbanTaskNewDTO
    {
        public string Name;
        public string StoryName;
        public string ProjectName;
        public string Notes;
        public KanbanPriority Priority;
        public KanbanStatus Status;
        public DateTime DueDate;
        
    }
}