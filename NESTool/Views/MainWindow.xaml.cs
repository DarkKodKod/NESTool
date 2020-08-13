using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.Utils;
using NESTool.ViewModels;
using NESTool.Views;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace NESTool
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MonitorInfoEx
    {
        public int cbSize;
        public Rect rcMonitor;
        public Rect rcWork;
        public UInt32 dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string szDeviceName;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProjectItemType _currentViewType = ProjectItemType.None;

        public static bool ToolBarTileSetShowHideGrid = true;
        public static bool ToolBarBanksShowHideGroupMarks = false;
        public static EditFrameTools ToolBarMapTool = EditFrameTools.Select;

        private readonly LoadingDialog _loadingDialog = new LoadingDialog();
        private readonly FieldInfo _menuDropAlignmentField;

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("User32")]
        public static extern IntPtr MonitorFromWindow(IntPtr hWnd, int dwFlags);

        [DllImport("user32", EntryPoint = "GetMonitorInfo", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetMonitorInfoEx(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        public MainWindow()
        {
            InitializeComponent();

            // Hack to correct the Menu display orientation
            _menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            System.Diagnostics.Debug.Assert(_menuDropAlignmentField != null);

            EnsureStandardPopupAlignment();
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;

            SignalManager.Get<SetUpWindowPropertiesSignal>().AddListener(OnSetUpWindowProperties);
            SignalManager.Get<CreateNewElementSignal>().AddListener(OnCreateNewElement);
            SignalManager.Get<UpdateFolderSignal>().AddListener(OnUpdateFolder);
            SignalManager.Get<LoadProjectItemSignal>().AddListener(OnLoadProjectItem);
            SignalManager.Get<DeleteElementSignal>().AddListener(OnDeleteElement);
            SignalManager.Get<ShowGridSignal>().AddListener(OnShowGrid);
            SignalManager.Get<HideGridSignal>().AddListener(OnHideGrid);
            SignalManager.Get<ShowGroupMarksSignal>().AddListener(OnShowGroupMarks);
            SignalManager.Get<HideGroupMarksSignal>().AddListener(OnHideGroupMarks);
            SignalManager.Get<MapPaintToolSignal>().AddListener(OnMapPaintTool);
            SignalManager.Get<MapSelectToolSignal>().AddListener(OnMapSelectTool);
            SignalManager.Get<MapEraseToolSignal>().AddListener(OnMapEraseTool);
            SignalManager.Get<CloseProjectSuccessSignal>().AddListener(OnCloseProjectSuccess);
            SignalManager.Get<ShowLoadingDialogSignal>().AddListener(OnShowLoadingDialog);
            SignalManager.Get<HideLoadingDialogSignal>().AddListener(OnHideLoadingDialog);

            // Initialize visibility statte of the toolbars
            tbrTileSet.Visibility = Visibility.Collapsed;
            tbrMap.Visibility = Visibility.Collapsed;
            tbrBanks.Visibility = Visibility.Collapsed;
        }

        private void OnHideGrid()
        {
            ToolBarTileSetShowHideGrid = false;
        }

        private void OnShowGrid()
        {
            ToolBarTileSetShowHideGrid = true;
        }

        private void OnShowGroupMarks()
        {
            ToolBarBanksShowHideGroupMarks = true;
        }

        private void OnHideGroupMarks()
        {
            ToolBarBanksShowHideGroupMarks = false;
        }

        private void OnMapPaintTool()
        {
            tbMapSelect.IsChecked = false;
            tbMapErase.IsChecked = false;

            ToolBarMapTool = EditFrameTools.Paint;
        }
        private void OnMapSelectTool()
        {
            tbMapPaint.IsChecked = false;
            tbMapErase.IsChecked = false;

            ToolBarMapTool = EditFrameTools.Select;
        }
        private void OnMapEraseTool()
        {
            tbMapSelect.IsChecked = false;
            tbMapPaint.IsChecked = false;

            ToolBarMapTool = EditFrameTools.Erase;
        }

        private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            EnsureStandardPopupAlignment();
        }

        private void OnLoadProjectItem(ProjectItem item)
        {
            if (item.IsRoot || item.IsFolder)
            {
                return;
            }

            if (dpItemPanel.Children.Count > 0)
            {
                if (dpItemPanel.Children[0] is UserControl oldGui)
                {
                    if (oldGui.DataContext is ItemViewModel oldModel)
                    {
                        oldModel.OnDeactivate();
                    }
                }

                if (dpItemPanel.Children[0] is ICleanable cloneable)
                {
                    cloneable.CleanUp();
                }

                dpItemPanel.Children.Clear();
            }

            UserControl view = null;

            tbrTileSet.Visibility = Visibility.Collapsed;
            tbrMap.Visibility = Visibility.Collapsed;
            tbrBanks.Visibility = Visibility.Collapsed;

            switch (item.Type)
            {
                case ProjectItemType.Bank:
                    tbrBanks.Visibility = Visibility.Visible;

                    tbShowHideGroupMarks.IsChecked = ToolBarBanksShowHideGroupMarks;

                    view = new Banks(); 
                    break;
                case ProjectItemType.Character: view = new Character(); break;
                case ProjectItemType.Map:
                    tbrMap.Visibility = Visibility.Visible;

                    switch (ToolBarMapTool)
                    {
                        case EditFrameTools.Select:
                            tbMapSelect.IsChecked = true;
                            break;
                        case EditFrameTools.Erase:
                            tbMapErase.IsChecked = true;
                            break;
                        case EditFrameTools.Paint:
                            tbMapPaint.IsChecked = true;
                            break;
                    }

                    view = new Map();
                    break;
                case ProjectItemType.TileSet:
                    tbrTileSet.Visibility = Visibility.Visible;

                    tbShowHideGrid.IsChecked = ToolBarTileSetShowHideGrid;

                    view = new TileSet();
                    break;
                case ProjectItemType.Palette: view = new Palette(); break;
                case ProjectItemType.World: view = new World(); break;
            }

            if (view != null)
            {
                dpItemPanel.Children.Add(view);

                if (view.DataContext is ItemViewModel viewModel)
                {
                    viewModel.ProjectItem = item;

                    viewModel.OnActivate();
                }
            }

            _currentViewType = item.Type;

            dpItemPanel.UpdateLayout();
        }

        private void OnDeleteElement(ProjectItem item)
        {
            if (_currentViewType == item.Type)
            {
                if (dpItemPanel.Children.Count > 0)
                {
                    if (dpItemPanel.Children[0] is UserControl oldGui)
                    {
                        if (oldGui.DataContext is ItemViewModel oldModel)
                        {
                            oldModel.OnDeactivate();
                        }
                    }

                    dpItemPanel.Children.Clear();
                }

                _currentViewType = ProjectItemType.None;

                dpItemPanel.UpdateLayout();
            }
        }

        private void OnShowLoadingDialog()
        {
            _loadingDialog.Owner = this;
            _loadingDialog.Show();
        }

        private void OnHideLoadingDialog()
        {
            _loadingDialog.Hide();
        }

        private void OnCloseProjectSuccess()
        {
            tbrTileSet.Visibility = Visibility.Collapsed;
            tbrMap.Visibility = Visibility.Collapsed;
            tbrBanks.Visibility = Visibility.Collapsed;

            dpItemPanel.Children.Clear();

            _currentViewType = ProjectItemType.None;

            dpItemPanel.UpdateLayout();
        }

        private void EnsureStandardPopupAlignment()
        {
            if (SystemParameters.MenuDropAlignment && _menuDropAlignmentField != null)
            {
                _menuDropAlignmentField.SetValue(null, false);
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        private void MainWindowView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            WindowInteropHelper wih = new WindowInteropHelper(window);
            IntPtr hWnd = wih.Handle;

            const int MONITOR_DEFAULTTOPRIMARY = 1;
            MonitorInfoEx mi = new MonitorInfoEx();
            mi.cbSize = Marshal.SizeOf(mi);
            GetMonitorInfoEx(MonitorFromWindow(hWnd, MONITOR_DEFAULTTOPRIMARY), ref mi);

             GetWindowRect(hWnd, out Rect appBounds);

            double windowHeight = appBounds.Right - appBounds.Left;
            double windowWidth = appBounds.Bottom - appBounds.Top;

            double monitorHeight = mi.rcMonitor.Right - mi.rcMonitor.Left;
            double monitorWidth = mi.rcMonitor.Bottom - mi.rcMonitor.Top;

            bool fullScreen = !((windowHeight == monitorHeight) && (windowWidth == monitorWidth));

            SignalManager.Get<SizeChangedSignal>().Dispatch(e, fullScreen);
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;

            Left = (screenWidth / 2) - (windowWidth / 2);
            Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void OnSetUpWindowProperties(WindowVO vo)
        {
            WindowState = vo.IsFullScreen ? WindowState.Maximized : WindowState.Normal;

            Height = vo.SizeY;
            Width = vo.SizeX;

            CenterWindowOnScreen();
        }

        private void OnUpdateFolder(ProjectItem item)
        {
            TreeViewItem treeItem = (TreeViewItem)(tvProjectItems.ItemContainerGenerator.ContainerFromItem(item));

            if (treeItem != null)
            {
                if (item.Items.Count == 0)
                {
                    treeItem.IsExpanded = false;
                }
            }
            else
            {
                UpdateTreeLayout(item);
            }
        }

        private void OnCreateNewElement(ProjectItem item)
        {
            TreeViewItem parentItem = (TreeViewItem)(tvProjectItems.ItemContainerGenerator.ContainerFromItem(item.Parent));

            if (parentItem != null)
            {
                parentItem.IsExpanded = true;
            }
            else
            {
                UpdateTreeLayout(item);
            }
        }

        private void UpdateTreeLayout(ProjectItem item)
        {
            IEnumerable<ProjectItem> nodes = (IEnumerable<ProjectItem>)tvProjectItems.ItemsSource;
            if (nodes == null)
            {
                return;
            }

            Stack<ProjectItem> queue = new Stack<ProjectItem>();
            queue.Push(item);

            ProjectItem parent = item.Parent;

            while (parent != null)
            {
                queue.Push(parent);
                parent = parent.Parent;
            }

            ItemContainerGenerator generator = tvProjectItems.ItemContainerGenerator;

            while (queue.Count > 0)
            {
                ProjectItem dequeue = queue.Pop();

                tvProjectItems.UpdateLayout();

                TreeViewItem treeViewItem = (TreeViewItem)generator.ContainerFromItem(dequeue);

                bool areThereMoreElement = queue.Count > 0 || item.Items.Count > 0;

                treeViewItem.IsExpanded = areThereMoreElement;

                generator = treeViewItem.ItemContainerGenerator;
            }
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;

            if (stackPanel.DataContext is ProjectItem item && !item.IsLoaded)
            {
                item.IsLoaded = true;
                item.IsSelected = true;

                using (EnableRenameElementCommand command = new EnableRenameElementCommand())
                {
                    command.Execute(item);
                }
            }
        }

        private void EditableTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.IsVisible)
            {
                tb.Focus();
                tb.SelectAll();

                if (tb.DataContext is ProjectItem item)
                {
                    // back up - for possible cancelling
                    item.OldCaptionValue = tb.Text;
                }
            }
        }

        private void EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb.DataContext is ProjectItem item)
            {
                item.IsInEditMode = false;
            }
        }

        private void EditableTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb.DataContext is ProjectItem item)
            {
                if (e.Key == Key.Enter)
                {
                    item.IsInEditMode = false;

                    string name = tb.Text;

                    if (name != item.DisplayName)
                    {
                        // now check if the name is not already taken
                        if (item.IsFolder)
                        {
                            name = ProjectItemFileSystem.GetValidFolderName(item.FileHandler.Path, tb.Text);
                        }
                        else
                        {
                            string extension = Util.GetExtensionByType(item.Type);

                            name = ProjectItemFileSystem.GetValidFileName(item.FileHandler.Path, tb.Text, extension);
                        }

                        tb.Text = name;
                    }
                }

                if (e.Key == Key.Escape)
                {
                    tb.Text = item.OldCaptionValue;

                    item.IsInEditMode = false;
                }
            }
        }

        private void EditableTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;

            string previousText = tb.Text;

            if (tb.SelectedText.Length > 0)
            {
                previousText = tb.Text.Replace(tb.SelectedText, string.Empty);
            }

            e.Handled = !Util.ValidFileName(previousText + e.Text);
        }
    }
}
