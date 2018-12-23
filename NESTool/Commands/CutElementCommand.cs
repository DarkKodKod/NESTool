using ArchitectureLibrary.Clipboard;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class CutElementCommand : ItemSelectedCommand
    {
        public override bool CanExecute(object parameter)
        {
            if (ItemSeleceted == null)
            {
                return false;
            }

            if (ItemSeleceted.Root)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            if (ItemSeleceted == null)
            {
                return;
            }

            ClipboardManager.SetData(ItemSeleceted);

            SignalManager.Get<CutElementSignal>().Dispatch(ItemSeleceted);
        }
    }
}
