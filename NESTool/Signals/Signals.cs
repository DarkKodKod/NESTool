﻿using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.ViewModels;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Signals;

// Generics
public class BrowseFolderSuccessSignal : Signal<Control, string> { }
public class BrowseFileSuccessSignal : Signal<string, bool> { }
public class SetUpWindowPropertiesSignal : Signal<WindowVO> { }
public class CloseDialogSignal : Signal { }
public class ShowGridSignal : Signal { }
public class HideGridSignal : Signal { }
public class ShowGroupMarksSignal : Signal { }
public class HideGroupMarksSignal : Signal { }
public class ProjectConfigurationSavedSignal : Signal { }
public class ProjectItemLoadedSignal : Signal<string> { }
public class ShowLoadingDialogSignal : Signal { }
public class FinishedLoadingProjectSignal : Signal { }
public class WriteBuildOutputSignal : Signal<string, OutputMessageType, string> { }

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

// Banks
public class FileModelVOSelectionChangedSignal : Signal<FileModelVO> { }
public class BankImageUpdatedSignal : Signal { }
public class AddNewTileSetLinkSignal : Signal<BankLinkVO> { }
public class SelectTileSetSignal : Signal<string> { }
public class BankTileDeletedSignal : Signal { }
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
public class ColorPaletteControlSelectedSignal : Signal<Color, PaletteIndex, int> { }
public class UpdateCharacterImageSignal : Signal { }
public class SelectPaletteIndexSignal : Signal<PaletteIndex> { }
public class PaintTileSignal : Signal<int, Point> { }
public class EraseTileSignal : Signal<int> { }
public class SavePropertySignal : Signal<int, bool, bool, int, bool> { }
public class CharacterPaletteIndexSignal : Signal<PaletteIndex> { }

// Map
public class UpdateMapImageSignal : Signal { }
public class MapPaintToolSignal : Signal { }
public class MapSelectToolSignal : Signal { }
public class MapEraseToolSignal : Signal { }
public class DeleteSelectedMapElementSignal : Signal<int> { }
public class MoveUpSelectedMapElementSignal : Signal<int> { }
public class MoveDownSelectedMapElementSignal : Signal<int> { }
public class AddMapElementSignal : Signal<string> { }
public class SaveSelectedPropertyValueSignal : Signal<string> { }
public class AddImageToMapSignal : Signal<Entity> { }
public class RemoveImageToMapSignal : Signal<string> { }
public class ShowSpriteSignal : Signal { }
public class HideSpriteSignal : Signal { }
public class MapElementSpriteSelecteedSignal : Signal<string> { }
public class MapElementSpritePosChangedSignal : Signal<string, int, int> { }
public class SetMapElementImagePosSignal : Signal<string, int, int> { }

// Entity
public class EntitySourceSelectionChangedSignal : Signal<EntitySource> { }
public class CharacterAnimationVOSelectionChangedSignal : Signal<CharacterAnimationVO> { }
public class DeleteSelectedPropertySignal : Signal<string> { }
public class AddPropertySignal : Signal<string> { }