using System.Reflection;

namespace VecoBackend.Utils;

public class AppSettings
{
    public static AppSettings Settings { get; set; }
    public string UseDatabase { get; set; }
    public string MigrationAssembly { get; set; }
    public Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();

    public AppSettings()
    {
        Settings = this;
    }
}