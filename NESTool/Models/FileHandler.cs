namespace NESTool.Models;

public class FileHandler
{
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AFileModel? FileModel { get; set; } = null;

    public void Save() => FileModel?.Save(Path, Name);
}
