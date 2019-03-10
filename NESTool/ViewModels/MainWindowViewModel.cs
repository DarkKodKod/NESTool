using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.Utils.Adorners;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NESTool.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Commands
        public OpenProjectCommand OpenProjectCommand { get; } = new OpenProjectCommand();
        public CloseProjectCommand CloseProjectCommand { get; } = new CloseProjectCommand();
        public ExitCommand ExitCommand { get; } = new ExitCommand();
        public NewProjectCommand NewProjectCommand { get; } = new NewProjectCommand();
        public NewElementCommand NewElementCommand { get; } = new NewElementCommand();
        public SaveAllCommand SaveAllCommand { get; } = new SaveAllCommand();
        public SaveCommand SaveCommand { get; } = new SaveCommand();
        public LoadConfigsCommand LoadConfigsCommand { get; } = new LoadConfigsCommand();
        public ShowAboutDialogCommand ShowAboutDialogCommand { get; } = new ShowAboutDialogCommand();
        public BuildProjectCommand BuildProjectCommand { get; } = new BuildProjectCommand();
        public OpenProjectPropertiesCommand OpenProjectPropertiesCommand { get; } = new OpenProjectPropertiesCommand();
        public ViewHelpCommand ViewHelpCommand { get; } = new ViewHelpCommand();
        public CopyElementCommand CopyElementCommand { get; } = new CopyElementCommand();
        public PasteElementCommand PasteElementCommand { get; } = new PasteElementCommand();
        public DuplicateElementCommand DuplicateElementCommand { get; } = new DuplicateElementCommand();
        public DeleteElementCommand DeleteElementCommand { get; } = new DeleteElementCommand();
        public EnableRenameElementCommand EnableRenameElementCommand { get; } = new EnableRenameElementCommand();
        public CreateFolderCommand CreateFolderCommand { get; } = new CreateFolderCommand();
        public CreateElementFromMenuCommand CreateElementFromMenuCommand { get; } = new CreateElementFromMenuCommand();
        public PreviewMouseLeftButtonDownCommand PreviewMouseLeftButtonDownCommand { get; } = new PreviewMouseLeftButtonDownCommand();
        public PreviewMouseLeftButtonUpCommand PreviewMouseLeftButtonUpCommand { get; } = new PreviewMouseLeftButtonUpCommand();
        public PreviewMouseMoveCommand PreviewMouseMoveCommand { get; } = new PreviewMouseMoveCommand();
        public DragEnterCommand DragEnterCommand { get; } = new DragEnterCommand();
        public DragOverCommand DragOverCommand { get; } = new DragOverCommand();
        public DragLeaveCommand DragLeaveCommand { get; } = new DragLeaveCommand();
        public DropCommand DropCommand { get; } = new DropCommand();
        public OpenImportImageDialogCommand OpenImportImageDialogCommand { get; } = new OpenImportImageDialogCommand();
        public WindowsGetFocusCommand WindowsGetFocusCommand { get; } = new WindowsGetFocusCommand();
        public QueryContinueDragCommand QueryContinueDragCommand { get; } = new QueryContinueDragCommand();
        public LoadMappersCommand LoadMappersCommand { get; } = new LoadMappersCommand();
        public UndoCommand UndoCommand { get; } = new UndoCommand();
        public RedoCommand RedoCommand { get; } = new RedoCommand();
        public TreeviewSelectedItemChangedCommand TreeviewSelectedItemChangedCommand { get; } = new TreeviewSelectedItemChangedCommand();
        #endregion

        private const string _projectNameKey = "applicationTitle";

        private string _title = "";
        private string _projectName;
        private List<ProjectItem> _projectItems;
        private List<RecentProjectModel> _recentProjects = new List<RecentProjectModel>();
        private bool? _isFullscreen = null;

        #region Drag & Drop
        private Point _startPoint;
        private TreeViewInsertAdorner _insertAdorner;
        private TreeViewDragAdorner _dragAdorner;
        private bool _isDragging = false;
        #endregion

        #region get/set
        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                _projectName = value;
                OnPropertyChanged("ProjectName");
            }
        }

        public List<RecentProjectModel> RecentProjects
        {
            get { return _recentProjects; }
            set
            {
                _recentProjects = value;
                OnPropertyChanged("RecentProjects");
            }
        }

        public List<ProjectItem> ProjectItems
        {
            get { return _projectItems; }
            set
            {
                _projectItems = value;
                OnPropertyChanged("ProjectItems");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            #region Signals
            SignalManager.Get<OpenProjectSuccessSignal>().AddListener(OpenProjectSuccess);
            SignalManager.Get<CloseProjectSuccessSignal>().AddListener(OnCloseProjectSuccess);
            SignalManager.Get<ExitSuccessSignal>().AddListener(OnExitSuccess);
            SignalManager.Get<LoadConfigSuccessSignal>().AddListener(OnLoadConfigSuccess);
            SignalManager.Get<UpdateRecentProjectsSignal>().AddListener(OnUpdateRecentProjects);
            SignalManager.Get<MouseLeftButtonDownSignal>().AddListener(OnMouseLeftButtonDown);
            SignalManager.Get<MouseLeftButtonUpSignal>().AddListener(OnMouseLeftButtonUp);
            SignalManager.Get<MouseMoveSignal>().AddListener(OnMouseMove);
            SignalManager.Get<UpdateAdornersSignal>().AddListener(OnUpdateAdorners);
            SignalManager.Get<InitializeAdornersSignal>().AddListener(OnInitializeAdorners);
            SignalManager.Get<DetachAdornersSignal>().AddListener(OnDetachAdorners);
            SignalManager.Get<SizeChangedSignal>().AddListener(OnSizeChanged);
            SignalManager.Get<CreateProjectSuccessSignal>().AddListener(OnCreateProjectSuccess);
            SignalManager.Get<DeleteElementSignal>().AddListener(OnDeleteElement);
            SignalManager.Get<PasteElementSignal>().AddListener(OnPasteElement);
            SignalManager.Get<FindAndCreateElementSignal>().AddListener(OnFindAndCreateElement);
            SignalManager.Get<DropElementSignal>().AddListener(OnDropElement);
            #endregion
        }

        #region Signal methods
        private ProjectItem FindInItemsAndDelete(ICollection<ProjectItem> items, ProjectItem[] aPath, int index)
        {
            bool res = items.Contains(aPath[index]);

            if (res == true && index > 0)
            {
                return FindInItemsAndDelete(aPath[index].Items, aPath, index - 1);
            }

            ProjectItem copy = aPath[index];

            aPath[index].Parent.Items.Remove(aPath[index]);

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
                item.Parent.Items.Add(newItem);
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

            ProjectItem originalParent = item.Parent;

            ProjectItem parent = item.Parent;

            while (parent != null)
            {
                path.Add(parent);

                parent = parent.Parent;
            }

            ProjectItem matchItem = FindInItemsAndDelete(_projectItems, path.ToArray(), path.ToArray().Length - 1);

            if (matchItem != null)
            {
                OnPropertyChanged("ProjectItems");

                SignalManager.Get<UpdateFolderSignal>().Dispatch(originalParent);
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
                OpenProjectCommand openProjectCommand = new OpenProjectCommand();
                if (openProjectCommand.CanExecute(config.DefaultProjectPath))
                {
                    openProjectCommand.Execute(config.DefaultProjectPath);
                }
            }
        }

        private void OnCreateProjectSuccess(string projectFullPath)
        {
            // After creating the project now it is time to open it
            OpenProjectCommand openProjectCommand = new OpenProjectCommand();
            if (openProjectCommand.CanExecute(projectFullPath))
            {
                openProjectCommand.Execute(projectFullPath);
            }
        }

        private void OnUpdateRecentProjects(string[] recentProjects)
        {
            List<RecentProjectModel> list = new List<RecentProjectModel>();

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
                        DisplayName = $"_{ index } {projectName} ({project})"
                    });

                    index++;
                }
            }

            RecentProjects = list;
        }

        private void OpenProjectSuccess(ProjectOpenVO vo)
        {
            ProjectItems = vo.Items;

            string projectName = (string)Application.Current.FindResource(_projectNameKey);

            ProjectName = vo.ProjectName;

            Title = $"{ vo.ProjectName } - { projectName }";

            ProjectModel project = ModelManager.Get<ProjectModel>();
            project.Name = vo.ProjectName;
        }

        private void OnCloseProjectSuccess()
        {
            ProjectItems = null;

            string projectName = (string)Application.Current.FindResource(_projectNameKey);

            ProjectName = "";

            Title = $"{ projectName }";
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
                TreeViewItem treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)vo.OriginalSource);

                if (!(vo.Sender is TreeView treeView) || treeViewItem == null)
                {
                    return;
                }

                if (!(treeView.SelectedItem is ProjectItem projectItem) || projectItem.IsRoot)
                {
                    return;
                }

                DataObject dragData = new DataObject(projectItem);

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

                object data = eventArgs.Data.GetData(typeof(ProjectItem));

                _dragAdorner = new TreeViewDragAdorner(data, (data as ProjectItem).GetHeaderTemplate(), control, adornerLayer);

                _dragAdorner.UpdatePosition(startPosition.X, startPosition.Y);
            }

            if (_insertAdorner == null)
            {
                UIElement itemContainer = Util.GetItemContainerFromPoint(control, eventArgs.GetPosition(control));

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
            draggedElement.Parent.Items.Remove(draggedElement);

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
                targetElement.Parent.Items.Add(draggedElement);

                SignalManager.Get<UpdateFolderSignal>().Dispatch(targetElement.Parent);
            }

            draggedElement.IsSelected = true;

            OnPropertyChanged("ProjectItems");
        }
        #endregion
    }
}
