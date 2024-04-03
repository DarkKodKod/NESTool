using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.VOs;
using System.Collections.Generic;
using System.Linq;

namespace NESTool.UserControls.ViewModels;

public class CharacterFrameEditorViewModel : ViewModel
{
    private FileModelVO[] _banks;
    private int _selectedBank;
    private string _tabId;
    private int _frameIndex;
    private FileHandler _fileHandler;

    #region Commands
    public SwitchCharacterFrameViewCommand SwitchCharacterFrameViewCommand { get; } = new();
    public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new();
    #endregion

    #region get/set
    public FileHandler FileHandler
    {
        get => _fileHandler;
        set
        {
            _fileHandler = value;

            OnPropertyChanged("FileHandler");
        }
    }

    public FileModelVO[] Banks
    {
        get => _banks;
        set
        {
            _banks = value;

            OnPropertyChanged("Banks");
        }
    }

    public int SelectedBank
    {
        get => _selectedBank;
        set
        {
            _selectedBank = value;

            OnPropertyChanged("SelectedBank");
        }
    }

    public string TabID
    {
        get => _tabId;
        set
        {
            _tabId = value;

            OnPropertyChanged("TabID");

            for (int i = 0; i < CharacterModel.Animations.Count; ++i)
            {
                if (CharacterModel.Animations[i].ID == TabID)
                {
                    AnimationIndex = i;
                    break;
                }
            }
        }
    }

    public CharacterModel CharacterModel { get; set; }
    public int AnimationIndex { get; set; }

    public int FrameIndex
    {
        get => _frameIndex;
        set
        {
            _frameIndex = value;

            OnPropertyChanged("FrameIndex");
        }
    }
    #endregion

    public CharacterFrameEditorViewModel()
    {
        UpdateDialogInfo();
    }

    private void UpdateDialogInfo()
    {
        IEnumerable<FileModelVO> banks = ProjectFiles.GetModels<BankModel>().ToArray()
            .Where(p => (p.Model as BankModel).BankUseType == BankUseType.Characters);

        Banks = new FileModelVO[banks.Count()];

        int index = 0;

        foreach (FileModelVO item in banks)
        {
            item.Index = index;

            Banks[index] = item;

            index++;
        }
    }
}
