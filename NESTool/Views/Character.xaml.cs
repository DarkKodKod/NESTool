using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for Character.xaml
    /// </summary>
    public partial class Character : UserControl
    {
        public Character()
        {
            InitializeComponent();

            actionTabs.ItemsSource = vmCharacterModel.Tabs;
        }

        private void ActionTabs_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseButtonEventArgs args = e as MouseButtonEventArgs;

            FrameworkElement source = (FrameworkElement)args.OriginalSource;

            if (source.DataContext.ToString() == "{NewItemPlaceholder}")
            {
                e.Handled = true;
            }
        }
    }
}
