using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NESTool.UserControls.ViewModels
{
    public class CharacterAnimationViewModel : ViewModel
    {
        private float _speed = 0.1f;
        private bool _showCollisionBox = false;
        private string _tabID;
        private CharacterModel _characterModel;
        private FileHandler _fileHandler;
        private bool _isPlaying;
        private bool _isPaused;
        private double _rectangleTop = 0.0;
        private double _rectangleLeft = 0.0;
        private ImageSource _frameImage;
        private int _frameIndex;
        private int _animationIndex = -1;
        private bool _dontSave = false;
        private DispatcherTimer _dispatcherTimer;
		private int _collisionWidth;
		private int _collisionHeight;
		private int _collisionOffsetX;
		private int _collisionOffsetY;
        private Visibility _rectangleVisibility = Visibility.Hidden;
        private double _rectangleWidth;
        private double _rectangleHeight;

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

        public double RectangleLeft
        {
            get { return _rectangleLeft; }
            set
            {
                _rectangleLeft = value;

                OnPropertyChanged("RectangleLeft");
            }
        }

        public double RectangleWidth
        {
            get { return _rectangleWidth; }
            set
            {
                _rectangleWidth = value;

                OnPropertyChanged("RectangleWidth");
            }
        }

        public double RectangleHeight
        {
            get { return _rectangleHeight; }
            set
            {
                _rectangleHeight = value;

                OnPropertyChanged("RectangleHeight");
            }
        }

        public double RectangleTop
        {
            get { return _rectangleTop; }
            set
            {
                _rectangleTop = value;

                OnPropertyChanged("RectangleTop");
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

        public int FrameIndex
        {
            get
            {
                return _frameIndex;
            }
            set
            {
                _frameIndex = value;

                OnPropertyChanged("FrameIndex");
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

		public int CollisionWidth
		{
			get { return _collisionWidth; }
			set
			{
				_collisionWidth = value;

				if (_animationIndex != -1)
				{
					if (CharacterModel.Animations[_animationIndex].CollisionInfo == null)
					{
						CharacterModel.Animations[_animationIndex].CollisionInfo = new CollisionInfo();
					}

					CharacterModel.Animations[_animationIndex].CollisionInfo.Width = value;

                    RectangleWidth = value;

                    if (!_dontSave)
                    {
                        FileHandler.Save();
                    }
                }

				OnPropertyChanged("CollisionWidth");
			}
		}

		public int CollisionHeight
		{
			get { return _collisionHeight; }
			set
			{
				_collisionHeight = value;

				if (_animationIndex != -1)
				{
					if (CharacterModel.Animations[_animationIndex].CollisionInfo == null)
					{
						CharacterModel.Animations[_animationIndex].CollisionInfo = new CollisionInfo();
					}

					CharacterModel.Animations[_animationIndex].CollisionInfo.Height = value;

                    RectangleHeight = value;

                    if (!_dontSave)
                    {
                        FileHandler.Save();
                    }
                }

				OnPropertyChanged("CollisionHeight");
			}
		}

		public int CollisionOffsetX
		{
			get { return _collisionOffsetX; }
			set
			{
				_collisionOffsetX = value;

				if (_animationIndex != -1)
				{
					if (CharacterModel.Animations[_animationIndex].CollisionInfo == null)
					{
						CharacterModel.Animations[_animationIndex].CollisionInfo = new CollisionInfo();
					}

					CharacterModel.Animations[_animationIndex].CollisionInfo.OffsetX = value;

                    RectangleLeft = value;

                    if (!_dontSave)
                    {
                        FileHandler.Save();
                    }
                }

				OnPropertyChanged("CollisionOffsetX");
			}
		}

		public int CollisionOffsetY
		{
			get { return _collisionOffsetY; }
			set
			{
				_collisionOffsetY = value;

				if (_animationIndex != -1)
				{
					if (CharacterModel.Animations[_animationIndex].CollisionInfo == null)
					{
						CharacterModel.Animations[_animationIndex].CollisionInfo = new CollisionInfo();
					}

					CharacterModel.Animations[_animationIndex].CollisionInfo.OffsetY = value;

                    RectangleTop = value;

                    if (!_dontSave)
                    {
                        FileHandler.Save();
                    }
                }

				OnPropertyChanged("CollisionOffsetY");
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

                        if (!_dontSave)
                        {
                            FileHandler.Save();
                        }
                    }

                    OnPropertyChanged("Speed");
                }
            }
        }

        public Visibility RectangleVisibility
        {
            get { return _rectangleVisibility; }
            set
            {
                _rectangleVisibility = value;

                OnPropertyChanged("RectangleVisibility");
            }
        }

        public bool ShowCollisionBox
        {
            get { return _showCollisionBox; }
            set
            {
                if (_showCollisionBox != value)
                {
                    _showCollisionBox = value;

                    RectangleVisibility = value == true ? Visibility.Visible : Visibility.Hidden;

                    OnPropertyChanged("ShowCollisionBox");
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

            _dontSave = true;

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

            FrameIndex = 0;
            _animationIndex = -1;

            for (int i = 0; i < CharacterModel.Animations.Length; ++i)
            {
                if (CharacterModel.Animations[i].ID == TabID)
                {
                    _animationIndex = i;

                    Speed = CharacterModel.Animations[_animationIndex].Speed;

					CollisionInfo cInfo = CharacterModel.Animations[_animationIndex].CollisionInfo;

					CollisionWidth = cInfo == null ? 0 : cInfo.Width;
					CollisionHeight = cInfo == null ? 0 : cInfo.Height;
					CollisionOffsetX = cInfo == null ? 0 : cInfo.OffsetX;
					CollisionOffsetY = cInfo == null ? 0 : cInfo.OffsetY;

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

            _dontSave = false;
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

        public void LoadFrameImage()
        {
            if (CharacterModel == null || _animationIndex == -1)
            {
                return;
            }

            WriteableBitmap frameBitmap = CharacterUtils.CreateImage(CharacterModel, _animationIndex, FrameIndex);

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
            FrameIndex++;

            if (FrameIndex >= 64 ||
                CharacterModel.Animations[_animationIndex].Frames[FrameIndex].Tiles == null)
            {
                FrameIndex = 0;
            }

            LoadFrameImage();
        }

        private void PreviousFrame()
        {
            FrameIndex--;

            if (FrameIndex < 0)
            {
                for (int i = CharacterModel.Animations[_animationIndex].Frames.Length - 1; i >= 0; --i)
                {
                    if (CharacterModel.Animations[_animationIndex].Frames[i].Tiles != null)
                    {
                        FrameIndex = i;
                        break;
                    }
                }
            }

            LoadFrameImage();
        }
    }
}