using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using System.IO;
using System.Windows;

namespace NESTool.Commands
{
    public class CreateElementFromMenuCommand : ItemSelectedCommand
    {
        private const string _fileNameKey = "NewElementName";

        private readonly string _newFileName;

        public CreateElementFromMenuCommand()
        {
            _newFileName = (string)Application.Current.FindResource(_fileNameKey);
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

            string name = ProjectItemFileSystem.GetValidFileName(
                ItemSeleceted.FullPath, 
                _newFileName, 
                Util.GetExtensionByType(ItemSeleceted.Type));

            ProjectItem newElement = new ProjectItem()
            {
                DisplayName = name,
                FullPath = Path.Combine(ItemSeleceted.FullPath, name),
                IsFolder = false,
                Parent = ItemSeleceted,
                Root = false,
                ParentFolder = ItemSeleceted.FullPath,
                Type = ItemSeleceted.Type
            };

            ItemSeleceted.Items.Add(newElement);

            SignalManager.Get<CreateNewElementSignal>().Dispatch(newElement);

            ProjectItemFileSystem.CreateFileElement(ref newElement);
        }
    }
}
