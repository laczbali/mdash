using System;
using Dashboard.API.Models;

namespace Dashboard.API.DTOs
{
    public class KanbanTaskToReturnDTO
    {
        public int Id;
        public string Name;
        public string StoryName;
        public string ProjectName;
        public string Notes;
        public KanbanStatus Status;
        public KanbanPriority Priority;
        public DateTime CreationDate;
        public DateTime DueDate;   
    }
}