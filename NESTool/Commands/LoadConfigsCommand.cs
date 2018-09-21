using NESTool.Architecture.Commands;
using NESTool.Architecture.Model;
using NESTool.Architecture.Signals;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class LoadConfigsCommand : Command
    {
        public override void Execute(object parameter)
        {
            ModelManager.Get<NESToolConfigurationModel>().Load();

            SignalManager.Get<LoadConfigSuccessSignal>().Dispatch();
        }
    }
}
