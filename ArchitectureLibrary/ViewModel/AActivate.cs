namespace ArchitectureLibrary.ViewModel
{
    public abstract class AActivate
    {
        protected bool IsActive { get; set; } = false;

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
