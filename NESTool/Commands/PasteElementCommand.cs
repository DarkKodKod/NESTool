using ArchitectureLibrary.Clipboard;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.ViewModels;

namespace NESTool.Commands
{
    public class PasteElementCommand : ItemSelectedCommand
    {
        public override bool CanExecute(object parameter)
        {
            if (ClipboardManager.IsEmpty() || ItemSeleceted == null)
            {
                return false;
            }

            // Only is possible the paste element if is in a place with the same type
            ProjectItem clipbr = ClipboardManager.GetData() as ProjectItem;
            if (clipbr != null && clipbr.Type != ItemSeleceted.Type)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            var newItem = ClipboardManager.GetData() as ProjectItem;

            if (newItem != null)
            {
                newItem.IsLoaded = true;

                SignalManager.Get<PasteElementSignal>().Dispatch(ItemSeleceted, newItem);
            }
        }
    }
}
