using ArchitectureLibrary.Signals;
using NESTool.ViewModels;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Signals
{
    // Generics
    public class BrowseFolderSuccessSignal : Signal<string> { }
    public class SetUpWindowPropertiesSignal : Signal<WindowVO> { }

    // Meta file system
    public class CreateMetaFileSignal : Signal { }
    public class RegisterMetaFileSignal : Signal { }

    // File system
    public class RegisterFileSignal : Signal { }
    public class RegisterFolderSignal : Signal { }
    public class DeleteFileSignal : Signal<ProjectItem> { }
    public class CutFileSignal : Signal<ProjectItem> { }
    public class PasteFileSignal : Signal<ProjectItem, ProjectItem> { }
    public class RenameFileSignal : Signal<ProjectItem> { }
    public class CreateFolderSignal : Signal<ProjectItem> { }
    public class CreateNewElementSignal : Signal<ProjectItem> { }
    public class MoveFileSignal : Signal { }

    // MainWindowViewModel
    public class WindowGetFocusSignal : Signal { }
    public class OpenProjectSuccessSignal : Signal<ProjectOpenVO> { }
    public class CloseProjectSuccessSignal : Signal { }
    public class SaveSuccessSignal : Signal { }
    public class SaveAllSuccessSignal : Signal { }
    public class ExitSuccessSignal : Signal { }
    public class LoadConfigSuccessSignal : Signal { }
    public class UpdateRecentProjectsSignal : Signal<string[]> { }
    public class BuildProjectSuccessSignal : Signal { }
    public class ProjectItemSelectedSignal : Signal<ProjectItem> { }
    public class ProjectItemUnselectedSignal : Signal<ProjectItem> { }
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