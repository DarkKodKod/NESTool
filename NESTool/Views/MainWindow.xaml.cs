using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using System.Runtime.InteropServices;
using System;
using System.Windows.Interop;
using NESTool.VOs;
using NESTool.ViewModels;
using System.Collections.Generic;
using NESTool.Commands;
using NESTool.Utils;
using NESTool.FileSystem;
using NESTool.Views;
using NESTool.Enums;

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

            // Initialize visibility statte of the toolbars
            tbrTileSet.Visibility = Visibility.Hidden;
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

            dpItemPanel.Children.Clear();

            UserControl view = null;

            tbrTileSet.Visibility = Visibility.Hidden;

            switch (item.Type)
            {
                case ProjectItemType.Bank: view = new Banks(); break;
                case ProjectItemType.Character: view = new Character(); break;
                case ProjectItemType.Map: view = new Map(); break;
                case ProjectItemType.TileSet:
                    tbrTileSet.Visibility = Visibility.Visible;
                    view = new TileSet();
                    break;
                case ProjectItemType.PatternTable: view = new PatternTable(); break;
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
        }

        private void OnDeleteElement(ProjectItem item)
        {
            if (_currentViewType == item.Type)
            {
                dpItemPanel.Children.Clear();

                _currentViewType = ProjectItemType.None;
            }
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

            Rect appBounds = new Rect();
            GetWindowRect(hWnd, out appBounds);

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
                    item.OldValue = tb.Text;
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
                    tb.Text = item.OldValue;

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
