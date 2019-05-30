using NESTool.Commands;
using NESTool.Models;
using System.ComponentModel;
using System.Windows.Controls;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for CharacterFrameView.xaml
    /// </summary>
    public partial class CharacterFrameView : UserControl, INotifyPropertyChanged
    {
        private string _tabId;
        private int _frameIndex;

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

        #region Commands
        public OpenCharacterFrameCommand OpenCharacterFrameCommand { get; } = new OpenCharacterFrameCommand();
        public DeleteAnimationFrameCommand DeleteAnimationFrameCommand { get; } = new DeleteAnimationFrameCommand();
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }

        public CharacterFrameView(string tabID, int frameIndex, FileHandler fileHandler)
        {
            InitializeComponent();

            TabID = tabID;
            FrameIndex = frameIndex;
            FileHandler = fileHandler;
        }
    }
}
