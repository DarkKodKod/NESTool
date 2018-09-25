using NESTool.Architecture.Signals;
using NESTool.ViewModels;
using NESTool.VOs;

namespace NESTool.Signals
{
    // Generics
    public class BrowseFolderSuccessSignal : Signal<string> { }

    // MainWindowViewModel
    public class WindowGetFocusSignal : Signal { }
    public class NewFileSuccessSignal : Signal { }
    public class OpenProjectSuccessSignal : Signal<ProjectOpenVO> { }
    public class CloseProjectSuccessSignal : Signal { }
    public class SaveSuccessSignal : Signal { }
    public class SaveAllSuccessSignal : Signal { }
    public class ExitSuccessSignal : Signal { }
    public class LoadConfigSuccessSignal : Signal { }
    public class UpdateRecentProjectsSignal : Signal<string[]> { }
    public class BuildProjectSuccessSignal : Signal { }
    public class ProjectItemExpandedSignal : Signal<ProjectItem> { }
    public class ProjectItemSelectedSignal : Signal<ProjectItem> { }

    // ProjectDialogViewModel
    public class CreateProjectSuccessSignal : Signal<string> { }

    // ProjectPropertiesViewModel
    public class SaveConfiguratioSuccessSignal : Signal { }

    // TileSetViewModel

    // BanksViewModel

    // CharacterViewModel

    // MapViewModel
}