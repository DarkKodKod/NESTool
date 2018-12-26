using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.ViewModels;
using System.IO;

namespace NESTool.Commands
{
    public class CreateElementCommand : Command
    {
        public override void Execute(object parameter)
        {
            ElementTypeModel element = parameter as ElementTypeModel;

            if (element == null)
            {
                return;
            }

            string name = "New Element";

            ProjectItem newElement = new ProjectItem()
            {
                DisplayName = name,
                FullPath = Path.Combine(element.Path, name),
                IsFolder = false,
                Root = false,
                ParentFolder = element.Path,
                Type = element.Type
            };

            SignalManager.Get<FindAndCreateElementSignal>().Dispatch(newElement);

            ProjectItemFileSystem.CreateFileElement(ref newElement);
        }
    }
}
