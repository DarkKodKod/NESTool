using ArchitectureLibrary.Commands;
using ArchitectureLibrary.History.Signals;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.HistoryActions;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using System.Windows;

namespace NESTool.Commands
{
    public class CreateElementCommand : Command
    {
        private const string _fileNameKey = "NewElementName";

        private readonly string _newFileName;

        public CreateElementCommand()
        {
            _newFileName = (string)Application.Current.FindResource(_fileNameKey);
        }

        public override void Execute(object parameter)
        {
            ElementTypeModel element = parameter as ElementTypeModel;

            if (element == null)
            {
                return;
            }

            string name = ProjectItemFileSystem.GetValidFileName(
                element.Path,
                _newFileName,
                Util.GetExtensionByType(element.Type));

            ProjectItem newElement = new ProjectItem()
            {
                DisplayName = name,
                IsFolder = false,
                IsRoot = false,
                Type = element.Type
            };

            SignalManager.Get<RegisterHistoryActionSignal>().Dispatch(new CreateNewElementHistoryAction(newElement));

            SignalManager.Get<FindAndCreateElementSignal>().Dispatch(newElement);

            ProjectItemFileSystem.CreateFileElement(newElement, element.Path, name);
        }
    }
}
