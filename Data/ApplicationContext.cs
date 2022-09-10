using Microsoft.EntityFrameworkCore;
using VecoBackend.Models;

namespace VecoBackend.Data;

public class ApplicationContext : DbContext
{
    private IConfiguration _configuration;

    public ApplicationContext(DbContextOptions<ApplicationContext> options, IConfiguration configuration) :
        base(options)
    {
        _configuration = configuration;
    }

    public DbSet<TaskModel> TaskModels { get; set; }
    public DbSet<UserModel> UserModels { get; set; }
    public DbSet<UserTaskModel> UserTaskModels { get; set; }
    public DbSet<TaskImageModel> TaskImageModels { get; set; }
    public DbSet<ImageStorageModel> ImageStorageModels { get; set; }
    public DbSet<NotificationTokensModel> NotificationTokensModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().HasIndex(u => u.email).IsUnique();
        modelBuilder.Entity<UserModel>().HasIndex(u => u.token).IsUnique();
        modelBuilder.Entity<UserTaskModel>().HasOne(x => x.user).WithMany(x => x.userTasks)
            .HasForeignKey(x => x.UserId);
        modelBuilder.Entity<UserTaskModel>().HasOne(x => x.task).WithMany(x => x.userTasks)
            .HasForeignKey(x => x.TaskId);
        modelBuilder.Entity<NotificationTokensModel>().HasOne(x => x.user).WithMany(x => x.notificationTokens)
            .HasForeignKey(x => x.UserId);
        modelBuilder.Entity<TaskImageModel>().HasOne(x => x.UserTask).WithMany(x => x.task_images)
            .HasForeignKey(x => x.UserTaskId);
        modelBuilder.Entity<TaskImageModel>().HasOne(x => x.ImageStorage).WithMany(x => x.TaskImageModel)
            .HasForeignKey(x => x.imageId);
        modelBuilder.Entity<ImageStorageModel>().HasOne(x => x.UserModel).WithMany(x => x.images)
            .HasForeignKey(x => x.userId);
        modelBuilder.Entity<UserTaskModel>().Property(t => t.DeleteTime).HasColumnType("datetime2");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("MainDB");
        optionsBuilder.UseNpgsql(connectionString);
    }
}