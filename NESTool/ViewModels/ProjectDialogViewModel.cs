using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Controls;

namespace NESTool.ViewModels;

[SupportedOSPlatform("windows")]
public class ProjectDialogViewModel : ViewModel
{
    public CreateProjectCommand CreateProjectCommand { get; } = new();
    public BrowseFolderCommand BrowseFolderCommand { get; } = new();
    public DispatchSignalCommand<CloseDialogSignal> CloseDialogCommand { get; } = new();

    #region get/set
    public string ProjectName
    {
        get => _projectName;
        set
        {
            if (Util.ValidFileName(value))
            {
                _projectName = value;
                _previousValidName = value;
                OnPropertyChanged("ProjectName");
            }
            else
            {
                ProjectName = _previousValidName;
            }
        }
    }

    public string FolderPath
    {
        get => _folderPath;
        set
        {
            _folderPath = value;
            OnPropertyChanged("FolderPath");
        }
    }

    public List<MapperModel> Mappers
    {
        get => _mappers;
        set
        {
            _mappers = value;
            OnPropertyChanged("Mappers");
        }
    }

    public int SelectedMapper
    {
        get => _selectedMapper;
        set
        {
            _selectedMapper = value;
            OnPropertyChanged("SelectedMapper");
        }
    }

    public MirroringType[]? Mirrorings
    {
        get => _mirrorings;
        set
        {
            _mirrorings = value;
            OnPropertyChanged("Mirrorings");
        }
    }

    public int[]? CHRSizes
    {
        get => _chrSizes;
        set
        {
            _chrSizes = value;
            OnPropertyChanged("CHRSizes");
        }
    }

    public int SelectedCHRSize
    {
        get => _selectedCHRSize;
        set
        {
            _selectedCHRSize = value;
            OnPropertyChanged("SelectedCHRSize");
        }
    }

    public int[]? PRGSizes
    {
        get => _prgSizes;
        set
        {
            _prgSizes = value;
            OnPropertyChanged("PRGSizes");
        }
    }

    public int SelectedPRGSize
    {
        get => _selectedPRGSize;
        set
        {
            _selectedPRGSize = value;
            OnPropertyChanged("SelectedPRGSize");
        }
    }
    #endregion

    private string _previousValidName = string.Empty;
    private string _projectName = string.Empty;
    private string _folderPath = string.Empty;
    private int[]? _chrSizes;
    private int[]? _prgSizes;
    private MirroringType[]? _mirrorings;
    private List<MapperModel> _mappers = [];
    private int _selectedMapper;
    private int _selectedCHRSize;
    private int _selectedPRGSize;

    public ProjectDialogViewModel()
    {
        MappersModel mappers = ModelManager.Get<MappersModel>();

        Mappers = mappers.Mappers;

        if (mappers.Mappers.Count > 0)
        {
            SelectedMapper = mappers.Mappers[0].Id;
            CHRSizes = mappers.Mappers[0].CHR;
            Mirrorings = mappers.Mappers[0].Mirroring;
            PRGSizes = mappers.Mappers[0].PRG;
        }

        SelectedPRGSize = 0;
        SelectedCHRSize = 0;

        SignalManager.Get<BrowseFolderSuccessSignal>().Listener += BrowseFolderSuccess;
        SignalManager.Get<CloseDialogSignal>().Listener += OnCloseDialog;
    }

    private void OnCloseDialog()
    {
        SignalManager.Get<BrowseFolderSuccessSignal>().Listener -= BrowseFolderSuccess;
        SignalManager.Get<CloseDialogSignal>().Listener -= OnCloseDialog;
    }

    private void BrowseFolderSuccess(Control owner, string folderPath) => FolderPath = folderPath;
}
