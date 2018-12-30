using NESTool.ViewModels;

namespace NESTool.Commands
{
    public class EnableRenameElementCommand : ItemSelectedCommand
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
            if (parameter is ProjectItem)
            {
                ItemSeleceted = parameter as ProjectItem;
            }

            if (ItemSeleceted == null)
            {
                return;
            }

            ItemSeleceted.IsInEditMode = true;
        }
    }
}
