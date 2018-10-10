using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Enums;
using NESTool.Signals;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace NESTool.ViewModels.ProjectItems
{
    public class ProjectItem : ViewModel
    {
        public ProjectItem(string displayName, string fullPath, ProjectItemType type)
        {
            Items = new ObservableCollection<ProjectItem>();

            DisplayName = displayName;
            FullPath = fullPath;
            Type = type;
        }

        public ProjectItemType Type { get; set; }
        public string DisplayName { get; set; }
        public string FullPath { get; set; }
        public ObservableCollection<ProjectItem> Items { get; set; }

        private bool _isSelected;
        private object _selectedItem = null;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");

                    if (_isSelected)
                    {
                        SelectedItem = this;
                    }
                }
            }
        }

        public object SelectedItem
        {
            get { return _selectedItem; }
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged();
                }
            }
        }

        public DataTemplate GetHeaderTemplate()
        {
            //create the data template
            DataTemplate dataTemplate = new DataTemplate();

            //create stack pane;
            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            stackPanel.SetValue(StackPanel.BackgroundProperty, new SolidColorBrush(Colors.Black));

            // create text
            FrameworkElementFactory label = new FrameworkElementFactory(typeof(TextBlock));
            label.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("DisplayName") });
            label.SetValue(TextBlock.MarginProperty, new Thickness(2));
            label.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            label.SetValue(TextBlock.ToolTipProperty, new Binding());
            label.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.White));
            stackPanel.AppendChild(label);

            //set the visual tree of the data template
            dataTemplate.VisualTree = stackPanel;

            return dataTemplate;
        }

        private void OnSelectedItemChanged() => SignalManager.Get<ProjectItemSelectedSignal>().Dispatch(this);
    }
}
