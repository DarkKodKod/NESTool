using NESTool.ViewModels.ProjectItems;
using System.Collections.Generic;

namespace NESTool.VOs
{
    public class ProjectOpenVO
    {
        public List<ProjectItem> Items { get; set; }
        public string ProjectName { get; set; }
    }
}
