using NESTool.Signals;
using Microsoft.WindowsAPICodePack.Dialogs;
using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;

namespace NESTool.Commands
{
    public class BrowseFolderCommand : Command
    {
        public override void Execute(object parameter)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "Select folder";
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = parameter as string;
            dialog.AddToMostRecentlyUsedList = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.DefaultDirectory = parameter as string;
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SignalManager.Get<BrowseFolderSuccessSignal>().Dispatch(dialog.FileName);
            }
        }
    }
}
