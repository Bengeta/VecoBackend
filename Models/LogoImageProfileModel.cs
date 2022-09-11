using VecoBackend.Enums;
using VecoBackend.Interfaces;

namespace VecoBackend.Models;

public class LogoImageProfileModel : IImageProfile
{
    private const int mb = 1048576;

    public LogoImageProfileModel()
    {
        AllowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };
    }

    public SaveImageType ImageType => SaveImageType.Logo;

    public string Folder => "logos";

    public int Width => 300;

    public int Height => 300;

    public int MaxSizeBytes => 5 * mb;

    public IEnumerable<string> AllowedExtensions { get; }
}