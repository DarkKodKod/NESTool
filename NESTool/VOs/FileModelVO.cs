using NESTool.Models;
using System.ComponentModel;

namespace NESTool.VOs;

public class FileModelVO : INotifyPropertyChanged
{
    private string _name = string.Empty;

    public int Index { get; set; }
    public string Name
    {
        get { return _name; }
        set
        {
            if (_name != value)
            {
                _name = value;

                OnPropertyChanged("Name");
            }
        }
    }
    public AFileModel? Model { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propname)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
    }
}
