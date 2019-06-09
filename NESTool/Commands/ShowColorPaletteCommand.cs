using ArchitectureLibrary.Commands;
using NESTool.Views;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.Commands
{
    public class ShowColorPaletteCommand : Command
    {
        public override void Execute(object parameter)
        {
            UserControl userControl = parameter as UserControl;

            Window window = Window.GetWindow(userControl);

            ColorPaletteDialog dialog = new ColorPaletteDialog(0)
            {
                Owner = window
            };
            dialog.ShowDialog();
        }
    }
}
