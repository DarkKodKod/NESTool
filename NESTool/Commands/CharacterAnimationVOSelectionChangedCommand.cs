using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.VOs;
using System.Windows.Controls;

namespace NESTool.Commands;

public class CharacterAnimationVOSelectionChangedCommand : Command
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        SelectionChangedEventArgs? changedEvent = parameter as SelectionChangedEventArgs;

        if (changedEvent?.AddedItems.Count > 0)
        {
            if (changedEvent.AddedItems[0] is CharacterAnimationVO animationVO)
            {
                SignalManager.Get<CharacterAnimationVOSelectionChangedSignal>().Dispatch(animationVO);
            }
        }
    }
}
