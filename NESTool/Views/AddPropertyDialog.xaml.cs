using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.Utils;
using System;
using System.Windows;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for AddPropertyDialog.xaml
    /// </summary>
    public partial class AddPropertyDialog : Window
    {
        public AddPropertyDialog()
        {
            InitializeComponent();
        }

        public void OnActivate()
        {
            txtPropertyName.Focus();

            #region Signals
            SignalManager.Get<AddPropertySignal>().Listener += OnAddProperty;
            #endregion
        }

        public void OnDeactivate()
        {
            txtPropertyName.Focus();

            #region Signals
            SignalManager.Get<AddPropertySignal>().Listener -= OnAddProperty;
            #endregion
        }

        private void OnAddProperty(string property)
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
