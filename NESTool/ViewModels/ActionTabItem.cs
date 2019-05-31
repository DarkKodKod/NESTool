using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using NESTool.UserControls.Views;
using System.Windows.Controls;

namespace NESTool.ViewModels
{
    public class ActionTabItem : ViewModel
    {
        private bool _isInEditMode;
        private string _header;
        private UserControl _content;
        private CharacterFrameEditorViewModel _currentFrameViewModel;

        public string ID { get; set; }
        public UserControl FramesView { get; set; }
        public UserControl PixelsView { get; set; }

        public string Header
        {
            get
            {
                return _header;
            }
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

        public UserControl Content
        {
            get
            {
                return _content;
            }
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
                if (_currentFrameViewModel != null)
                {
                    _currentFrameViewModel.OnDeactivate();
                }

                Content = FramesView;
            }
            else
            {
                Content = PixelsView;

                CharacterFrameEditorView characterView = _content as CharacterFrameEditorView;

                _currentFrameViewModel = characterView.DataContext as CharacterFrameEditorViewModel;

                _currentFrameViewModel.OnActivate();

                _currentFrameViewModel.TabID = tabId;
                _currentFrameViewModel.FrameIndex = frameIndex;
            }
        }

        public bool IsInEditMode
        {
            get { return _isInEditMode; }
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
}
