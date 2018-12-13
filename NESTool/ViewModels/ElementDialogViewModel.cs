using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Signals;

namespace NESTool.ViewModels
{
    public class ElementDialogViewModel : ViewModel
    {
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();
        public CreateElementCommand CreateElementCommand { get; } = new CreateElementCommand();

        public ElementDialogViewModel()
        {
            SignalManager.Get<CloseDialogSignal>().AddListener(OnCloseDialog);
        }

        private void OnCloseDialog()
        {
            SignalManager.Get<CloseDialogSignal>().RemoveListener(OnCloseDialog);
        }
    }
}
