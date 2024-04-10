using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using NESTool.Models;

namespace NESTool.Commands;

public class LoadConfigsCommand : Command
{
    public override void Execute(object? parameter)
    {
        ModelManager.Get<NESToolConfigurationModel>().Load();
    }
}
