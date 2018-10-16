using ArchitectureLibrary.ViewModel;
using NESTool.Enums;

namespace NESTool.ViewModels
{
    public class ProjectPropertiesDialogViewModel : ViewModel
    {
        public bool Battery
        {
            get { return _battery; }
            set
            {
                _battery = value;
                OnPropertyChanged("Battery");
            }
        }

        public FrameTiming FrameTiming
        {
            get { return _frameTiming; }
            set
            {
                _frameTiming = value;
                OnPropertyChanged("FrameTiming");
            }
        }

        public SpriteSize SpriteSize
        {
            get { return _spriteSize; }
            set
            {
                _spriteSize = value;
                OnPropertyChanged("SpriteSize");
            }
        }

        private bool _battery;
        private FrameTiming _frameTiming;
        private SpriteSize _spriteSize;
    }
}
