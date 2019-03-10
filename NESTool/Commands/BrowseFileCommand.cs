using NESTool.Signals;
using Microsoft.WindowsAPICodePack.Dialogs;
using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;

namespace NESTool.Commands
{
    public class BrowseFileCommand : Command
    {
        public override void Execute(object parameter)
        {
            object[] values = (object[])parameter;
            string path = (string)values[0];
            string[] filters = null;

            if (values.Length > 1)
            {
                filters = (string[])values[1];
            }

            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Title = "Select File",
                IsFolderPicker = false,
                InitialDirectory = parameter as string,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = parameter as string,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (filters != null && filters.Length > 0)
            {
                for (int i = 0; i < filters.Length; i = i + 2)
                {
                    dialog.Filters.Add(new CommonFileDialogFilter(filters[i], filters[i+1]));
                }
            }

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SignalManager.Get<BrowseFileSuccessSignal>().Dispatch(dialog.FileName);
            }
        }
    }
}
