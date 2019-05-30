using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.UserControls.ViewModels
{
    public class CharacterAnimationViewModel : ViewModel
    {
        private int _speed = 1;
        private string _tabID;
        private CharacterModel _characterModel;
        private FileHandler _fileHandler;

        #region Commands
        public PauseCharacterAnimationCommand PauseCharacterAnimationCommand { get; } = new PauseCharacterAnimationCommand();
        public NextFrameCharacterAnimationCommand NextFrameCharacterAnimationCommand { get; } = new NextFrameCharacterAnimationCommand();
        public StopCharacterAnimationCommand StopCharacterAnimationCommand { get; } = new StopCharacterAnimationCommand();
        public PlayCharacterAnimationCommand PlayCharacterAnimationCommand { get; } = new PlayCharacterAnimationCommand();
        public PreviousFrameCharacterAnimationCommand PreviousFrameCharacterAnimationCommand { get; } = new PreviousFrameCharacterAnimationCommand();
        public NewAnimationFrameCommand NewAnimationFrameCommand { get; } = new NewAnimationFrameCommand();
        #endregion

        #region get/set
        public FileHandler FileHandler
        {
            get { return _fileHandler; }
            set
            {
                _fileHandler = value;

                OnPropertyChanged("FileHandler");
            }
        }

        public CharacterModel CharacterModel
        {
            get { return _characterModel; }
            set
            {
                _characterModel = value;

                OnPropertyChanged("CharacterModel");
            }
        }

        public string TabID
        {
            get { return _tabID; }
            set
            {
                _tabID = value;

                OnPropertyChanged("TabID");
            }
        }

        public int Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;

                OnPropertyChanged("Speed");
            }
        }
        #endregion

        public CharacterAnimationViewModel()
        {
            #region Signals
            SignalManager.Get<PauseCharacterAnimationSignal>().AddListener(OnPauseCharacterAnimation);
            SignalManager.Get<NextFrameCharacterAnimationSignal>().AddListener(OnNextFrameCharacterAnimation);
            SignalManager.Get<StopCharacterAnimationSignal>().AddListener(OnStopCharacterAnimation);
            SignalManager.Get<PlayCharacterAnimationSignal>().AddListener(OnPlayCharacterAnimation);
            SignalManager.Get<PreviousFrameCharacterAnimationSignal>().AddListener(OnPreviousFrameCharacterAnimation);
            #endregion
        }

        public override void OnActivate()
        {
            base.OnActivate();

            for (int i = 0; i < CharacterModel.Animations.Length; ++i)
            {
                CharacterAnimation animation = CharacterModel.Animations[i];

                if (animation.ID == TabID && animation.Frames != null)
                {
                    for (int j = 0; j < animation.Frames.Length; ++j)
                    {
                        Frame frame = animation.Frames[j];

                        if (frame.Tiles != null)
                        {
                            SignalManager.Get<NewAnimationFrameSignal>().Dispatch(TabID);
                        }
                    }
                }
            }
        }

        private void OnPauseCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }
        }

        private void OnNextFrameCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }
        }

        private void OnStopCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }
        }

        private void OnPlayCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }

            // play current animation
        }

        private void OnPreviousFrameCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }
        }
    }
}
