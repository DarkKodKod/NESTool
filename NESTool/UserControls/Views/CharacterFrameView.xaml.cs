using NESTool.Commands;
using System.ComponentModel;
using System.Windows.Controls;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for CharacterFrameView.xaml
    /// </summary>
    public partial class CharacterFrameView : UserControl, INotifyPropertyChanged
    {
        public string TabID { get; set; }

        public int FrameIndex { get; set; }

        #region Commands
        public OpenCharacterFrameCommand OpenCharacterFrameCommand { get; } = new OpenCharacterFrameCommand();
        public DeleteAnimationFrameCommand DeleteAnimationFrameCommand { get; } = new DeleteAnimationFrameCommand();
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }

        public CharacterFrameView(string tabID, int frameIndex)
        {
            InitializeComponent();

            TabID = tabID;
            FrameIndex = frameIndex;
        }
    }
}
