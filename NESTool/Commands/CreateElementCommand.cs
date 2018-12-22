using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using NESTool.ViewModels;
using NESTool.VOs;
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
                Type = element.Type
            };

            SignalManager.Get<FindAndCreateElementSignal>().Dispatch(newElement);

            CreateFilesOrDirectory(name, element.Path);
        }

        private void CreateFilesOrDirectory(string name, string path)
        {
            var data = new FileHandleVO()
            {
                Name = name,
                Path = path,
                Model = new MetaFileModel()
                {
                    Path = path,
                    Name = name
                }
            };

            SignalManager.Get<CreateFileSignal>().Dispatch(data);
        }
    }
}
