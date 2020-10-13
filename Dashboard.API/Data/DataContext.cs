using Dashboard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.API.Data
{
    /// <summary>
    /// DbContext represents a session with the database.
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Calls base class (DbContext) constuctor
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // Tell the Entity Framework about our entities. The property name will become the table name.
        public DbSet<User> Users { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SettingField> SettingFields { get; set; }
        public DbSet<City> Cities {get; set;}
        public DbSet<KanbanProject> KanbanProjects {get; set;}
        public DbSet<KanbanStory> KanbanStories {get; set;}
        public DbSet<KanbanTask> KanbanTasks {get; set;}
    }
}