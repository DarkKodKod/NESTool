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

        #region Commands
        public PauseCharacterAnimationCommand PauseCharacterAnimationCommand { get; } = new PauseCharacterAnimationCommand();
        public NextFrameCharacterAnimationCommand NextFrameCharacterAnimationCommand { get; } = new NextFrameCharacterAnimationCommand();
        public StopCharacterAnimationCommand StopCharacterAnimationCommand { get; } = new StopCharacterAnimationCommand();
        public PlayCharacterAnimationCommand PlayCharacterAnimationCommand { get; } = new PlayCharacterAnimationCommand();
        public PreviousFrameCharacterAnimationCommand PreviousFrameCharacterAnimationCommand { get; } = new PreviousFrameCharacterAnimationCommand();
        public NewAnimationFrameCommand NewAnimationFrameCommand { get; } = new NewAnimationFrameCommand();
        #endregion

        #region get/set
        public CharacterModel CharacterModel { get; set; }

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
            SignalManager.Get<NewAnimationFrameSignal>().AddListener(OnNewAnimationFrame);
            #endregion
        }

        private void OnNewAnimationFrame(string tabID)
        {
            if (!IsActive || TabID != tabID)
            {
                return;
            }
        }

        private void OnPauseCharacterAnimation()
        {
            if (!IsActive)
            {
                return;
            }
        }

        private void OnNextFrameCharacterAnimation()
        {
            if (!IsActive)
            {
                return;
            }
        }

        private void OnStopCharacterAnimation()
        {
            if (!IsActive)
            {
                return;
            }
        }

        private void OnPlayCharacterAnimation()
        {
            if (!IsActive)
            {
                return;
            }
        }

        private void OnPreviousFrameCharacterAnimation()
        {
            if (!IsActive)
            {
                return;
            }
        }
    }
}
