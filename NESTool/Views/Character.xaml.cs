using NESTool.ViewModels;
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

        private void EditableTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.IsVisible)
            {
                if (tb.DataContext is ActionTabItem item)
                {
                    // back up - for possible cancelling
                    item.OldCaptionValue = tb.Text;
                }
            }
        }

        private void EditableTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb.DataContext is ActionTabItem item)
            {
                if (e.Key == Key.Enter)
                {
                    item.IsInEditMode = false;

                    string name = tb.Text;

                    if (name != item.Header)
                    {
                        tb.Text = name;
                    }
                }

                if (e.Key == Key.Escape)
                {
                    tb.Text = item.OldCaptionValue;

                    item.IsInEditMode = false;
                }
            }
        }
        
        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContentControl tb = sender as ContentControl;

            if (tb.DataContext is ActionTabItem item)
            {
                item.IsInEditMode = true;
            }
        }
    }
}
