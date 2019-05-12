using ArchitectureLibrary.ViewModel;
using System.Windows.Controls;

namespace NESTool.ViewModels
{
    public class ActionTabItem : ViewModel
    {
        private bool _isInEditMode;
        private string _header;

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;

                OnPropertyChanged("Header");
            }
        }
        public UserControl Content { get; set; }
        public string OldCaptionValue { get; set; } = "";

        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set
            {
                if (_isInEditMode != value)
                {
                    _isInEditMode = value;

                    OnPropertyChanged("IsInEditMode");
                }
            }
        }
    }
}
