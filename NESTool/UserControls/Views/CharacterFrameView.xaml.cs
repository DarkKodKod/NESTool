using NESTool.Commands;
using NESTool.Models;
using NESTool.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for CharacterFrameView.xaml
    /// </summary>
    public partial class CharacterFrameView : UserControl, INotifyPropertyChanged
    {
        private string _tabId;
        private int _frameIndex;
        private ImageSource _frameImage;
        private CharacterModel _characterModel;
        private Dictionary<string, WriteableBitmap> _frameBitmapCache = new Dictionary<string, WriteableBitmap>();

        #region get/set
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
            get { return _tabId; }
            set
            {
                _tabId = value;

                OnPropertyChanged("TabID");
            }
        }

        public FileHandler FileHandler { get; set; }

        public int FrameIndex
        {
            get { return _frameIndex; }
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
        #endregion

        #region Commands
        public SwitchCharacterFrameViewCommand SwitchCharacterFrameViewCommand { get; } = new SwitchCharacterFrameViewCommand();
        public DeleteAnimationFrameCommand DeleteAnimationFrameCommand { get; } = new DeleteAnimationFrameCommand();
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }

        public CharacterFrameView(string tabID, int frameIndex, FileHandler fileHandler, CharacterModel model)
        {
            InitializeComponent();

            TabID = tabID;
            FrameIndex = frameIndex;
            FileHandler = fileHandler;
            CharacterModel = model;

            OnActivate();
        }

        public void OnActivate()
        {
            LoadFrameImage();
        }

        private void LoadFrameImage()
        {
            if (CharacterModel == null)
            {
                return;
            }

            int animationIndex = -1;

            for (int i = 0; i < CharacterModel.Animations.Length; ++i)
            {
                if (CharacterModel.Animations[i].ID == TabID)
                {
                    animationIndex = i;
                    break;
                }
            }

            if (animationIndex == -1 || FrameIndex == -1)
            {
                return;
            }

            WriteableBitmap frameBitmap = CharacterUtils.CreateImage(CharacterModel, animationIndex, FrameIndex, ref _frameBitmapCache);

            if (frameBitmap == null)
            {
                return;
            }

            FrameImage = Util.ConvertWriteableBitmapToBitmapImage(frameBitmap);
        }
    }
}
