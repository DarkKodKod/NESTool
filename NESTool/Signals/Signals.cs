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
    public class CloseDialogSignal : Signal { }

    // Meta file system
    public class CreateMetaFileSignal : Signal<FileHandleVO> { }
    public class DeleteMetaFileSignal : Signal { }
    public class MoveMetaFileSignal : Signal { }
    public class RenameMetaFileSignal : Signal { }

    // File system
    public class CreateFileSignal : Signal { }

    // Edit
    public class PasteElementSignal : Signal<ProjectItem, ProjectItem> { }
    public class CutElementSignal : Signal<ProjectItem> { }
    public class DeleteElementSignal : Signal<ProjectItem> { }
    public class FindAndCreateElementSignal : Signal<ProjectItem> { }
    public class CreateNewElementSignal : Signal<ProjectItem> { }

    //CreateFileSignal

    // MainWindowViewModel
    public class OpenProjectSuccessSignal : Signal<ProjectOpenVO> { }
    public class CloseProjectSuccessSignal : Signal { }
    public class ExitSuccessSignal : Signal { }
    public class LoadConfigSuccessSignal : Signal { }
    public class UpdateRecentProjectsSignal : Signal<string[]> { }
    public class ProjectItemSelectedSignal : Signal<ProjectItem> { }
    public class ProjectItemUnselectedSignal : Signal<ProjectItem> { }
    public class MouseLeftButtonDownSignal : Signal<Point> { }
    public class MouseLeftButtonUpSignal : Signal { }
    public class MouseMoveSignal : Signal<MouseMoveVO> { }
    public class UpdateAdornersSignal : Signal<TreeViewItem, DragEventArgs> { }
    public class InitializeAdornersSignal : Signal<TreeViewItem, DragEventArgs> { }
    public class DetachAdornersSignal : Signal { }
    public class SizeChangedSingal : Signal<SizeChangedEventArgs, bool> { }
    public class CreateProjectSuccessSignal : Signal<string> { }

    // TileSetViewModel

    // BanksViewModel

    // CharacterViewModel

    // MapViewModel
}