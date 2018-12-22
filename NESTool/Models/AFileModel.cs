using Nett;

namespace NESTool.Models
{
    public abstract class AFileModel
    {
        [TomlIgnore] public abstract string FileExtension { get; }

        protected string _fileExtension = "";
    }
}
