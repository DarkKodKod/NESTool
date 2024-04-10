using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace NESTool.Utils;

public static class BanksUtils
{
    public static WriteableBitmap? CreateImage(BankModel bankModel, ref Dictionary<string, WriteableBitmap> bitmapCache, bool sendSignals = true)
    {
        FileModelVO[] tileSets = ProjectFiles.GetModels<TileSetModel>().ToArray();

        WriteableBitmap? bankBitmap = BitmapFactory.New(128, 128);

        int index = 0;

        foreach (PTTileModel tile in bankModel.PTTiles)
        {
            if (string.IsNullOrEmpty(tile.GUID) || string.IsNullOrEmpty(tile.TileSetID))
            {
                continue;
            }

            if (!bitmapCache.TryGetValue(tile.TileSetID, out WriteableBitmap? sourceBitmap))
            {
                TileSetModel? model = ProjectFiles.GetModel<TileSetModel>(tile.TileSetID);

                if (model == null)
                {
                    continue;
                }

                ProjectModel projectModel = ModelManager.Get<ProjectModel>();

                string path = Path.Combine(projectModel.ProjectPath, model.ImagePath);

                BitmapImage bmImage = new BitmapImage();
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                bmImage.EndInit();
                bmImage.Freeze();

                sourceBitmap = BitmapFactory.ConvertToPbgra32Format(bmImage);

                bitmapCache.Add(tile.TileSetID, sourceBitmap);

                if (sendSignals)
                {
                    // Add the link object
                    foreach (FileModelVO tileset in tileSets)
                    {
                        if (tileset.Model?.GUID == tile.TileSetID)
                        {
                            SignalManager.Get<AddNewTileSetLinkSignal>().Dispatch(new BankLinkVO() { Caption = tileset.Name, Id = tile.TileSetID });
                            break;
                        }
                    }
                }
            }

            WriteableBitmap cropped = sourceBitmap.Crop((int)tile.Point.X, (int)tile.Point.Y, 8, 8);

            int destX = index % 16 * 8;
            int destY = index / 16 * 8;

            Util.CopyBitmapImageToWriteableBitmap(ref bankBitmap, destX, destY, cropped);

            index++;
        }

        return bankBitmap;
    }
}