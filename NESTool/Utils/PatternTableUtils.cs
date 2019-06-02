﻿using NESTool.FileSystem;
using NESTool.Models;
using NESTool.VOs;
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
                    if (Util.CopyTileSetToBitmap(tile.GUID, tile.Point, ref patternTableBitmap, index, tileSets, ref bitmapCache, sendSignals))
                    {
                        index++;
                    }
                }
            }

            patternTableBitmap.Freeze();

            return patternTableBitmap;
        }
    }
}
