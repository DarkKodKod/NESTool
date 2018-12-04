namespace ArchitectureLibrary.Clipboard
{
    public class ClipboardData
    {
        public string AssemblyType { get; set; } = "";
        public string Content { get; set; } = "";

        public void Clear()
        {
            AssemblyType = "";
            Content = "";
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(AssemblyType) && string.IsNullOrEmpty(Content);
        }
    }
}