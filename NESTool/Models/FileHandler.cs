namespace NESTool.Models;

public class FileHandler
{
    public string Path { get; set; }
    public string Name { get; set; }
    public AFileModel FileModel { get; set; }

    public void Save() => FileModel.Save(Path, Name);
}
