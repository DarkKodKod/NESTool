using ArchitectureLibrary.Signals;
using NESTool.ViewModels.ProjectItems;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;

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
    public class MouseLeftButtonDownSignal : Signal<Point> { }
    public class MouseMoveSignal : Signal<MouseMoveVO> { }
    public class UpdateInsertAdornerSignal : Signal<TreeViewItem, DragEventArgs> { }
    public class InitializeInsertAdornerSignal : Signal<TreeViewItem, DragEventArgs> { }
    public class DetachAdornersSignal : Signal { }

    // ProjectDialogViewModel
    public class CreateProjectSuccessSignal : Signal<string> { }

    // ProjectPropertiesViewModel
    public class SaveConfiguratioSuccessSignal : Signal { }

    // TileSetViewModel

    // BanksViewModel

    // CharacterViewModel

    // MapViewModel
}