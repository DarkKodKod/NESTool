using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.UserControls.Views;
using NESTool.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for Character.xaml
    /// </summary>
    public partial class Character : UserControl, ICleanable
    {
        public Character()
        {
            InitializeComponent();

            actionTabs.ItemsSource = vmCharacterModel.Tabs;

            #region Signals
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener += OnColorPaletteControlSelected;
            #endregion
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

        private void OnColorPaletteControlSelected(Color color, PaletteIndex paletteIndex, int colorPosition)
        {
            if (DataContext is CharacterViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }
            }

            SolidColorBrush scb = new SolidColorBrush
            {
                Color = color
            };

            PaletteView palette = null;

            switch (paletteIndex)
            {
                case PaletteIndex.Palette0: palette = palette0; break;
                case PaletteIndex.Palette1: palette = palette1; break;
                case PaletteIndex.Palette2: palette = palette2; break;
                case PaletteIndex.Palette3: palette = palette3; break;
            }

            switch (colorPosition)
            {
                case 0: palette.cvsColor0.Background = scb; break;
                case 1: palette.cvsColor1.Background = scb; break;
                case 2: palette.cvsColor2.Background = scb; break;
                case 3: palette.cvsColor3.Background = scb; break;
            }
        }

        private void ColorPaletteCleanup()
        {
            Color color = Color.FromRgb(0, 0, 0);
            SolidColorBrush brush = new SolidColorBrush(color);

            void SetColorBack(PaletteView palette)
            {
                palette.cvsColor0.Background = brush;
                palette.cvsColor1.Background = brush;
                palette.cvsColor2.Background = brush;
                palette.cvsColor3.Background = brush;
            }

            SetColorBack(palette0);
            SetColorBack(palette1);
            SetColorBack(palette2);
            SetColorBack(palette3);
        }

        public void CleanUp()
        {
            ColorPaletteCleanup();

            #region Signals
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener -= OnColorPaletteControlSelected;
            #endregion
        }
    }
}
