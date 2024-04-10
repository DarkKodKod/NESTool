using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.VOs;
using System.Collections.Generic;

namespace NESTool.ViewModels;

public class AddMapElementDialogViewModel : ViewModel
{
    private List<FileModelVO> _entities = [];
    private int _selectedEntity = 0;
    private string _selectedEntityId = string.Empty;

    #region Commands
    public AddMapElementCommand AddMapElementCommand { get; } = new();
    #endregion

    public AddMapElementDialogViewModel()
    {
        Entities.AddRange(ProjectFiles.GetModels<EntityModel>());

        if (Entities.Count > 0)
        {
            AFileModel? fileModel = Entities[SelectedEntity].Model;

            if (fileModel != null)
                SelectedEntityId = fileModel.GUID;
        }
    }

    #region get/set
    public List<FileModelVO> Entities
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

            AFileModel? fileModel = Entities[SelectedEntity].Model;

            if (fileModel != null)
            {
                SelectedEntityId = fileModel.GUID;
            }

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
