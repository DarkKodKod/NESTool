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
            object[] values = (object[])parameter;
            UserControl userControl = (UserControl)values[0];
            int colorPosition = (int)values[1];
            int paletteIndex = (int)values[2];
            string animationID = (string)values[3];

            Window window = Window.GetWindow(userControl);

            ColorPaletteDialog dialog = new ColorPaletteDialog(paletteIndex, colorPosition, animationID)
            {
                Owner = window
            };
            dialog.ShowDialog();
        }
    }
}
