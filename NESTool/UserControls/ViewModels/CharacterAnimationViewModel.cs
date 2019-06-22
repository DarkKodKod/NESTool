using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NESTool.UserControls.ViewModels
{
    public class CharacterAnimationViewModel : ViewModel
    {
        private float _speed = 0.1f;
        private string _tabID;
        private CharacterModel _characterModel;
        private FileHandler _fileHandler;
        private bool _isPlaying;
        private bool _isPaused;
        private ImageSource _frameImage;
        private int _frameIndex;
        private int _animationIndex = -1;
        private Dictionary<string, WriteableBitmap> _frameBitmapCache = new Dictionary<string, WriteableBitmap>();
        private DispatcherTimer _dispatcherTimer;

        #region Commands
        public PauseCharacterAnimationCommand PauseCharacterAnimationCommand { get; } = new PauseCharacterAnimationCommand();
        public NextFrameCharacterAnimationCommand NextFrameCharacterAnimationCommand { get; } = new NextFrameCharacterAnimationCommand();
        public StopCharacterAnimationCommand StopCharacterAnimationCommand { get; } = new StopCharacterAnimationCommand();
        public PlayCharacterAnimationCommand PlayCharacterAnimationCommand { get; } = new PlayCharacterAnimationCommand();
        public PreviousFrameCharacterAnimationCommand PreviousFrameCharacterAnimationCommand { get; } = new PreviousFrameCharacterAnimationCommand();
        public NewAnimationFrameCommand NewAnimationFrameCommand { get; } = new NewAnimationFrameCommand();
        #endregion

        #region get/set
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;

                OnPropertyChanged("IsPlaying");
            }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                _isPaused = value;

                OnPropertyChanged("IsPaused");
            }
        }

        public ImageSource FrameImage
        {
            get
            {
                return _frameImage;
            }
            set
            {
                _frameImage = value;

                OnPropertyChanged("FrameImage");
            }
        }

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

        public float Speed
        {
            get { return _speed; }
            set
            {
                if (_speed != value)
                {
                    _speed = value;

                    if (_dispatcherTimer != null)
                    {
                        _dispatcherTimer.Interval = TimeSpan.FromSeconds(Speed);
                    }

                    if (_animationIndex != -1)
                    {
                        CharacterModel.Animations[_animationIndex].Speed = value;

                        FileHandler.Save();
                    }

                    OnPropertyChanged("Speed");
                }
            }
        }
        #endregion
        
        public override void OnActivate()
        {
            base.OnActivate();

            #region Signals
            SignalManager.Get<PauseCharacterAnimationSignal>().AddListener(OnPauseCharacterAnimation);
            SignalManager.Get<NextFrameCharacterAnimationSignal>().AddListener(OnNextFrameCharacterAnimation);
            SignalManager.Get<StopCharacterAnimationSignal>().AddListener(OnStopCharacterAnimation);
            SignalManager.Get<PlayCharacterAnimationSignal>().AddListener(OnPlayCharacterAnimation);
            SignalManager.Get<PreviousFrameCharacterAnimationSignal>().AddListener(OnPreviousFrameCharacterAnimation);
            #endregion

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

            IsPlaying = false;

            _frameIndex = 0;
            _animationIndex = -1;

            for (int i = 0; i < CharacterModel.Animations.Length; ++i)
            {
                if (CharacterModel.Animations[i].ID == TabID)
                {
                    _animationIndex = i;

                    Speed = CharacterModel.Animations[_animationIndex].Speed;

                    break;
                }
            }

            LoadFrameImage();

            _dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(Speed)
            };
            _dispatcherTimer.Tick += Update;
            _dispatcherTimer.Start();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<PauseCharacterAnimationSignal>().RemoveListener(OnPauseCharacterAnimation);
            SignalManager.Get<NextFrameCharacterAnimationSignal>().RemoveListener(OnNextFrameCharacterAnimation);
            SignalManager.Get<StopCharacterAnimationSignal>().RemoveListener(OnStopCharacterAnimation);
            SignalManager.Get<PlayCharacterAnimationSignal>().RemoveListener(OnPlayCharacterAnimation);
            SignalManager.Get<PreviousFrameCharacterAnimationSignal>().RemoveListener(OnPreviousFrameCharacterAnimation);
            #endregion

            IsPlaying = false;
            IsPaused = false;

            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Stop();
            }
        }

        private void LoadFrameImage()
        {
            if (CharacterModel == null || _animationIndex == -1)
            {
                return;
            }

            WriteableBitmap frameBitmap = CharacterUtils.CreateImage(CharacterModel, _animationIndex, _frameIndex, ref _frameBitmapCache);

            if (frameBitmap == null)
            {
                return;
            }

            FrameImage = Util.ConvertWriteableBitmapToBitmapImage(frameBitmap);
        }

        private void OnPauseCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }

            if (IsPaused)
            {
                return;
            }

            IsPaused = true;
            IsPlaying = false;
        }

        private void OnNextFrameCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }

            IsPaused = true;
            IsPlaying = false;

            NextFrame();
        }

        private void OnStopCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }

            IsPlaying = false;
            IsPaused = false;
        }

        private void OnPlayCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }

            if (IsPlaying)
            {
                return;
            }

            IsPlaying = true;
            IsPaused = false;
        }

        private void Update(object sender, object e)
        {
            if (IsPlaying)
            {
                NextFrame();
            }
        }

        private void OnPreviousFrameCharacterAnimation(string tabId)
        {
            if (!IsActive || TabID != tabId)
            {
                return;
            }

            IsPlaying = false;
            IsPaused = true;

            PreviousFrame();
        }

        private void NextFrame()
        {
            _frameIndex++;

            if (_frameIndex >= 64 ||
                CharacterModel.Animations[_animationIndex].Frames[_frameIndex].Tiles == null)
            {
                _frameIndex = 0;
            }

            LoadFrameImage();
        }

        private void PreviousFrame()
        {
            _frameIndex--;

            if (_frameIndex < 0)
            {
                for (int i = CharacterModel.Animations[_animationIndex].Frames.Length - 1; i >= 0; --i)
                {
                    if (CharacterModel.Animations[_animationIndex].Frames[i].Tiles != null)
                    {
                        _frameIndex = i;
                        break;
                    }
                }
            }

            LoadFrameImage();
        }
    }
}