using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NESTool.Commands;

public class MoveTileToBankCommand : Command
{
    public override bool CanExecute(object? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        object[] values = (object[])parameter;
        WriteableBitmap cropperImage = (WriteableBitmap)values[0];
        BankModel model = (BankModel)values[3];

        return cropperImage != null && model != null && !model.IsFull();
    }

    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        object[] values = (object[])parameter;

        Point croppedPoint = (Point)values[1];
        int selectedTileSet = (int)values[2];
        BankModel model = (BankModel)values[3];

        if (model.GetEmptyTileIndex(out int index))
        {
            FileModelVO[] tileSets = ProjectFiles.GetModels<TileSetModel>().ToArray();

            AFileModel? theModel = tileSets[selectedTileSet].Model;

            if (theModel != null)
            {
                model.PTTiles[index].GUID = Guid.NewGuid().ToString();
                model.PTTiles[index].TileSetID = theModel.GUID;
                model.PTTiles[index].Point = croppedPoint;
                model.PTTiles[index].Group = index;
            }

            SignalManager.Get<BankImageUpdatedSignal>().Dispatch();
        }
    }
}
