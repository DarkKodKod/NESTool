using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace NESTool.Commands;

public class CharacterCloseTabCommand : Command
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the animation tab?", "Delete", MessageBoxButton.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            MouseButtonEventArgs? args = parameter as MouseButtonEventArgs;

            FrameworkElement? source = (FrameworkElement?)args?.OriginalSource;

            if (source?.DataContext is ActionTabItem tabItem)
                SignalManager.Get<AnimationTabDeletedSignal>().Dispatch(tabItem);
        }
    }
}
