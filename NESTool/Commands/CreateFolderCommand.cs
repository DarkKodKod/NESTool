using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.ViewModels;
using System.Collections.ObjectModel;
using System.IO;

namespace NESTool.Commands
{
    public class CreateFolderCommand : Command
    {
        private ProjectItem ItemSeleceted = null;

        public CreateFolderCommand()
        {
            SignalManager.Get<ProjectItemSelectedSignal>().AddListener(OnProjectItemSelected);
            SignalManager.Get<ProjectItemUnselectedSignal>().AddListener(OnProjectItemUnselected);
        }

        public override void Deactivate()
        {
            SignalManager.Get<ProjectItemSelectedSignal>().RemoveListener(OnProjectItemSelected);
            SignalManager.Get<ProjectItemUnselectedSignal>().RemoveListener(OnProjectItemUnselected);
        }

        public override bool CanExecute(object parameter)
        {
            if (ItemSeleceted != null)
            {
                var item = ItemSeleceted as ProjectItem;

                if (item.IsFolder)
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
                Items = new ObservableCollection<ProjectItem>(),
                Parent = ItemSeleceted,
                Root = false,
                Type = ItemSeleceted.Type
            };

            ItemSeleceted.Items.Add(newFolder);

            SignalManager.Get<CreateNewElementSignal>().Dispatch(newFolder);
        }

        private void OnProjectItemSelected(ProjectItem item)
        {
            ItemSeleceted = item;
        }

        private void OnProjectItemUnselected(ProjectItem item)
        {
            ItemSeleceted = null;
        }
    }
}
