using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace NESTool.UserControls.ViewModels;

public class CharacterAnimationViewModel : ViewModel
{
    private float _speed = 0.1f;
    private bool _showCollisionBox = false;
    private string _tabID = string.Empty;
    private CharacterModel? _characterModel;
    private FileHandler? _fileHandler;
    private bool _isPlaying;
    private bool _isPaused;
    private double _rectangleTop = 0.0;
    private double _rectangleLeft = 0.0;
    private ImageSource? _frameImage;
    private int _frameIndex;
    private int _animationIndex = -1;
    private bool _dontSave = false;
    private DispatcherTimer? _dispatcherTimer;
    private int _collisionWidth;
    private int _collisionHeight;
    private int _collisionOffsetX;
    private int _collisionOffsetY;
    private Visibility _rectangleVisibility = Visibility.Hidden;
    private double _rectangleWidth;
    private double _rectangleHeight;
    private readonly Dictionary<int, ImageSource> _bitmapImages = [];

    #region Commands
    public PauseCharacterAnimationCommand PauseCharacterAnimationCommand { get; } = new();
    public NextFrameCharacterAnimationCommand NextFrameCharacterAnimationCommand { get; } = new();
    public StopCharacterAnimationCommand StopCharacterAnimationCommand { get; } = new();
    public PlayCharacterAnimationCommand PlayCharacterAnimationCommand { get; } = new();
    public PreviousFrameCharacterAnimationCommand PreviousFrameCharacterAnimationCommand { get; } = new();
    public NewAnimationFrameCommand NewAnimationFrameCommand { get; } = new();
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

    public ImageSource? FrameImage
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

    public FileHandler? FileHandler
    {
        get { return _fileHandler; }
        set
        {
            _fileHandler = value;

            OnPropertyChanged("FileHandler");
        }
    }

    public CharacterModel? CharacterModel
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
                CharacterModel? model = CharacterModel;

                if (model != null)
                {
                    CollisionInfo colInfo = model.Animations[_animationIndex].CollisionInfo;

                    colInfo.Width = value;

                    RectangleWidth = value;

                    if (!_dontSave)
                    {
                        FileHandler?.Save();
                    }
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
                CharacterModel? model = CharacterModel;

                if (model != null)
                {
                    CollisionInfo colInfo = model.Animations[_animationIndex].CollisionInfo;

                    colInfo.Height = value;

                    RectangleHeight = value;

                    if (!_dontSave)
                    {
                        FileHandler?.Save();
                    }
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
                if (CharacterModel != null)
                {
                    CollisionInfo colInfo = CharacterModel.Animations[_animationIndex].CollisionInfo;

                    colInfo.OffsetX = value;
                }

                RectangleLeft = value;

                if (!_dontSave)
                {
                    FileHandler?.Save();
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
                if (CharacterModel != null)
                {
                    CollisionInfo colInfo = CharacterModel.Animations[_animationIndex].CollisionInfo;

                    colInfo.OffsetY = value;
                }

                RectangleTop = value;

                if (!_dontSave)
                {
                    FileHandler?.Save();
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
                    if (CharacterModel != null)
                    {
                        CharacterModel.Animations[_animationIndex].Speed = value;

                        if (!_dontSave)
                        {
                            FileHandler?.Save();
                        }
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
        SignalManager.Get<PauseCharacterAnimationSignal>().Listener += OnPauseCharacterAnimation;
        SignalManager.Get<NextFrameCharacterAnimationSignal>().Listener += OnNextFrameCharacterAnimation;
        SignalManager.Get<StopCharacterAnimationSignal>().Listener += OnStopCharacterAnimation;
        SignalManager.Get<PlayCharacterAnimationSignal>().Listener += OnPlayCharacterAnimation;
        SignalManager.Get<PreviousFrameCharacterAnimationSignal>().Listener += OnPreviousFrameCharacterAnimation;
        SignalManager.Get<DeleteAnimationFrameSignal>().Listener += OnDeleteAnimationFrame;
        #endregion

        _dontSave = true;

        for (int i = 0; i < CharacterModel?.Animations.Count; ++i)
        {
            CharacterAnimation animation = CharacterModel.Animations[i];

            if (animation.ID == TabID && animation.Frames != null)
            {
                for (int j = 0; j < animation.Frames.Count; ++j)
                {
                    FrameModel frame = animation.Frames[j];

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

        for (int i = 0; i < CharacterModel?.Animations.Count; ++i)
        {
            if (CharacterModel.Animations[i].ID == TabID)
            {
                _animationIndex = i;

                Speed = CharacterModel.Animations[_animationIndex].Speed;

                CollisionInfo cInfo = CharacterModel.Animations[_animationIndex].CollisionInfo;

                CollisionWidth = cInfo.Width;
                CollisionHeight = cInfo.Height;
                CollisionOffsetX = cInfo.OffsetX;
                CollisionOffsetY = cInfo.OffsetY;

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
        SignalManager.Get<PauseCharacterAnimationSignal>().Listener -= OnPauseCharacterAnimation;
        SignalManager.Get<NextFrameCharacterAnimationSignal>().Listener -= OnNextFrameCharacterAnimation;
        SignalManager.Get<StopCharacterAnimationSignal>().Listener -= OnStopCharacterAnimation;
        SignalManager.Get<PlayCharacterAnimationSignal>().Listener -= OnPlayCharacterAnimation;
        SignalManager.Get<PreviousFrameCharacterAnimationSignal>().Listener -= OnPreviousFrameCharacterAnimation;
        SignalManager.Get<DeleteAnimationFrameSignal>().Listener -= OnDeleteAnimationFrame;
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

        if (!_bitmapImages.TryGetValue(FrameIndex, out _))
        {
            ImageVO? vo = CharacterUtils.CreateImage(CharacterModel, _animationIndex, FrameIndex, ref CharacterViewModel.GroupedPalettes);

            if (vo == null || vo.Image == null)
            {
                return;
            }

            _bitmapImages.Add(FrameIndex, vo.Image);
        }

        FrameImage = _bitmapImages[FrameIndex];
    }

    private void OnDeleteAnimationFrame(string tabID, int frameIndex)
    {
        if (TabID != tabID)
        {
            return;
        }

        CharacterModel? model = CharacterModel;

        if (model != null)
        {
            CharacterAnimation? animation = model.Animations.Find(anim => anim.ID == tabID);

            animation?.Frames.RemoveAt(frameIndex);

            if (!_dontSave)
            {
                FileHandler?.Save();
            }
        }
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

        _bitmapImages.Clear();

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

    private void Update(object? sender, object e)
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

        CharacterModel? model = CharacterModel;

        if (model != null)
        {
            if (FrameIndex >= CharacterModel.AnimationSize || FrameIndex >= model.Animations[_animationIndex].Frames.Count)
            {
                FrameIndex = 0;
            }

            LoadFrameImage();
        }
    }

    private void PreviousFrame()
    {
        FrameIndex--;

        CharacterModel? model = CharacterModel;

        if (FrameIndex < 0)
        {
            if (model != null)
            {
                for (int i = model.Animations[_animationIndex].Frames.Count - 1; i >= 0; --i)
                {
                    if (model.Animations[_animationIndex].Frames[i].Tiles != null)
                    {
                        FrameIndex = i;
                        break;
                    }
                }
            }
        }

        LoadFrameImage();
    }
}