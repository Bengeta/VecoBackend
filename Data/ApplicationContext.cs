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
    public DbSet<TaskPhotoModel> TaskPhotoModels { get; set; }
    public DbSet<NotificationTokensModel> NotificationTokensModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().HasIndex(u => u.email).IsUnique();
        modelBuilder.Entity<UserModel>().HasIndex(u => u.token).IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("MainDB");
        optionsBuilder.UseNpgsql(connectionString);
    }
}