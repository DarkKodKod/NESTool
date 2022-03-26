using NESTool.Models;

namespace NESTool.ViewModels
{
    public class EntityViewModel : ItemViewModel
    {
        public EntityModel GetModel()
        {
            return ProjectItem?.FileHandler.FileModel is EntityModel model ? model : null;
        }

        #region Commands
        #endregion

        #region get/set
        #endregion

        public EntityViewModel()
        {
            UpdateDialogInfo();
        }

        public override void OnActivate()
        {
            base.OnActivate();

            #region Signals
            #endregion
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            #endregion
        }

        private void UpdateDialogInfo()
        {
            //
        }
    }
}
