using NESTool.Architecture.ViewModel;

namespace NESTool.ViewModels
{
    public class ProjectItem : ViewModel
    {
        public ProjectItem(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; set; }

        private bool _isSelected;
        private bool _isExpanded;
        private object _selectedItem = null;

        private bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");

                    OnExpandedItem();
                }
            }
        }

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

        private void OnExpandedItem()
        {

        }

        private void OnSelectedItemChanged()
        {
            // Raise event / do other things
        }
    }
}
