using ArchitectureLibrary.Signals;
using NESTool.ViewModels;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Signals
{
    // Generics
    public class BrowseFolderSuccessSignal : Signal<string> { }
    public class BrowseFileSuccessSignal : Signal<string, bool> { }
    public class SetUpWindowPropertiesSignal : Signal<WindowVO> { }
    public class CloseDialogSignal : Signal { }
    public class ShowGridSignal : Signal { }
    public class HideGridSignal : Signal { }
    public class ProjectConfigurationSavedSignal : Signal { }

    // Edit
    public class PasteElementSignal : Signal<ProjectItem, ProjectItem> { }
    public class MoveElementSignal : Signal<ProjectItem, ProjectItem> { }
    public class DeleteElementSignal : Signal<ProjectItem> { }
    public class FindAndCreateElementSignal : Signal<ProjectItem> { }
    public class CreateNewElementSignal : Signal<ProjectItem> { }

    // FileSystem
    public class RegisterFileHandlerSignal : Signal<ProjectItem, string> { }
    public class RenameFileSignal : Signal<ProjectItem> { }

    // MainWindowViewModel
    public class UpdateFolderSignal : Signal<ProjectItem> { }
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
    public class SizeChangedSignal : Signal<SizeChangedEventArgs, bool> { }
    public class CreateProjectSuccessSignal : Signal<string> { }
    public class DropElementSignal : Signal<ProjectItem, ProjectItem> { }
    public class LoadProjectItemSignal : Signal<ProjectItem> { }

    // TileSet
    public class UpdateTileSetImageSignal : Signal { }
    public class MouseWheelSignal : Signal<MouseWheelVO> { }
    public class OutputSelectedQuadrantSignal : Signal<Image, WriteableBitmap, Point> { }
    public class SelectedPixelSignal : Signal<WriteableBitmap, Point> { }
    public class ColorPaletteSelectSignal : Signal<Color> { }
    public class SavedPixelChangesSignal : Signal { }

    // PatternTable
    public class FileModelVOSelectionChangedSignal : Signal<FileModelVO> { }
    public class PatternTableImageUpdatedSignal : Signal { }
    public class AddNewTileSetLinkSignal : Signal<PatternTableLinkVO> { }
    public class SelectTileSetSignal : Signal<string> { }
    public class PatternTableTileDeletedSignal : Signal { }
    public class CleanupTileSetLinksSignal : Signal { }

    // Character
    public class AnimationTabDeletedSignal : Signal<ActionTabItem> { }
    public class AnimationTabNewSignal : Signal { }
    public class RenamedAnimationTabSignal : Signal<string> { }
    public class PauseCharacterAnimationSignal : Signal<string> { }
    public class NextFrameCharacterAnimationSignal : Signal<string> { }
    public class StopCharacterAnimationSignal : Signal<string> { }
    public class PlayCharacterAnimationSignal : Signal<string> { }
    public class PreviousFrameCharacterAnimationSignal : Signal<string> { }
    public class NewAnimationFrameSignal : Signal<string> { }
    public class SwitchCharacterFrameViewSignal : Signal<string, int> { }
    public class DeleteAnimationFrameSignal : Signal<string, int> { }
    public class ColorPaletteControlSelectedSignal : Signal<Color, int, int, string> { }
    public class ColorPaletteCleanupSignal : Signal<string, int> { }

    // Map
}