using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using VecoBackend.Interfaces;

namespace VecoBackend.Models;
[Keyless]
public class BoxImageProfileModel : IImageProfile
{
    private int _id;
    private const int mb = 1048576;

    public BoxImageProfileModel()
    {
        AllowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };
    }

    public ImageType ImageType => ImageType.Box;

    public string Folder => "boxes";

    public int Width => 500;

    public int Height => 500;

    public int MaxSizeBytes => 10 * mb;

    public IEnumerable<string> AllowedExtensions { get; }
}