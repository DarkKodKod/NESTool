using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using System;
using System.Windows;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for AddMapElementDialog.xaml
    /// </summary>
    public partial class AddMapElementDialog : Window
    {
        public AddMapElementDialog()
        {
            InitializeComponent();
        }

        public void OnActivate()
        {
            #region Signals
            SignalManager.Get<AddMapElementSignal>().Listener += OnAddMapElement;
            #endregion
        }

        public void OnDeactivate()
        {
            #region Signals
            SignalManager.Get<AddMapElementSignal>().Listener -= OnAddMapElement;
            #endregion
        }

        private void OnAddMapElement(string mapElement)
        {
            DialogResult = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowUtility.RemoveIcon(this);
        }
    }
}
