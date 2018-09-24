using NESTool.Architecture.Signals;
using NESTool.VOs;

namespace NESTool.Signals
{
    // Generics
    public class BrowseFolderSuccessSignal : Signal<string> { }

    // MainWindowViewModel
    public class NewFileSuccessSignal : Signal { }
    public class OpenProjectSuccessSignal : Signal<ProjectOpenVO> { }
    public class CloseProjectSuccessSignal : Signal { }
    public class SaveSuccessSignal : Signal { }
    public class SaveAllSuccessSignal : Signal { }
    public class ExitSuccessSignal : Signal { }
    public class LoadConfigSuccessSignal : Signal { }
    public class UpdateRecentProjectsSignal : Signal<string[]> { }
    public class BuildProjectSuccessSignal : Signal { }

    // ProjectDialogViewModel
    public class CreateProjectSuccessSignal : Signal<string> { }
}
