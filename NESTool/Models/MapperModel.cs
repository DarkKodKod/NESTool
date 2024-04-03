using ArchitectureLibrary.Model;
using NESTool.Enums;

namespace NESTool.Models;

public class MapperModel : ISingletonModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int[] PRG { get; set; }
    public int[] CHR { get; set; }
    public MirroringType[] Mirroring { get; set; }
}
