﻿using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using NESTool.ViewModels;
using NESTool.VOs;
using System.IO;

namespace NESTool.Commands
{
    public class CreateElementFromMenuCommand : Command
    {
        private ProjectItem ItemSeleceted = null;

        public CreateElementFromMenuCommand()
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

            CreateFilesOrDirectory(name, ItemSeleceted.FullPath);
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
