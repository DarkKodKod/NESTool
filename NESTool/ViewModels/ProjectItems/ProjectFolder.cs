using NESTool.Enums;

namespace NESTool.ViewModels.ProjectItems
{
    public class ProjectFolder : ProjectItem
    {
        public ProjectFolder(string displayName, string fullPath, ProjectItemType type) : base(displayName, fullPath, type)
        {
        }

        public ProjectItemType Group { get; set; }
    }
}
