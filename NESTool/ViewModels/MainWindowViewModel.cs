﻿using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using ArchitectureLibrary.WPF.Adorners;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NESTool.ViewModels;

[SupportedOSPlatform("windows")]
public class MainWindowViewModel : ViewModel
{
    #region Commands
    public OpenProjectCommand OpenProjectCommand { get; } = new();
    public CloseProjectCommand CloseProjectCommand { get; } = new();
    public DispatchSignalCommand<ExitSuccessSignal> ExitCommand { get; } = new();
    public NewProjectCommand NewProjectCommand { get; } = new();
    public NewElementCommand NewElementCommand { get; } = new();
    public LoadConfigsCommand LoadConfigsCommand { get; } = new();
    public ShowAboutDialogCommand ShowAboutDialogCommand { get; } = new();
    public OpenBuildProjectCommand OpenBuildProjectCommand { get; } = new();
    public OpenProjectPropertiesCommand OpenProjectPropertiesCommand { get; } = new();
    public ViewHelpCommand ViewHelpCommand { get; } = new();
    public CopyElementCommand CopyElementCommand { get; } = new();
    public PasteElementCommand PasteElementCommand { get; } = new();
    public DuplicateElementCommand DuplicateElementCommand { get; } = new();
    public DeleteElementCommand DeleteElementCommand { get; } = new();
    public EnableRenameElementCommand EnableRenameElementCommand { get; } = new();
    public CreateFolderCommand CreateFolderCommand { get; } = new();
    public CreateElementFromMenuCommand CreateElementFromMenuCommand { get; } = new();
    public PreviewMouseLeftButtonDownCommand PreviewMouseLeftButtonDownCommand { get; } = new();
    public DispatchSignalCommand<MouseLeftButtonUpSignal> PreviewMouseLeftButtonUpCommand { get; } = new();
    public PreviewMouseMoveCommand PreviewMouseMoveCommand { get; } = new();
    public DragEnterCommand DragEnterCommand { get; } = new();
    public DragOverCommand DragOverCommand { get; } = new();
    public DragLeaveCommand DragLeaveCommand { get; } = new();
    public DropCommand DropCommand { get; } = new();
    public OpenImportImageDialogCommand OpenImportImageDialogCommand { get; } = new();
    public WindowsGetFocusCommand WindowsGetFocusCommand { get; } = new();
    public QueryContinueDragCommand QueryContinueDragCommand { get; } = new();
    public LoadMappersCommand LoadMappersCommand { get; } = new();
    public UndoCommand UndoCommand { get; } = new();
    public RedoCommand RedoCommand { get; } = new();
    public TreeviewSelectedItemChangedCommand TreeviewSelectedItemChangedCommand { get; } = new();
    public DispatchSignalCommand<ShowGridSignal> ShowGridCommand { get; } = new();
    public DispatchSignalCommand<HideGridSignal> HideGridCommand { get; } = new();
    public DispatchSignalCommand<ShowSpriteSignal> ShowSpriteCommand { get; } = new();
    public DispatchSignalCommand<HideSpriteSignal> HideSpriteCommand { get; } = new();
    public DispatchSignalCommand<ShowGroupMarksSignal> ShowGroupMarksCommand { get; } = new();
    public DispatchSignalCommand<HideGroupMarksSignal> HideGroupMarksCommand { get; } = new();
    public DispatchSignalCommand<MapPaintToolSignal> MapPaintToolCommand { get; } = new();
    public DispatchSignalCommand<MapEraseToolSignal> MapEraseToolCommand { get; } = new();
    public DispatchSignalCommand<MapSelectToolSignal> MapSelectToolCommand { get; } = new();
    #endregion

    private const string _projectNameKey = "applicationTitle";

    private string _title = string.Empty;
    private string _projectName = string.Empty;
    private List<ProjectItem>? _projectItems;
    private List<RecentProjectModel> _recentProjects = [];
    private bool? _isFullscreen = null;
    private readonly string _appName;

    #region Drag & Drop
    private Point _startPoint;
    private TreeViewInsertAdorner? _insertAdorner;
    private TreeViewDragAdorner? _dragAdorner;
    private bool _isDragging = false;
    #endregion

    #region get/set
    public string ProjectName
    {
        get => _projectName;
        set
        {
            _projectName = value;
            OnPropertyChanged("ProjectName");
        }
    }

    public List<RecentProjectModel> RecentProjects
    {
        get => _recentProjects;
        set
        {
            _recentProjects = value;
            OnPropertyChanged("RecentProjects");
        }
    }

    public List<ProjectItem>? ProjectItems
    {
        get => _projectItems;
        set
        {
            _projectItems = value;
            OnPropertyChanged("ProjectItems");
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged("Title");
        }
    }
    #endregion

    public MainWindowViewModel()
    {
        _appName = (string)Application.Current.FindResource(_projectNameKey);

        Title = _appName;

        #region Signals
        SignalManager.Get<OpenProjectSuccessSignal>().Listener += OpenProjectSuccess;
        SignalManager.Get<CloseProjectSuccessSignal>().Listener += OnCloseProjectSuccess;
        SignalManager.Get<ExitSuccessSignal>().Listener += OnExitSuccess;
        SignalManager.Get<LoadConfigSuccessSignal>().Listener += OnLoadConfigSuccess;
        SignalManager.Get<UpdateRecentProjectsSignal>().Listener += OnUpdateRecentProjects;
        SignalManager.Get<MouseLeftButtonDownSignal>().Listener += OnMouseLeftButtonDown;
        SignalManager.Get<MouseLeftButtonUpSignal>().Listener += OnMouseLeftButtonUp;
        SignalManager.Get<MouseMoveSignal>().Listener += OnMouseMove;
        SignalManager.Get<UpdateAdornersSignal>().Listener += OnUpdateAdorners;
        SignalManager.Get<InitializeAdornersSignal>().Listener += OnInitializeAdorners;
        SignalManager.Get<DetachAdornersSignal>().Listener += OnDetachAdorners;
        SignalManager.Get<SizeChangedSignal>().Listener += OnSizeChanged;
        SignalManager.Get<CreateProjectSuccessSignal>().Listener += OnCreateProjectSuccess;
        SignalManager.Get<DeleteElementSignal>().Listener += OnDeleteElement;
        SignalManager.Get<PasteElementSignal>().Listener += OnPasteElement;
        SignalManager.Get<FindAndCreateElementSignal>().Listener += OnFindAndCreateElement;
        SignalManager.Get<DropElementSignal>().Listener += OnDropElement;
        #endregion
    }

    #region Signal methods
    private ProjectItem FindInItemsAndDelete(ICollection<ProjectItem> items, ProjectItem[] aPath, int index)
    {
        bool res = items.Contains(aPath[index]);

        if (res && index > 0)
        {
            return FindInItemsAndDelete(aPath[index].Items, aPath, index - 1);
        }

        ProjectItem copy = aPath[index];

        aPath[index].Parent?.Items.Remove(aPath[index]);

        return copy;
    }

    private void OnPasteElement(ProjectItem item, ProjectItem newItem)
    {
        if (item.IsFolder)
        {
            newItem.Parent = item;
            item.Items.Add(newItem);
        }
        else
        {
            newItem.Parent = item.Parent;
            item.Parent?.Items.Add(newItem);
        }

        OnPropertyChanged("ProjectItems");
    }

    private void OnDeleteElement(ProjectItem item)
    {
        DeleteItemFromTheList(item);
    }

    private void DeleteItemFromTheList(ProjectItem item)
    {
        // Collect the chain of parents for later use
        List<ProjectItem> path = new List<ProjectItem>() { item };

        ProjectItem? originalParent = item.Parent;

        ProjectItem? parent = item.Parent;

        while (parent != null)
        {
            path.Add(parent);

            parent = parent.Parent;
        }

        if (_projectItems != null)
        {
            ProjectItem matchItem = FindInItemsAndDelete(_projectItems, path.ToArray(), path.ToArray().Length - 1);

            if (matchItem != null)
            {
                OnPropertyChanged("ProjectItems");

                if (originalParent != null)
                    SignalManager.Get<UpdateFolderSignal>().Dispatch(originalParent);
            }
        }
    }

    private void OnLoadConfigSuccess()
    {
        NESToolConfigurationModel model = ModelManager.Get<NESToolConfigurationModel>();

        OnUpdateRecentProjects(model.RecentProjects);

        LoadDefaultProject();
    }

    private void LoadDefaultProject()
    {
        NESToolConfigurationModel config = ModelManager.Get<NESToolConfigurationModel>();

        if (!string.IsNullOrEmpty(config.DefaultProjectPath))
        {
            using (OpenProjectCommand openProjectCommand = new())
            {
                if (openProjectCommand.CanExecute(config.DefaultProjectPath))
                {
                    openProjectCommand.Execute(config.DefaultProjectPath);
                }
            }
        }
    }

    private void OnCreateProjectSuccess(string projectFullPath)
    {
        // After creating the project now it is time to open it
        using (OpenProjectCommand openProjectCommand = new())
        {
            if (openProjectCommand.CanExecute(projectFullPath))
            {
                openProjectCommand.Execute(projectFullPath);
            }
        }
    }

    private void OnUpdateRecentProjects(string[] recentProjects)
    {
        List<RecentProjectModel> list = new();

        int index = 1;

        foreach (string project in recentProjects)
        {
            if (!string.IsNullOrEmpty(project))
            {
                // Extract the name of the folder as our project name
                int startIndex = project.LastIndexOf("\\");
                string projectName = project.Substring(startIndex + 1, project.Length - startIndex - 1);

                list.Add(new RecentProjectModel()
                {
                    Path = project,
                    DisplayName = $"_{index} {projectName} ({project})"
                });

                index++;
            }
        }

        RecentProjects = list;
    }

    private void OpenProjectSuccess(ProjectOpenVO vo)
    {
        ProjectItems = vo.Items;

        ProjectName = vo.ProjectName;

        Title = $"{vo.ProjectName} - {_appName}";

        ProjectModel project = ModelManager.Get<ProjectModel>();

        if (project.Name != vo.ProjectName)
        {
            project.Name = vo.ProjectName;

            project.Save();
        }

        project.Name = vo.ProjectName;
    }

    private void OnCloseProjectSuccess()
    {
        ProjectItems = null;

        ProjectName = "";

        Title = $"{_appName}";
    }

    private void OnExitSuccess()
    {
        Application.Current.Shutdown();
    }

    private void OnMouseLeftButtonDown(Point point)
    {
        _startPoint = point;

        _isDragging = false;
    }

    private void OnMouseLeftButtonUp()
    {
        _isDragging = false;
    }

    private void OnMouseMove(MouseMoveVO vo)
    {
        Vector diff = _startPoint - vo.Position;

        if ((_isDragging == false) && Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
        {
            TreeViewItem? treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject?)vo.OriginalSource);

            if (!(vo.Sender is TreeView treeView) || treeViewItem == null)
            {
                return;
            }

            if (!(treeView.SelectedItem is ProjectItem projectItem) || projectItem.IsRoot)
            {
                return;
            }

            DataObject dragData = new(projectItem);

            DragDrop.DoDragDrop(treeViewItem, dragData, DragDropEffects.Move);

            _isDragging = true;
        }
    }

    private void OnInitializeAdorners(TreeViewItem control, DragEventArgs eventArgs)
    {
        if (_dragAdorner == null)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(control);

            Point startPosition = eventArgs.GetPosition(control);

            object? data = eventArgs.Data.GetData(typeof(ProjectItem));

            if (data is ProjectItem d)
            {
                _dragAdorner = new TreeViewDragAdorner(data, d.GetHeaderTemplate(), control, adornerLayer);

                _dragAdorner.UpdatePosition(startPosition.X, startPosition.Y);
            }
        }

        if (_insertAdorner == null)
        {
            UIElement? itemContainer = Util.GetItemContainerFromPoint(control, eventArgs.GetPosition(control));

            if (itemContainer != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(control);
                bool isTopHalf = Util.IsPointInTopHalf(control, eventArgs);

                _insertAdorner = new TreeViewInsertAdorner(isTopHalf, itemContainer, adornerLayer);
            }
        }
    }

    private void OnUpdateAdorners(TreeViewItem control, DragEventArgs eventArgs)
    {
        if (_insertAdorner != null)
        {
            _insertAdorner.IsTopHalf = Util.IsPointInTopHalf(control, eventArgs);
            _insertAdorner.InvalidateVisual();
        }

        if (_dragAdorner != null)
        {
            Point currentPosition = eventArgs.GetPosition(control);

            _dragAdorner.UpdatePosition(currentPosition.X, currentPosition.Y);
        }
    }

    private void OnDetachAdorners()
    {
        _isDragging = false;

        if (_insertAdorner != null)
        {
            _insertAdorner.Destroy();
            _insertAdorner = null;
        }

        if (_dragAdorner != null)
        {
            _dragAdorner.Destroy();
            _dragAdorner = null;
        }
    }

    private void OnSizeChanged(SizeChangedEventArgs args, bool fullscreen)
    {
        bool changed = false;

        if (args.HeightChanged)
        {
            ModelManager.Get<NESToolConfigurationModel>().WindowSizeY = (int)args.NewSize.Height;

            changed = true;
        }

        if (args.WidthChanged)
        {
            ModelManager.Get<NESToolConfigurationModel>().WindowSizeX = (int)args.NewSize.Width;

            changed = true;
        }

        if (_isFullscreen != fullscreen)
        {
            _isFullscreen = fullscreen;

            ModelManager.Get<NESToolConfigurationModel>().FullScreen = fullscreen;

            changed = true;
        }

        if (changed)
        {
            ModelManager.Get<NESToolConfigurationModel>().Save();
        }
    }

    private void OnFindAndCreateElement(ProjectItem newElement)
    {
        if (ProjectItems == null)
            return;

        foreach (ProjectItem item in ProjectItems)
        {
            if (item.IsRoot == true && item.Type == newElement.Type)
            {
                newElement.Parent = item;

                item.Items.Add(newElement);

                SignalManager.Get<CreateNewElementSignal>().Dispatch(newElement);

                break;
            }
        }
    }

    private void OnDropElement(ProjectItem targetElement, ProjectItem draggedElement)
    {
        draggedElement.Parent?.Items.Remove(draggedElement);

        if (draggedElement.Parent != null)
            SignalManager.Get<UpdateFolderSignal>().Dispatch(draggedElement.Parent);

        if (targetElement.IsFolder)
        {
            draggedElement.Parent = targetElement;
            targetElement.Items.Add(draggedElement);

            SignalManager.Get<UpdateFolderSignal>().Dispatch(targetElement);
        }
        else
        {
            draggedElement.Parent = targetElement.Parent;
            targetElement.Parent?.Items.Add(draggedElement);

            if (targetElement.Parent != null)
                SignalManager.Get<UpdateFolderSignal>().Dispatch(targetElement.Parent);
        }

        draggedElement.IsSelected = true;

        OnPropertyChanged("ProjectItems");
    }
    #endregion
}
