using ArchitectureLibrary.Commands;
using NESTool.Enums;
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

            PaletteIndex paletteIndex = PaletteIndex.Palette0;

            if (values.Length > 2)
            {
                paletteIndex = (PaletteIndex)values[2];
            }

            Window window = Window.GetWindow(userControl);

            ColorPaletteDialog dialog = new ColorPaletteDialog(paletteIndex, colorPosition)
            {
                Owner = window
            };
            dialog.ShowDialog();
        }
    }
}
