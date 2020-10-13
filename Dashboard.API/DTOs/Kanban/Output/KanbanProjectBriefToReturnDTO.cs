namespace Dashboard.API.DTOs
{
    public class KanbanProjectBriefToReturnDTO
    {
        public string Name;
        public string Notes;
        public int ActiveStories;
        public int OpenTasks;
        public int ClosedTasks;
    }
}