using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Dashboard.API.DTOs;
using Dashboard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.API.Data
{
    public class KanbanRepository : IKanbanRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public KanbanRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddProject(string username, KanbanProject newProject)
        {
            if (string.IsNullOrWhiteSpace(newProject.Name))
            {
                throw new Exception("Project name cannot be empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (await _context.KanbanProjects.FirstOrDefaultAsync(x => x.Name == newProject.Name && x.ParentUser == user) != null)
            {
                throw new Exception("Project name must be unique");
            }

            newProject.ParentUser = user;

            await _context.KanbanProjects.AddAsync(newProject);
            _context.SaveChanges();
        }

        public async Task AddStory(string username, string projectName, KanbanStory newStory)
        {
            if (string.IsNullOrWhiteSpace(newStory.Name))
            {
                throw new Exception("Story name cannot be empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var project = await _context.KanbanProjects.FirstOrDefaultAsync(x => x.Name == projectName && x.ParentUser == user);
            if (project == null)
            {
                throw new Exception("Invalid project: [" + projectName + "]");
            }

            if (await _context.KanbanStories.FirstOrDefaultAsync(x => x.Name == newStory.Name && x.ParentProject == project) != null)
            {
                throw new Exception("Story name must be unique within project.");
            }

            newStory.ParentProject = project;

            await _context.KanbanStories.AddAsync(newStory);
            _context.SaveChanges();
        }

        public async Task AddTask(string username, string projectName, string storyName, KanbanTask newTask)
        {
            if (string.IsNullOrWhiteSpace(newTask.Name))
            {
                throw new Exception("Task name cannot be empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var project = await _context.KanbanProjects.FirstOrDefaultAsync(x => x.Name == projectName && x.ParentUser == user);
            if (project == null)
            {
                throw new Exception("Invalid project: [" + projectName + "]");
            }
            var story = await _context.KanbanStories.FirstOrDefaultAsync(x => x.Name == storyName && x.ParentProject == project);
            if (story == null)
            {
                throw new Exception("Invalid story: [" + storyName + "]");
            }

            if (await _context.KanbanTasks.FirstOrDefaultAsync(x => x.Name == newTask.Name && x.ParentStory == story) != null)
            {
                throw new Exception("Task name must be unique within story");
            }

            newTask.CreationDate = DateTime.Now;
            newTask.ParentStory = story;
            await _context.KanbanTasks.AddAsync(newTask);
            _context.SaveChanges();
        }

        public async Task DeleteProject(string username, int projectId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var project = await _context.KanbanProjects
                .Include(x => x.Stories)
                    .ThenInclude(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.Id == projectId && x.ParentUser == user);

            if(project == null)
            {
                throw new Exception("No project with ID [" + projectId.ToString() + "] for the user.");
            }

            foreach (var story in project.Stories)
            {
                foreach (var task in story.Tasks)
                {
                    _context.KanbanTasks.Remove(task);
                }
                _context.KanbanStories.Remove(story);
            }
            _context.KanbanProjects.Remove(project);

            _context.SaveChanges();
        }

        public async Task DeleteStory(string username, int storyId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var story = await _context.KanbanStories
                .Include(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.Id == storyId && x.ParentProject.ParentUser == user);

            if(story == null)
            {
                throw new Exception("No story with ID [" + storyId.ToString() + "] for the user.");
            }

            foreach (var task in story.Tasks)
            {
                _context.KanbanTasks.Remove(task);
            }
            _context.KanbanStories.Remove(story);

            _context.SaveChanges();
        }

        public async Task DeleteTask(string username, int taskId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var task = await _context.KanbanTasks
                .FirstOrDefaultAsync(x => x.Id == taskId && x.ParentStory.ParentProject.ParentUser == user);

            if (task == null)
            {
                throw new Exception("No task with ID [" + taskId.ToString() + "] for the user.");
            }

            _context.KanbanTasks.Remove(task);
            _context.SaveChanges();
        }

        public async Task<List<KanbanProjectBriefToReturnDTO>> GetAllProjects(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var projects = await _context.KanbanProjects
                .Include(x => x.Stories)
                    .ThenInclude(x => x.Tasks)
                .Where(x => x.ParentUser == user).ToListAsync();

            var projectsToReturn = _mapper.Map<List<KanbanProjectBriefToReturnDTO>>(projects);
            return projectsToReturn;
        }

        public async Task<List<KanbanTaskToReturnDTO>> GetAllTasks(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var tasks = await _context.KanbanTasks
                .Include(x => x.ParentStory)
                    .ThenInclude(x => x.ParentProject)
                .Where(x => x.ParentStory.ParentProject.ParentUser == user).ToListAsync();

            var tasksToReturn = _mapper.Map<List<KanbanTaskToReturnDTO>>(tasks);
            return tasksToReturn;
        }

        public async Task<KanbanProjectToReturnDTO> GetProject(string username, int projectId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var project = await _context.KanbanProjects
                .Include(x => x.Stories)
                    .ThenInclude(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.Id == projectId && x.ParentUser == user);

            var projectToReturn = _mapper.Map<KanbanProjectToReturnDTO>(project);
            return projectToReturn;
        }

        public async Task ModifyProject(string username, int projectId, KanbanProjectNewDTO newData)
        {
            if (string.IsNullOrWhiteSpace(newData.Name))
            {
                throw new Exception("Project name cannot be empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var project = await _context.KanbanProjects
                .FirstOrDefaultAsync(x => x.Id == projectId && x.ParentUser == user);

            if (project == null)
            {
                throw new Exception("No project with ID [" + projectId.ToString() + "] for the user.");
            }

            if (newData.Name != project.Name)
            {
                if (await _context.KanbanProjects.FirstOrDefaultAsync(x => x.Name == newData.Name && x.ParentUser == user) != null)
                {
                    throw new Exception("Project name must be unique");
                }
            }

            project.Name = newData.Name;
            project.Notes = newData.Notes;
            _context.SaveChanges();
        }

        public async Task ModifyStory(string username, int storyId, KanbanStoryNewDTO newData)
        {
            if (string.IsNullOrWhiteSpace(newData.Name))
            {
                throw new Exception("Story name cannot be empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var story = await _context.KanbanStories
                .Include(x => x.ParentProject)
                .FirstOrDefaultAsync(x => x.Id == storyId && x.ParentProject.ParentUser == user);

            if (story == null)
            {
                throw new Exception("No story with ID [" + storyId.ToString() + "] for the user.");
            }

            var project = await _context.KanbanProjects.FirstOrDefaultAsync(x => x.Name == newData.ProjectName && x.ParentUser == user);
            if (project == null)
            {
                throw new Exception("No project with name [" + newData.Name + "] for the user.");
            }

            if (newData.Name != story.Name || newData.ProjectName != story.ParentProject.Name)
            {
                if (await _context.KanbanStories.FirstOrDefaultAsync(x => x.Name == newData.Name && x.ParentProject == project) != null)
                {
                    throw new Exception("Story name must be unique within project.");
                }
            }

            story.Name = newData.Name;
            story.Notes = newData.Notes;
            story.ParentProject = project;
            _context.SaveChanges();
        }

        public async Task ModifyTask(string username, int taskId, KanbanTaskNewDTO newData)
        {
            if (string.IsNullOrWhiteSpace(newData.Name))
            {
                throw new Exception("Task name cannot be empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            var task = await _context.KanbanTasks
                .Include(x => x.ParentStory)
                .FirstOrDefaultAsync(x => x.Id == taskId && x.ParentStory.ParentProject.ParentUser == user);
            if (task == null)
            {
                throw new Exception("No task with ID [" + taskId.ToString() + "] for the user.");
            }

            var project = await _context.KanbanProjects.FirstOrDefaultAsync(x => x.Name == newData.ProjectName && x.ParentUser == user);
            if (project == null)
            {
                throw new Exception("Invalid project: [" + newData.ProjectName + "]");
            }
            var story = await _context.KanbanStories.FirstOrDefaultAsync(x => x.Name == newData.StoryName && x.ParentProject == project);
            if (story == null)
            {
                throw new Exception("Invalid story: [" + newData.StoryName + "]");
            }

            if (newData.Name != task.Name || newData.StoryName != task.ParentStory.Name)
            {
                if (await _context.KanbanTasks.FirstOrDefaultAsync(x => x.Name == newData.Name && x.ParentStory == story) != null)
                {
                    throw new Exception("Task name must be unique within story");
                }
            }

            task.ParentStory = story;
            task.Name = newData.Name;
            task.Status = newData.Status;
            task.Notes = newData.Notes;
            task.Priority = newData.Priority;
            task.DueDate = newData.DueDate;

            _context.SaveChanges();
        }
    }
}