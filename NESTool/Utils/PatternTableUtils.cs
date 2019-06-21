using ArchitectureLibrary.Signals;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace NESTool.Utils
{
    public static class PatternTableUtils
    {
        public static WriteableBitmap CreateImage(PatternTableModel patternTableModel, ref Dictionary<string, WriteableBitmap> bitmapCache, bool sendSignals = true)
        {
            FileModelVO[] tileSets = ProjectFiles.GetModels<TileSetModel>().ToArray();

            WriteableBitmap patternTableBitmap = BitmapFactory.New(128, 128);

            using (patternTableBitmap.GetBitmapContext())
            {
                int index = 0;

                foreach (PTTileModel tile in patternTableModel.PTTiles)
                {
                    if (string.IsNullOrEmpty(tile.GUID) || string.IsNullOrEmpty(tile.TileSetID))
                    {
                        continue;
                    }

                    if (!bitmapCache.TryGetValue(tile.TileSetID, out WriteableBitmap sourceBitmap))
                    {
                        TileSetModel model = ProjectFiles.GetModel<TileSetModel>(tile.TileSetID);

                        if (model == null)
                        {
                            continue;
                        }

                        BitmapImage bmImage = new BitmapImage();

                        bmImage.BeginInit();
                        bmImage.CacheOption = BitmapCacheOption.OnLoad;
                        bmImage.UriSource = new Uri(model.ImagePath, UriKind.Absolute);
                        bmImage.EndInit();
                        bmImage.Freeze();

                        sourceBitmap = BitmapFactory.ConvertToPbgra32Format(bmImage as BitmapSource);

                        bitmapCache.Add(tile.TileSetID, sourceBitmap);

                        // Add the link object
                        foreach (FileModelVO tileset in tileSets)
                        {
                            if (tileset.Model.GUID == tile.TileSetID && sendSignals)
                            {
                                SignalManager.Get<AddNewTileSetLinkSignal>().Dispatch(new PatternTableLinkVO() { Caption = tileset.Name, Id = tile.TileSetID });
                                break;
                            }
                        }
                    }

                    using (sourceBitmap.GetBitmapContext())
                    {
                        WriteableBitmap cropped = sourceBitmap.Crop((int)tile.Point.X, (int)tile.Point.Y, 8, 8);
                        BitmapImage croppedBitmap = Util.ConvertWriteableBitmapToBitmapImage(cropped);

                        int destX = (index % 16) * 8;
                        int destY = (index / 16) * 8;

                        Util.CopyBitmapImageToWriteableBitmap(ref patternTableBitmap, destX, destY, croppedBitmap);
                    }

                    index++;
                }
            }

            return patternTableBitmap;
        }
    }
}