using NESTool.Models;

namespace NESTool.ViewModels;

public class WorldViewModel : ItemViewModel
{
    public WorldModel GetModel()
    {
        return ProjectItem?.FileHandler.FileModel is WorldModel model ? model : null;
    }

    #region Commands
    #endregion

    #region get/set
    #endregion

    public WorldViewModel()
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
