using ArchitectureLibrary.Signals;
using NESTool.ViewModels.ProjectItems;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Signals
{
    // Generics
    public class BrowseFolderSuccessSignal : Signal<string> { }
    public class SetUpWindowPropertiesSignal : Signal<WindowVO> { }

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
    public class ProjectItemSelectedSignal : Signal<ProjectItem> { }
    public class MouseLeftButtonDownSignal : Signal<Point> { }
    public class MouseLeftButtonUpSignal : Signal { }
    public class MouseMoveSignal : Signal<MouseMoveVO> { }
    public class UpdateAdornersSignal : Signal<TreeViewItem, DragEventArgs> { }
    public class InitializeAdornersSignal : Signal<TreeViewItem, DragEventArgs> { }
    public class DetachAdornersSignal : Signal { }
    public class SizeChangedSingal : Signal<SizeChangedEventArgs, bool> { }
    public class LoadMappersSuccessSignal : Signal { }
    public class CreateProjectSuccessSignal : Signal<string> { }

    // ProjectDialogViewModel
    public class ClosedNewProjectSignal : Signal { }

    // ProjectPropertiesViewModel
    public class ClosedProjectPropertiesSignal : Signal { }

    // TileSetViewModel

    // BanksViewModel

    // CharacterViewModel

    // MapViewModel
}