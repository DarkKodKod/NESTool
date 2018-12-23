using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;
using NESTool.ViewModels;
using System.IO;

namespace NESTool.Commands
{
    public class CreateFolderCommand : ItemSelectedCommand
    {
        public override bool CanExecute(object parameter)
        {
            if (ItemSeleceted != null)
            {
                if (ItemSeleceted.IsFolder)
                {
                    return true;
                }
            }

            return false;
        }

        public override void Execute(object parameter)
        {
            if (ItemSeleceted == null)
            {
                return;
            }

            string name = "New folder";

            ProjectItem newFolder = new ProjectItem()
            {
                DisplayName = name,
                FullPath = Path.Combine(ItemSeleceted.FullPath, name),
                IsFolder = true,
                Parent = ItemSeleceted,
                Root = false,
                Type = ItemSeleceted.Type
            };

            ItemSeleceted.Items.Add(newFolder);

            SignalManager.Get<CreateNewElementSignal>().Dispatch(newFolder);

            FileSystemManager.CreateFolder(name, ItemSeleceted.FullPath);
        }
    }
}
