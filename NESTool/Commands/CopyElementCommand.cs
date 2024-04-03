using ArchitectureLibrary.Clipboard;

namespace NESTool.Commands;

public class CopyElementCommand : ItemSelectedCommand
{
    public override bool CanExecute(object parameter)
    {
        if (ItemSelected == null)
        {
            return false;
        }

        if (ItemSelected.IsRoot)
        {
            return false;
        }

        return true;
    }

    public override void Execute(object parameter)
    {
        ClipboardManager.SetData(ItemSelected);
    }
}
