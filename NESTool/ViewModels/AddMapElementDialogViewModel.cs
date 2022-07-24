using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.VOs;
using System.Collections.Generic;

namespace NESTool.ViewModels
{
    public class AddMapElementDialogViewModel : ViewModel
    {
        private FileModelVO[] _entities;
        private int _selectedEntity = 0;
        private string _selectedEntityId;

        #region Commands
        public AddMapElementCommand AddMapElementCommand { get; } = new AddMapElementCommand();
        #endregion

        public AddMapElementDialogViewModel()
        {
            List<FileModelVO> list = new List<FileModelVO>();

            list.AddRange(ProjectFiles.GetModels<EntityModel>());

            Entities = list.ToArray();

            SelectedEntityId = Entities[SelectedEntity].Model.GUID;
        }

        #region get/set
        public FileModelVO[] Entities
        {
            get => _entities;
            set
            {
                _entities = value;

                OnPropertyChanged("Entities");
            }
        }

        public int SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                _selectedEntity = value;

                SelectedEntityId = Entities[SelectedEntity].Model.GUID;

                OnPropertyChanged("SelectedEntity");
            }
        }

        public string SelectedEntityId
        {
            get => _selectedEntityId;
            set
            {
                _selectedEntityId = value;

                OnPropertyChanged("SelectedEntityId");
            }
        }
        #endregion
    }
}
