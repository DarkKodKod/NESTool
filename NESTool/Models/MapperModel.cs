using ArchitectureLibrary.Model;

namespace NESTool.Models
{
    public class MapperModel : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] PRG { get; set; }
        public int[] CHR { get; set; }
    }
}
