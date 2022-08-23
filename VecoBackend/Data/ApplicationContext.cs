using Microsoft.EntityFrameworkCore;
using VecoBackend.Models;

namespace VecoBackend.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {}
      DbSet<TaskModel> TaskModels{ get; set; }
      DbSet<UserModel> UserModels{ get; set; }
      DbSet<UserTaskModel> UserTaskModels{ get; set; }
      
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserTaskModel>()
            .HasKey(t => new { t.user_id, t.task_id });
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("MainDB");
        optionsBuilder.UseNpgsql(connectionString);
    }

}