using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class DeleteBankTileCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            object[] values = (object[])parameter;

            int selectedTile = (int)values[0];
            BankModel model = (BankModel)values[1];

            if (selectedTile < 0 || model == null)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            object[] values = (object[])parameter;

            int selectedTile = (int)values[0];
            BankModel model = (BankModel)values[1];

            model.PTTiles[selectedTile].GUID = string.Empty;

            for (int i = selectedTile; i < 255; ++i)
            {
                PTTileModel tmp = model.PTTiles[i];

                model.PTTiles[i] = model.PTTiles[i + 1];

                model.PTTiles[i + 1] = tmp;
            }

            SignalManager.Get<BankTileDeletedSignal>().Dispatch();
        }
    }
}
