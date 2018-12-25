using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;
using NESTool.ViewModels;
using System.IO;

namespace NESTool.Commands
{
    public class CreateElementFromMenuCommand : ItemSelectedCommand
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

            string name = "New Element";

            ProjectItem newElement = new ProjectItem()
            {
                DisplayName = name,
                FullPath = Path.Combine(ItemSeleceted.FullPath, name),
                IsFolder = false,
                Parent = ItemSeleceted,
                Root = false,
                Type = ItemSeleceted.Type
            };

            ItemSeleceted.Items.Add(newElement);

            SignalManager.Get<CreateNewElementSignal>().Dispatch(newElement);

            ProjectItemFileSystem.CreateFile(name, ItemSeleceted.FullPath, ItemSeleceted.Type);
        }
    }
}
