using System.ComponentModel;

namespace ArchitectureLibrary.ViewModel;

public class ViewModel : AActivate, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propname)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
    }
}
