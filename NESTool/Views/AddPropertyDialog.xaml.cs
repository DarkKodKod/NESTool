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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowUtility.RemoveIcon(this);
        }

        private void OnClickAdd(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
