using NESTool.Architecture.Signals;
using NESTool.ViewModels;
using System.Collections.Generic;

namespace NESTool.Signals
{
    // Generics
    public class BrowseFolderSuccessSignal : Signal<string> { }

    // MainWindowViewModel
    public class NewFileSuccessSignal : Signal { }
    public class OpenProjectSuccessSignal : Signal<List<ProjectItem>> { }
    public class CloseProjectSuccessSignal : Signal { }
    public class SaveSuccessSignal : Signal { }
    public class SaveAllSuccessSignal : Signal { }
    public class ExitSuccessSignal : Signal { }
    public class LoadConfigSuccessSignal : Signal { }

    // ProjectDialogViewModel
    public class CreateProjectSuccessSignal : Signal<string> { }
}
