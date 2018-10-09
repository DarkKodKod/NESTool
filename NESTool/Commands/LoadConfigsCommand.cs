using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
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
