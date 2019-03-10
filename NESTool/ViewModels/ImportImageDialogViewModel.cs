using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Signals;

namespace NESTool.ViewModels
{
    public class ImportImageDialogViewModel : ViewModel
    {
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();

        public ImportImageDialogViewModel()
        {
            SignalManager.Get<CloseDialogSignal>().AddListener(OnCloseDialog);

            InitializeElements();
        }

        private void OnCloseDialog()
        {
            SignalManager.Get<CloseDialogSignal>().RemoveListener(OnCloseDialog);
        }

        private void InitializeElements()
        {
        }
    }
}
