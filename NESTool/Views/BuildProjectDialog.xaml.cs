using System.Windows;
using System;
using NESTool.Utils;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for BuildProject.xaml
    /// </summary>
    public partial class BuildProjectDialog : Window
    {
        public BuildProjectDialog()
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
