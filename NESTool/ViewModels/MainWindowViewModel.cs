using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.Utils.Adorners;
using NESTool.ViewModels.ProjectItems;
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
        public OpenProjectCommand OpenProjectCommand { get; } = new OpenProjectCommand();
        public CloseProjectCommand CloseProjectCommand { get; } = new CloseProjectCommand();
        public ExitCommand ExitCommand { get; } = new ExitCommand();
        public NewFileCommand NewFileCommand { get; } = new NewFileCommand();
        public NewProjectCommand NewProjectCommand { get; } = new NewProjectCommand();
        public SaveAllCommand SaveAllCommand { get; } = new SaveAllCommand();
        public SaveCommand SaveCommand { get; } = new SaveCommand();
        public LoadConfigsCommand LoadConfigsCommand { get; } = new LoadConfigsCommand();
        public ShowAboutDialogCommand ShowAboutDialogCommand { get; } = new ShowAboutDialogCommand();
        public BuildProjectCommand BuildProjectCommand { get; } = new BuildProjectCommand();
        public OpenProjectPropertiesCommand OpenProjectPropertiesCommand { get; } = new OpenProjectPropertiesCommand();
        public ViewHelpCommand ViewHelpCommand { get; } = new ViewHelpCommand();
        public CutElementCommand CutElementCommand { get; } = new CutElementCommand();
        public CopyElementCommand CopyElementCommand { get; } = new CopyElementCommand();
        public PasteElementCommand PasteElementCommand { get; } = new PasteElementCommand();
        public DuplicateElementCommand DuplicateElementCommand { get; } = new DuplicateElementCommand();
        public DeleteElementCommand DeleteElementCommand { get; } = new DeleteElementCommand();
        public RenameElementCommand RenameElementCommand { get; } = new RenameElementCommand();
        public CreateNewElementCommand CreateNewElementCommand { get; } = new CreateNewElementCommand();
        public CreateFolderCommand CreateFolderCommand { get; } = new CreateFolderCommand();
        public PreviewMouseLeftButtonDownCommand PreviewMouseLeftButtonDownCommand { get; } = new PreviewMouseLeftButtonDownCommand();
        public PreviewMouseLeftButtonUpCommand PreviewMouseLeftButtonUpCommand { get; } = new PreviewMouseLeftButtonUpCommand();
        public PreviewMouseMoveCommand PreviewMouseMoveCommand { get; } = new PreviewMouseMoveCommand();
        public DragEnterCommand DragEnterCommand { get; } = new DragEnterCommand();
        public DragOverCommand DragOverCommand { get; } = new DragOverCommand();
        public DragLeaveCommand DragLeaveCommand { get; } = new DragLeaveCommand();
        public DropCommand DropCommand { get; } = new DropCommand();
        public ImportImageCommand ImportImageCommand { get; } = new ImportImageCommand();
        public WindowsGetFocusCommand WindowsGetFocusCommand { get; } = new WindowsGetFocusCommand();
        public QueryContinueDragCommand QueryContinueDragCommand { get; } = new QueryContinueDragCommand();

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

        public MainWindowViewModel()
        {
            SignalManager.Get<OpenProjectSuccessSignal>().AddListener(OpenProjectSuccess);
            SignalManager.Get<CloseProjectSuccessSignal>().AddListener(CloseProjectSuccess);
            SignalManager.Get<ExitSuccessSignal>().AddListener(ExitSuccess);
            SignalManager.Get<NewFileSuccessSignal>().AddListener(NewFileSuccess);
            SignalManager.Get<SaveAllSuccessSignal>().AddListener(SaveAllSuccess);
            SignalManager.Get<SaveSuccessSignal>().AddListener(SaveSuccess);
            SignalManager.Get<LoadConfigSuccessSignal>().AddListener(LoadConfigSuccess);
            SignalManager.Get<UpdateRecentProjectsSignal>().AddListener(UpdateRecentProjects);
            SignalManager.Get<BuildProjectSuccessSignal>().AddListener(BuildProjectSuccess);
            SignalManager.Get<ProjectItemSelectedSignal>().AddListener(OnProjectItemSelected);
            SignalManager.Get<WindowGetFocusSignal>().AddListener(OnWindowGetFocus);
            SignalManager.Get<MouseLeftButtonDownSignal>().AddListener(OnMouseLeftButtonDown);
            SignalManager.Get<MouseLeftButtonUpSignal>().AddListener(OnMouseLeftButtonUp);
            SignalManager.Get<MouseMoveSignal>().AddListener(OnMouseMove);
            SignalManager.Get<UpdateAdornersSignal>().AddListener(OnUpdateAdorners);
            SignalManager.Get<InitializeAdornersSignal>().AddListener(OnInitializeAdorners);
            SignalManager.Get<DetachAdornersSignal>().AddListener(OnDetachAdorners);
            SignalManager.Get<SizeChangedSingal>().AddListener(OnSizeChanged);
        }

        private void LoadConfigSuccess()
        {
            var model = ModelManager.Get<NESToolConfigurationModel>();

            UpdateRecentProjects(model.RecentProjects);

            LoadDefaultProject();
        }

        private void LoadDefaultProject()
        {
            var config = ModelManager.Get<NESToolConfigurationModel>();

            if (!string.IsNullOrEmpty(config.DefaultProjectPath))
            {
                var openProjectCommand = new OpenProjectCommand();
                if (openProjectCommand.CanExecute(config.DefaultProjectPath))
                {
                    openProjectCommand.Execute(config.DefaultProjectPath);
                }
            }
        }

        private void UpdateRecentProjects(string[] recentProjects)
        {
            var list = new List<RecentProjectModel>();

            int index = 1;

            foreach (var project in recentProjects)
            {
                if (!string.IsNullOrEmpty(project))
                {
                    // Extract the name of the folder as our project name
                    int startIndex = project.LastIndexOf("\\");
                    var projectName = project.Substring(startIndex + 1, project.Length - startIndex - 1);

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

            var projectName = (string)Application.Current.FindResource(_projectNameKey);

            ProjectName = vo.ProjectName;

            Title = $"{ vo.ProjectName } - { projectName }";
        }

        private void CloseProjectSuccess()
        {
            ProjectItems = null;

            var projectName = (string)Application.Current.FindResource(_projectNameKey);

            ProjectName = "";

            Title = $"{ projectName }";
        }

        private void ExitSuccess()
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
            var diff = _startPoint - vo.Position;

            if ((_isDragging == false) && Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                var treeView = vo.Sender as TreeView;

                var treeViewItem = Util.FindAncestor<TreeViewItem>((DependencyObject)vo.OriginalSource);

                if (treeView == null || treeViewItem == null)
                {
                    return;
                }   

                var folderViewModel = treeView.SelectedItem as ProjectItem;

                if (folderViewModel == null)
                {
                    return;
                }   

                var dragData = new DataObject(folderViewModel);

                DragDrop.DoDragDrop(treeViewItem, dragData, DragDropEffects.Move);

                _isDragging = true;
            }
        }

        private void OnInitializeAdorners(TreeViewItem control, DragEventArgs eventArgs)
        {
            if (_dragAdorner == null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(control);

                Point startPosition = eventArgs.GetPosition(control);

                if (eventArgs.Data.GetDataPresent(typeof(ProjectFolder)))
                {
                    object data = eventArgs.Data.GetData(typeof(ProjectFolder));

                    _dragAdorner = new TreeViewDragAdorner(data, (data as ProjectFolder).GetHeaderTemplate(), control, adornerLayer);
                }
                else
                {
                    object data = eventArgs.Data.GetData(typeof(ProjectItem));

                    _dragAdorner = new TreeViewDragAdorner(data, (data as ProjectItem).GetHeaderTemplate(), control, adornerLayer);
                }

                _dragAdorner.UpdatePosition(startPosition.X, startPosition.Y);
            }

            if (_insertAdorner == null)
            {
                UIElement itemContainer = Util.GetItemContainerFromPoint(control, eventArgs.GetPosition(control));

                if (itemContainer != null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(control);
                    var point = Util.IsPointInTopHalf(control, eventArgs);

                    _insertAdorner = new TreeViewInsertAdorner(point, itemContainer, adornerLayer);
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
                var currentPosition = eventArgs.GetPosition(control);

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

        private void OnWindowGetFocus()
        {
            //
        }

        private void OnProjectItemSelected(ProjectItem item)
        {
            //
        }

        private void NewFileSuccess()
        {
            //
        }

        private void SaveAllSuccess()
        {
            //
        }

        private void SaveSuccess()
        {
            //
        }

        private void BuildProjectSuccess()
        {
            //
        }
    }
}
