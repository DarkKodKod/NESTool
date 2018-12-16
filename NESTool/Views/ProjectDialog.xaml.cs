using System.Windows;
using System;
using NESTool.Utils;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for ProjectDialog.xaml
    /// </summary>
    public partial class ProjectDialog : Window
    {
        public ProjectDialog()
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
