using Nett;
using System.IO;

namespace NESTool.Models
{
    public abstract class AFileModel
    {
        public abstract string FileExtension { get; }

        protected string _fileExtension = "";

        public void Save(string path, string name)
        {
            Toml.WriteFile(this, Path.Combine(path, name + FileExtension));
        }
    }
}
