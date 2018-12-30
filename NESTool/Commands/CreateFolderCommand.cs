using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;
using NESTool.ViewModels;
using System.IO;
using System.Windows;

namespace NESTool.Commands
{
    public class CreateFolderCommand : ItemSelectedCommand
    {
        private const string _folderNameKey = "NewFolderName";

        private readonly string _newFolderName;

        public CreateFolderCommand()
        {
            _newFolderName = (string)Application.Current.FindResource(_folderNameKey);
        }

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

            string name = ProjectItemFileSystem.GetValidFolderName(ItemSeleceted.FullPath, _newFolderName);

            ProjectItem newFolder = new ProjectItem()
            {
                DisplayName = name,
                FullPath = Path.Combine(ItemSeleceted.FullPath, name),
                IsFolder = true,
                Parent = ItemSeleceted,
                Root = false,
                ParentFolder = ItemSeleceted.FullPath,
                Type = ItemSeleceted.Type
            };

            ItemSeleceted.Items.Add(newFolder);

            SignalManager.Get<CreateNewElementSignal>().Dispatch(newFolder);

            ProjectItemFileSystem.CreateFileElement(ref newFolder);
        }
    }
}
