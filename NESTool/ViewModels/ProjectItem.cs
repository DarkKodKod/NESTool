﻿using ArchitectureLibrary.Clipboard;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.CustomTypeConverter;
using NESTool.Enums;
using NESTool.Signals;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        public ProjectItem()
        {
            Items = new ObservableCollection<ProjectItem>();
        }

        public ProjectItem(string content)
        {
            Copy(ParseAndCreateObject(content));
        }

        public ProjectItemType Type { get; set; }
        public string DisplayName { get; set; } = "";
        public string FullPath { get; set; } = "";
        public bool Root { get; set; } = false;
        public bool IsFolder { get; set; } = false;
        public ProjectItem Parent = null;
        public ObservableCollection<ProjectItem> Items { get; set; }

        virtual public string GetContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Type:" + Type.ToString());
            sb.Append(";");
            sb.Append("DisplayName:" + DisplayName);
            sb.Append(";");
            sb.Append("Root:" + (Root ? "true" : "false"));
            sb.Append(";");
            sb.Append("IsFolder:" + (IsFolder ? "true" : "false"));
            sb.Append(";");
            sb.Append("FullPath:" + FullPath);
            sb.Append(";");
            sb.Append("Items:");

            if (Items.Count == 0)
            {
                sb.Append("null");
                sb.Append(";");
            }
            else
            {
                sb.Append(Items.Count.ToString());
                sb.Append(";");
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

        private ProjectItem ParseAndCreateObject(string content)
        {
            ProjectItem item = new ProjectItem();

            int index = 0;
            int found = 0;

            while (index < content.Length)
            {
                found = content.IndexOf(":", index);

                if (found == -1)
                {
                    break;
                }

                string component = content.Substring(index, found - index);

                index = found + 1;

                found = content.IndexOf(";", index);

                string value = content.Substring(index, found - index);

                switch (component)
                {
                    case "Type":
                        item.Type = (ProjectItemType)Enum.Parse(typeof(ProjectItemType), value);
                        break;
                    case "DisplayName":
                        item.DisplayName = value;
                        break;
                    case "Root":
                        item.Root = value == "true";
                        break;
                    case "IsFolder":
                        item.IsFolder = value == "true";
                        break;
                    case "FullPath":
                        item.FullPath = value;
                        break;
                    case "Items":
                        Items = new ObservableCollection<ProjectItem>();

                        if (value != "null")
                        {
                            for (int i = 0; i < int.Parse(value); ++i)
                            {
                                var itm = new ProjectItem();//ParseAndCreateObject("");
                                itm.Parent = item;
                                itm.Type = item.Type;
                                itm.DisplayName = "Placeholder";
                                item.Items.Add(itm);
                            }
                            found = content.Length;
                        }
                        break;
                }

                index = found + 1;
            }

            return item;
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

        private void Copy(ProjectItem item)
        {
            Type = item.Type;
            DisplayName = item.DisplayName;
            FullPath = item.FullPath;
            Root = item.Root;
            IsFolder = item.IsFolder;
            Parent = item.Parent;
            Items = new ObservableCollection<ProjectItem>(item.Items);
        }   

        private void OnUnSelectItemChanged() => SignalManager.Get<ProjectItemUnselectedSignal>().Dispatch(this);
        private void OnSelectedItemChanged() => SignalManager.Get<ProjectItemSelectedSignal>().Dispatch(this);
    }
}