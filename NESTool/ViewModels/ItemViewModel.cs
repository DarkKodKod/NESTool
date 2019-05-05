using ArchitectureLibrary.ViewModel;

namespace NESTool.ViewModels
{
    public class ItemViewModel : ViewModel
    {
        protected bool IsActive { get; set; } = false;

        public ProjectItem ProjectItem { get; set; }

        public virtual void OnActivate()
        {
            IsActive = true;
        }

        public virtual void OnDeactivate()
        {
            IsActive = false;
        }
    }
}
