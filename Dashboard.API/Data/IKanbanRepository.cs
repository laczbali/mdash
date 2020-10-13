using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.API.DTOs;
using Dashboard.API.Models;

namespace Dashboard.API.Data
{
    public interface IKanbanRepository
    {
        /// <summary>
        /// Create a new Project for a User
        /// </summary>
        /// <param name="username"></param>
        /// <param name="newProject">Define (unique for User) NAME, and the NOTES (if any)</param>
        /// <returns></returns>
        Task AddProject(string username, KanbanProject newProject);

        /// <summary>
        /// Create a new Story for a Project
        /// </summary>
        /// <param name="username"></param>
        /// <param name="projectName">Name of the parent Project (to add the Story to)</param>
        /// <param name="newStory">Define (unique for Project) NAME, and the NOTES (if any)</param>
        /// <returns></returns>
        Task AddStory(string username, string projectName, KanbanStory newStory);

        /// <summary>
        /// Create a new Task for a Story
        /// </summary>
        /// <param name="username"></param>
        /// <param name="projectName">Name of the parent Project (to add the Task to)</param>
        /// <param name="storyName">Name of the parent Story (to add the Task to)</param>
        /// <param name="newTask">Define (unique for Story) NAME, the NOTES (if any), the DUE_DATE and the PRIORITY</param>
        /// <returns></returns>
        Task AddTask(string username, string projectName, string storyName, KanbanTask newTask);

        /// <summary>
        /// Get all Tasks for a User
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<List<KanbanTaskToReturnDTO>> GetAllTasks(string username);

        /// <summary>
        /// Get all projects for a user, on a high level
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<List<KanbanProjectBriefToReturnDTO>> GetAllProjects(string username);

        /// <summary>
        /// Get all details and sub-components for a project
        /// </summary>
        /// <param name="username"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        Task<KanbanProjectToReturnDTO> GetProject(string username, int projectId);

        /// <summary>
        /// Modify the project specified by the ID with the new data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="projectId"></param>
        /// <param name="newData"></param>
        /// <returns></returns>
        Task ModifyProject(string username, int projectId, KanbanProjectNewDTO newData);

        /// <summary>
        /// Modify the story specified by the ID with the new data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="storyId"></param>
        /// <param name="newData"></param>
        /// <returns></returns>
        Task ModifyStory(string username, int storyId, KanbanStoryNewDTO newData);

        /// <summary>
        /// Modify the task specified by the ID with the new data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="storyId"></param>
        /// <param name="newData"></param>
        /// <returns></returns>
        Task ModifyTask(string username, int taskId, KanbanTaskNewDTO newData);

        /// <summary>
        /// Delete the project specified by the id
        /// </summary>
        /// <param name="username"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task DeleteProject(string username, int projectId);

        /// <summary>
        /// Delete the story specified by the id
        /// </summary>
        /// <param name="username"></param>
        /// <param name="storyId"></param>
        /// <returns></returns>
        Task DeleteStory(string username, int storyId);

        /// <summary>
        /// Delete the task specified by the id
        /// </summary>
        /// <param name="username"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task DeleteTask(string username, int taskId);
    }
}