using ArchitectureLibrary.ViewModel;

namespace NESTool.ViewModels
{
    public class ItemViewModel : ViewModel
    {
        public ProjectItem ProjectItem { get; set; }

        public virtual void OnActivate() { }
    }
}
