using NESTool.ViewModels;
using System.Collections.Generic;

namespace NESTool.VOs;

public record ProjectOpenVO
{
    public List<ProjectItem>? Items { get; init; }
    public string ProjectName { get; init; } = string.Empty;
}
