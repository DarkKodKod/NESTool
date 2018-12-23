using ArchitectureLibrary.Clipboard;

namespace NESTool.Commands
{
    public class CopyElementCommand : ItemSelectedCommand
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
            ClipboardManager.SetData(ItemSeleceted);
        }
    }
}
