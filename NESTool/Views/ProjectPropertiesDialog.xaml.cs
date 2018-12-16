using NESTool.Utils;
using System;
using System.Windows;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for ProjectPropertiesDialog.xaml
    /// </summary>
    public partial class ProjectPropertiesDialog : Window
    {
        public ProjectPropertiesDialog()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowUtility.RemoveIcon(this);
        }
    }
}
