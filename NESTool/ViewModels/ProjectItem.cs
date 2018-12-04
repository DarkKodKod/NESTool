﻿using ArchitectureLibrary.Clipboard;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.CustomTypeConverter;
using NESTool.Enums;
using NESTool.Signals;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace NESTool.ViewModels
{
    [TypeConverter(typeof(ProjectItemTypeConverter))]
    public class ProjectItem : ViewModel, IClipboardable
    {
        public ProjectItem(string displayName, string fullPath, ProjectItemType type)
        {
            Items = new ObservableCollection<ProjectItem>();

            DisplayName = displayName;
            FullPath = fullPath;
            Type = type;
        }

        public ProjectItem(string content)
        {

        }

        public ProjectItemType Type { get; set; }
        public string DisplayName { get; set; }
        public string FullPath { get; set; }
        public bool Root { get; set; }
        public bool IsFolder { get; set; }
        public ProjectItem Parent = null;
        public ObservableCollection<ProjectItem> Items { get; set; }

        virtual public string GetContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Type:" + Type.ToString());
            sb.Append(",");
            sb.Append("DisplayName:" + DisplayName);
            sb.Append(",");
            sb.Append("FullPath:" + FullPath);
            sb.Append(",");
            sb.Append("Items:");

            if (Items.Count == 0)
            {
                sb.Append("null");
            }
            else
            {
                foreach (var item in Items)
                {
                    sb.Append("{");
                    sb.Append(item.GetContent());
                    sb.Append("}");
                }
            }

            return sb.ToString();
        }

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
                    else
                    {
                        _selectedItem = null;
                        OnUnSelectItemChanged();
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

        private void OnUnSelectItemChanged() => SignalManager.Get<ProjectItemUnselectedSignal>().Dispatch(this);
        private void OnSelectedItemChanged() => SignalManager.Get<ProjectItemSelectedSignal>().Dispatch(this);
    }
}
