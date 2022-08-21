namespace VecoBackend.Interfaces;

public interface IMigratorService
{
    public string MigrateUp();
    public string MigrateDown(long version);
}