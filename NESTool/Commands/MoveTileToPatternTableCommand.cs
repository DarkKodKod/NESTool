using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NESTool.Commands
{
    public class MoveTileToPatternTableCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            object[] values = (object[])parameter;
            WriteableBitmap cropperImage = (WriteableBitmap)values[0];
            PatternTableModel model = (PatternTableModel)values[3];

            if (cropperImage == null || model.IsFull())
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            object[] values = (object[])parameter;

            WriteableBitmap cropperImage = (WriteableBitmap)values[0];
            Point croppedPoint = (Point)values[1];
            BitmapSource imgSource = (BitmapSource)values[2];
            PatternTableModel model = (PatternTableModel)values[3];

            SignalManager.Get<PatternTableImageUpdatedSignal>().Dispatch();
        }
    }
}
