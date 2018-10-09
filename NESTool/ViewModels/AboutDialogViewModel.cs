using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using System.Reflection;

namespace NESTool.ViewModels
{
    public class AboutDialogViewModel : ViewModel
    {
        public OpenLinkCommand OpenLinkCommand { get; } = new OpenLinkCommand();

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
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
