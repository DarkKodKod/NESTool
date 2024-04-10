using ArchitectureLibrary.ViewModel;
using NESTool.Commands;

namespace NESTool.ViewModels;

public class AddPropertyDialogViewModel : ViewModel
{
    private string _newProperty = string.Empty;

    #region Commands
    public AddPropertyCommand AddPropertyCommand { get; } = new();
    #endregion

    #region get/set
    public string NewProperty
    {
        get => _newProperty;
        set
        {
            _newProperty = value;

            OnPropertyChanged("NewProperty");
        }
    }
    #endregion
}
