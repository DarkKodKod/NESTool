using NESTool.Architecture.Commands;
using NESTool.ViewModels.ProjectItems;
using System.Windows;

namespace NESTool.Commands
{
    public class DragEnterCommand : Command
    {
        public override void Execute(object parameter)
        {
            var mouseEvent = parameter as DragEventArgs;

            if (!mouseEvent.Data.GetDataPresent(typeof(ProjectItem)))
            {
                mouseEvent.Effects = DragDropEffects.None;
            }
        }
    }
}
