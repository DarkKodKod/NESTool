using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using System.Windows.Controls;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for CharacterAnimation.xaml
    /// </summary>
    public partial class CharacterAnimationView : UserControl
    {
        public CharacterAnimationView()
        {
            InitializeComponent();

            SignalManager.Get<NewAnimationFrameSignal>().AddListener(OnNewAnimationFrame);
        }

        private void OnNewAnimationFrame(string tabID)
        {
            if (DataContext is CharacterAnimationViewModel viewModel)
            {
                if (!viewModel.IsActive || viewModel.TabID != tabID)
                {
                    return;
                }
            }

            spFrames.Children.Add(new CharacterFrameView());
        }
    }
}
