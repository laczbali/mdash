using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Dashboard.API.Data;
using Dashboard.API.DTOs;
using Dashboard.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KanbanController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepo;
        private readonly IKanbanRepository _kanbanRepo;
        private readonly IMapper _mapper;
        public KanbanController(ISettingsRepository settingsRepo, IKanbanRepository kanbanRepo, IMapper mapper)
        {
            _mapper = mapper;
            _kanbanRepo = kanbanRepo;
            _settingsRepo = settingsRepo;
        }

        [HttpGet]
        [Route("list-tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            try
            {
                var tasks = await _kanbanRepo.GetAllTasks(clientUsername);
                return Ok(tasks);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("list-projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            try
            {
                var projects = await _kanbanRepo.GetAllProjects(clientUsername);
                return Ok(projects);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("get-project/{projectId}")]
        public async Task<IActionResult> GetProject(int projectId)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            try
            {
                var project = await _kanbanRepo.GetProject(clientUsername, projectId);
                return Ok(project);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("add-task")]
        public async Task<IActionResult> AddTask(KanbanTaskNewDTO taskToAdd)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            var newTask = new KanbanTask();
            newTask.Name = taskToAdd.Name;
            newTask.Status = taskToAdd.Status;
            newTask.Notes = taskToAdd.Notes;
            newTask.DueDate = taskToAdd.DueDate;
            newTask.Priority = taskToAdd.Priority;

            try
            {
                await _kanbanRepo.AddTask(clientUsername, taskToAdd.ProjectName, taskToAdd.StoryName, newTask);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpPost]
        [Route("add-story")]
        public async Task<IActionResult> AddStory(KanbanStoryNewDTO storyToAdd)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            var newStory = new KanbanStory();
            newStory.Name = storyToAdd.Name;
            newStory.Notes = storyToAdd.Notes;

            try
            {
                await _kanbanRepo.AddStory(clientUsername, storyToAdd.ProjectName, newStory);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpPost]
        [Route("add-project")]
        public async Task<IActionResult> AddProject(KanbanProjectNewDTO projectToAdd)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            var newProject = new KanbanProject();
            newProject.Name = projectToAdd.Name;
            newProject.Notes = projectToAdd.Notes;

            try
            {
                await _kanbanRepo.AddProject(clientUsername, newProject);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpPost]
        [Route("modify-project/{projectId}")]
        public async Task<IActionResult> ModifyProject(int projectId, KanbanProjectNewDTO newProjectData)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            try
            {
                await _kanbanRepo.ModifyProject(clientUsername, projectId, newProjectData);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();

        }

        [HttpPost]
        [Route("modify-story/{storyId}")]
        public async Task<IActionResult> ModifyStory(int storyId, KanbanStoryNewDTO newStoryData)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            try
            {
                await _kanbanRepo.ModifyStory(clientUsername, storyId, newStoryData);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpPost]
        [Route("modify-task/{taskId}")]
        public async Task<IActionResult> ModifyTask(int taskId, KanbanTaskNewDTO newTaskData)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            try
            {
                await _kanbanRepo.ModifyTask(clientUsername, taskId, newTaskData);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("delete-project/{projectId}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            try
            {
                await _kanbanRepo.DeleteProject(clientUsername, projectId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("delete-story/{storyId}")]
        public async Task<IActionResult> DeleteStory(int storyId)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            try
            {
                await _kanbanRepo.DeleteStory(clientUsername, storyId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("delete-task/{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            try
            {
                await _kanbanRepo.DeleteTask(clientUsername, taskId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}