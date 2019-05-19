using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Signals;
using System.Windows.Controls;

namespace NESTool.ViewModels
{
    public class ActionTabItem : ViewModel
    {
        private bool _isInEditMode;
        private string _header;

        public string ID { get; set; }

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                if (_header != value)
                {
                    bool changedName = !string.IsNullOrEmpty(_header);

                    _header = value;

                    OnPropertyChanged("Header");

                    if (changedName)
                    {
                        SignalManager.Get<RenamedAnimationTabSignal>().Dispatch(value);
                    }
                }
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
