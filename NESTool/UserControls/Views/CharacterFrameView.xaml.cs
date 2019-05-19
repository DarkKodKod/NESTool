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
        #region Commands
        public OpenCharacterFrameCommand OpenCharacterFrameCommand { get; } = new OpenCharacterFrameCommand();
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }

        public CharacterFrameView()
        {
            InitializeComponent();
        }
    }
}
