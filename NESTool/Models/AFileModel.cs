namespace NESTool.Models
{
    public abstract class AFileModel
    {
        public abstract string FileExtension { get; }

        protected string _fileExtension = "";
    }
}
