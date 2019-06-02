namespace ArchitectureLibrary.Clipboard
{
    public class ClipboardData
    {
        public string Content { get; set; } = "";
        public string Assembly { get; set; } = "";
        public string Type { get; set; } = "";

        public void Clear()
        {
            Assembly = "";
            Type = "";
            Content = "";
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Assembly)
                && string.IsNullOrEmpty(Type)
                && string.IsNullOrEmpty(Content);
        }
    }
}