using System;

namespace Dashboard.API.Models
{
    public class KanbanTask
    {
        public int Id { get; set; }
        public KanbanStory ParentStory {get; set;}
        public string Name {get; set;}
        public KanbanStatus Status {get; set;}
        public string Notes {get; set;}
        public KanbanPriority Priority {get; set;}
        public DateTime CreationDate {get; set;}
        public DateTime DueDate {get; set;}
    }

    public enum KanbanStatus {
        New = 0,
        InProgress = 1,
        Done = 2
    }

    public enum KanbanPriority {
        Low = -1,
        Normal = 0,
        High = 1
    }
}