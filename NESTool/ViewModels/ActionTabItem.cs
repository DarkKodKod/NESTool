using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using NESTool.UserControls.Views;
using System.Windows.Controls;

namespace NESTool.ViewModels;

public class ActionTabItem : ViewModel
{
    private bool _isInEditMode;
    private string _header = string.Empty;
    private UserControl? _content;

    public string ID { get; set; } = string.Empty;
    public UserControl? FramesView { get; set; }
    public UserControl? PixelsView { get; set; }

    public string Header
    {
        get => _header;
        set
        {
            if (_header != value)
            {
                bool changedName = !string.IsNullOrEmpty(_header);

                _header = value;

                OnPropertyChanged("Header");

                if (changedName)
                {
                    SignalManager.Get<RenamedAnimationTabSignal>().Dispatch(value);
                }
            }
        }
    }

    public UserControl? Content
    {
        get => _content;
        set
        {
            _content = value;

            OnPropertyChanged("Content");
        }
    }

    public string OldCaptionValue { get; set; } = "";

    public void SwapContent(string tabId, int frameIndex)
    {
        if (Content != FramesView)
        {
            if (Content is CharacterFrameEditorView characterView)
            {
                CharacterFrameEditorViewModel? currentFrameViewModel = characterView.DataContext as CharacterFrameEditorViewModel;

                currentFrameViewModel?.OnDeactivate();
            }

            Content = FramesView;

            if (Content is CharacterAnimationView animationView)
            {
                animationView.OnActivate();

                CharacterAnimationViewModel? viewModel = animationView.DataContext as CharacterAnimationViewModel;
                viewModel?.OnActivate();
            }
        }
        else
        {
            if (Content is CharacterAnimationView animationView)
            {
                animationView.OnDeactivate();

                CharacterAnimationViewModel? viewModel = animationView.DataContext as CharacterAnimationViewModel;
                viewModel?.OnDeactivate();
            }

            Content = PixelsView;

            if (Content is CharacterFrameEditorView characterView)
            {
                CharacterFrameEditorViewModel? currentFrameViewModel = characterView.DataContext as CharacterFrameEditorViewModel;

                if (currentFrameViewModel != null)
                {
                    currentFrameViewModel.TabID = tabId;
                    currentFrameViewModel.FrameIndex = frameIndex;

                    currentFrameViewModel.OnActivate();
                }
            }
        }
    }

    public bool IsInEditMode
    {
        get => _isInEditMode;
        set
        {
            if (_isInEditMode != value)
            {
                _isInEditMode = value;

                OnPropertyChanged("IsInEditMode");
            }
        }
    }
}
