using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for CharacterFrameEditorView.xaml
    /// </summary>
    public partial class CharacterFrameEditorView : UserControl
    {
        public CharacterFrameEditorView()
        {
            InitializeComponent();

            #region Signals
            SignalManager.Get<ColorPaletteControlSelectedSignal>().AddListener(OnColorPaletteControlSelected);
            SignalManager.Get<ColorPaletteCleanupSignal>().AddListener(OnColorPaletteCleanup);
            #endregion
        }

        private void OnColorPaletteCleanup(string animationID, int frameIndex)
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                if (viewModel.TabID != animationID || viewModel.FrameIndex != frameIndex)
                {
                    return;
                }
            }

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

            SignalManager.Get<ColorPaletteCleanupSignal>().RemoveListener(OnColorPaletteCleanup);
        }

        private void OnColorPaletteControlSelected(Color color, int paletteIndex, int colorPosition, string animationID)
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                if (!viewModel.IsActive || viewModel.TabID != animationID)
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
                case 0: palette = palette0; break;
                case 1: palette = palette1; break;
                case 2: palette = palette2; break;
                case 3: palette = palette3; break;
            }

            switch (colorPosition)
            {
                case 0: palette.cvsColor0.Background = scb; break;
                case 1: palette.cvsColor1.Background = scb; break;
                case 2: palette.cvsColor2.Background = scb; break;
                case 3: palette.cvsColor3.Background = scb; break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                palette0.AnimationID = viewModel.TabID;
                palette1.AnimationID = viewModel.TabID;
                palette2.AnimationID = viewModel.TabID;
                palette3.AnimationID = viewModel.TabID;
            }
        }
    }
}
