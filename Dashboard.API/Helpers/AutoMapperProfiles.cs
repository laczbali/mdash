using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dashboard.API.Data;
using Dashboard.API.DTOs;
using Dashboard.API.Models;

namespace Dashboard.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<KanbanTask, KanbanTaskToReturnDTO>()
                .ForMember(
                    destination => destination.StoryName,
                    options => options.MapFrom(
                        src => src.ParentStory.Name
                    )
                )
                .ForMember(
                    destination => destination.ProjectName,
                    options => options.MapFrom(
                        src => src.ParentStory.ParentProject.Name
                    )
                );

            CreateMap<KanbanProject, KanbanProjectBriefToReturnDTO>()
                .ForMember(
                    destination => destination.ActiveStories,
                    options => options.MapFrom(
                        // Count stories that have open tasks in them
                        src => src.Stories.Where( story =>
                            story.Tasks.Where(task =>
                                task.Status == KanbanStatus.New || task.Status == KanbanStatus.InProgress
                            ).Count() > 0
                        ).Count()
                    )
                )
                .ForMember(
                    destination => destination.OpenTasks,
                    options => options.MapFrom(
                        // Get number of open tasks
                        src => src.Stories.SelectMany(story => story.Tasks)
                            .Where(task => task.Status == KanbanStatus.New || task.Status == KanbanStatus.InProgress)
                                .Count()
                    )
                )
                .ForMember(
                    destination => destination.ClosedTasks,
                    options => options.MapFrom(
                        // Get number of closed tasks
                        src => src.Stories.SelectMany(story => story.Tasks)
                            .Where(task => task.Status == KanbanStatus.Done)
                                .Count()
                    )
                );
        }
    }
}