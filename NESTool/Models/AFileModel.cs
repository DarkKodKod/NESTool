using Nett;
using System;
using System.IO;

namespace NESTool.Models
{
    public abstract class AFileModel
    {
        public abstract string FileExtension { get; }

        public string GUID { get; set; }

        protected string _fileExtension = "";

        public AFileModel()
        {
            GUID = Guid.NewGuid().ToString();
        }

        public void Save(string path, string name)
        {
            try
            {
                Toml.WriteFile(this, Path.Combine(path, name + FileExtension));
            }
            catch (IOException)
            {
                // Sometimes the IO is overlapped with another one, I dont want to save if this happens
            }
        }
    }
}
