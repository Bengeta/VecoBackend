using VecoBackend.Enums;

namespace VecoBackend.Interfaces;

public interface IImageProfile
{
    SaveImageType ImageType { get; }

    string Folder { get; }

    int Width { get; }

    int Height { get; }

    int MaxSizeBytes { get; }

    IEnumerable<string> AllowedExtensions { get; }
}