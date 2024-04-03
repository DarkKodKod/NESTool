using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using Microsoft.WindowsAPICodePack.Dialogs;
using NESTool.Signals;
using System.Runtime.Versioning;

namespace NESTool.Commands;

[SupportedOSPlatform("windows")]
public class BrowseFolderCommand : Command
{
    public override void Execute(object parameter)
    {
        CommonOpenFileDialog dialog = new()
        {
            Title = "Select folder",
            IsFolderPicker = true,
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

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            SignalManager.Get<BrowseFolderSuccessSignal>().Dispatch(dialog.FileName);
        }
    }
}
