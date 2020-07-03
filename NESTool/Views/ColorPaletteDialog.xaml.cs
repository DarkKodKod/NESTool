﻿using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
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
        private readonly PaletteIndex _paletteIndex;
        private readonly int _colorPosition;

        #region Commands
        public ColorPaletteSelectCommand ColorPaletteSelectCommand { get; } = new ColorPaletteSelectCommand();
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();
        #endregion

        public ColorPaletteDialog(PaletteIndex paletteIndex, int colorPosition)
        {
            InitializeComponent();

            _paletteIndex = paletteIndex;
            _colorPosition = colorPosition;

            SignalManager.Get<ColorPaletteSelectSignal>().AddListener(OnColorPaletteSelect);
            SignalManager.Get<CloseDialogSignal>().AddListener(OnCloseDialog);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }

        private void OnColorPaletteSelect(Color color)
        {
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Dispatch(color, _paletteIndex, _colorPosition);

            DialogResult = true;
        }

        private void OnCloseDialog()
        {
            SignalManager.Get<ColorPaletteSelectSignal>().RemoveListener(OnColorPaletteSelect);
            SignalManager.Get<CloseDialogSignal>().RemoveListener(OnCloseDialog);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowUtility.RemoveIcon(this);
        }
    }
}
