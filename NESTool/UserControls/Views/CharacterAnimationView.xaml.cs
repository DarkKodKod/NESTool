using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using System.Collections.Generic;
using System.Windows.Controls;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for CharacterAnimation.xaml
    /// </summary>
    public partial class CharacterAnimationView : UserControl
    {
        public List<CharacterFrameView> FrameViewList { get; set; } = new List<CharacterFrameView>();

        public CharacterAnimationView()
        {
            InitializeComponent();

            OnActivate();
        }

        public void OnActivate()
        {
            SignalManager.Get<NewAnimationFrameSignal>().Listener += OnNewAnimationFrame;
            SignalManager.Get<DeleteAnimationFrameSignal>().Listener += OnDeleteAnimationFrame;
        }

        public void OnDeactivate()
        {
            SignalManager.Get<NewAnimationFrameSignal>().Listener -= OnNewAnimationFrame;
            SignalManager.Get<DeleteAnimationFrameSignal>().Listener -= OnDeleteAnimationFrame;

            FrameViewList.Clear();
            spFrames.Children.RemoveRange(0, spFrames.Children.Count - 1);
        }

        private void OnDeleteAnimationFrame(string tabID, int frameIndex)
        {
            if (DataContext is CharacterAnimationViewModel viewModel)
            {
                if (viewModel.TabID != tabID)
                {
                    return;
                }
            }

            spFrames.Children.RemoveAt(frameIndex);

            foreach (CharacterFrameView frame in FrameViewList)
            {
                if (frame.FrameIndex == frameIndex)
                {
                    FrameViewList.Remove(frame);
                    break;
                }
            }

            int index = 0;

            // Adjust the index for all the remaining chidren
            foreach (object item in spFrames.Children)
            {
                if (item is CharacterFrameView view)
                {
                    view.FrameIndex = index++;
                }
            }
        }

        private void OnNewAnimationFrame(string tabID)
        {
            if (DataContext is CharacterAnimationViewModel viewModel)
            {
                if (viewModel.TabID != tabID)
                {
                    return;
                }

                CharacterFrameView frame = new CharacterFrameView(tabID, spFrames.Children.Count - 1, viewModel.FileHandler, viewModel.CharacterModel);

                FrameViewList.Add(frame);

                spFrames.Children.Insert(spFrames.Children.Count - 1, frame);
            }
        }
    }
}
