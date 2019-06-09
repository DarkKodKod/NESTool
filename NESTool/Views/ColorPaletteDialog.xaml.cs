using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Signals;
using NESTool.Utils;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for ColorPalette.xaml
    /// </summary>
    public partial class ColorPaletteDialog : Window, INotifyPropertyChanged
    {
        #region Commands
        public ColorPaletteSelectCommand ColorPaletteSelectCommand { get; } = new ColorPaletteSelectCommand();
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();
        #endregion

        public ColorPaletteDialog(int paletteIndex)
        {
            InitializeComponent();

            SignalManager.Get<ColorPalleteSelectSignal>().AddListener(OnColorPalleteSelect);
            SignalManager.Get<CloseDialogSignal>().AddListener(OnCloseDialog);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }

        private void OnColorPalleteSelect(Color color)
        {
            //

            DialogResult = true;
        }

        private void OnCloseDialog()
        {
            SignalManager.Get<ColorPalleteSelectSignal>().RemoveListener(OnColorPalleteSelect);
            SignalManager.Get<CloseDialogSignal>().RemoveListener(OnCloseDialog);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowUtility.RemoveIcon(this);
        }
    }
}
