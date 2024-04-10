using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using System;
using System.Reflection;

namespace NESTool.ViewModels;

public class AboutDialogViewModel : ViewModel
{
    public OpenLinkCommand OpenLinkCommand { get; } = new();

    public string Version
    {
        get => _version;
        set
        {
            _version = value;
            OnPropertyChanged("Version");
        }
    }

    private string _version = "";

    public AboutDialogViewModel()
    {
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;

        if (version != null)
            Version = version.ToString();
    }
}
