using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using System.Windows;

namespace NESTool.Commands;

public class CharacterNewTabCommand : Command
{
    public override void Execute(object parameter)
    {
        MessageBoxResult result = MessageBox.Show("Do you want to create a new animation?", "Create animation", MessageBoxButton.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            SignalManager.Get<AnimationTabNewSignal>().Dispatch();
        }
    }
}
