using ArchitectureLibrary.ViewModel;
using NESTool.Commands;

namespace NESTool.UserControls.ViewModels
{
    public class CharacterFrameEditorViewModel : ViewModel
    {
        private string _tabId;
        private int _frameIndex;

        #region Commands
        public SwitchCharacterFrameViewCommand SwitchCharacterFrameViewCommand { get; } = new SwitchCharacterFrameViewCommand();
        #endregion

        #region get/set
        public string TabID
        {
            get { return _tabId; }
            set
            {
                _tabId = value;

                OnPropertyChanged("TabID");
            }
        }

        public int FrameIndex
        {
            get { return _frameIndex; }
            set
            {
                _frameIndex = value;

                OnPropertyChanged("FrameIndex");
            }
        }
        #endregion

        public CharacterFrameEditorViewModel()
        {
            #region Signals
            #endregion
        }
    }
}
