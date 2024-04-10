using NESTool.ViewModels;

namespace NESTool.Commands;

public class EnableRenameElementCommand : ItemSelectedCommand
{
    public override bool CanExecute(object? parameter)
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

    public override void Execute(object? parameter)
    {
        if (parameter is ProjectItem)
        {
            ItemSelected = parameter as ProjectItem;
        }

        if (ItemSelected == null)
        {
            return;
        }

        ItemSelected.IsInEditMode = true;
    }
}
