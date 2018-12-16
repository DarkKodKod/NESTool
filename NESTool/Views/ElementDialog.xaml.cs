using NESTool.Utils;
using System;
using System.Windows;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for ElementDialog.xaml
    /// </summary>
    public partial class ElementDialog : Window
    {
        public ElementDialog()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowUtility.RemoveIcon(this);
        }

        private void OnClickOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
