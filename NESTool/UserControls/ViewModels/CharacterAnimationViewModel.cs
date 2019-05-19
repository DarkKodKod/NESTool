using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Signals;

namespace NESTool.UserControls.ViewModels
{
    class CharacterAnimationViewModel : ViewModel
    {
        #region Commands
        public PauseCharacterAnimationCommand PauseCharacterAnimationCommand { get; } = new PauseCharacterAnimationCommand();
        public NextFrameCharacterAnimationCommand NextFrameCharacterAnimationCommand { get; } = new NextFrameCharacterAnimationCommand();
        public StopCharacterAnimationCommand StopCharacterAnimationCommand { get; } = new StopCharacterAnimationCommand();
        public PlayCharacterAnimationCommand PlayCharacterAnimationCommand { get; } = new PlayCharacterAnimationCommand();
        public PreviousFrameCharacterAnimationCommand PreviousFrameCharacterAnimationCommand { get; } = new PreviousFrameCharacterAnimationCommand();
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
