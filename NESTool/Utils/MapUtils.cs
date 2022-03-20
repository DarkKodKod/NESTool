using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Utils
{
    public static class MapUtils
    {
        private static Color BackgroundColor = Color.FromRgb(System.Drawing.Color.DarkGray.R, System.Drawing.Color.DarkGray.G, System.Drawing.Color.DarkGray.B);

        public static void CreateImage(MapModel mapModel, ref WriteableBitmap mapBitmap, bool isUpdate)
        {
            if (!isUpdate)
            {
                mapBitmap = BitmapFactory.New(256, 240);
            }

            using (mapBitmap.GetBitmapContext())
            {
                for (int i = 0; i < mapModel.AttributeTable.Length; ++i)
                {
                    AttributeTable attTable = mapModel.AttributeTable[i];

                    if (attTable.MapTile == null)
                    {
                        continue;
                    }

                    bool erased = false;
                    bool updateTile = false;

                    if (isUpdate)
                    {
                        // Only update the tiles that were affected by the change
                        if (MapViewModel.FlagMapBitmapChanges[i] == TileUpdate.None)
                        {
                            continue;
                        }

                        erased = MapViewModel.FlagMapBitmapChanges[i] == TileUpdate.Erased;
                        updateTile = MapViewModel.FlagMapBitmapChanges[i] == TileUpdate.Normal;

                        MapViewModel.FlagMapBitmapChanges[i] = TileUpdate.None;
                    }

                    foreach (MapTile tile in attTable.MapTile)
                    {
                        if (isUpdate && !updateTile)
                        {
                            if (tile.Point != MapViewModel.PointMapBitmapChanges[i])
                            {
                                continue;
                            }
                        }

                        if (string.IsNullOrEmpty(tile.BankID) || string.IsNullOrEmpty(tile.BankTileID))
                        {
                            continue;
                        }

                        BankModel bankModel = ProjectFiles.GetModel<BankModel>(tile.BankID);

                        if (bankModel == null)
                        {
                            continue;
                        }

                        PTTileModel ptTileModel = bankModel.GetTileModel(tile.BankTileID);

                        if (ptTileModel.TileSetID == null)
                        {
                            continue;
                        }

                        WriteableBitmap tileSetBitmap = GetCacheBitmap(ptTileModel.TileSetID);
                        
                        if (tileSetBitmap == null)
                        {
                            continue;
                        }

                        CacheColorsFromBank(ptTileModel.Group, attTable, mapModel, bankModel);

                        using (tileSetBitmap.GetBitmapContext())
                        {
                            WriteableBitmap cropped = tileSetBitmap.Crop((int)ptTileModel.Point.X, (int)ptTileModel.Point.Y, 8, 8);

                            if (erased)
                            {
                                cropped.Clear(BackgroundColor);
                            }
                            else
                            {
                                Tuple<int, PaletteIndex> colorsKey = Tuple.Create(ptTileModel.Group, (PaletteIndex)attTable.PaletteIndex);

                                PaintPixelsBasedOnPalettes(ref cropped, colorsKey);
                            }

                            cropped.Freeze();

                            BitmapImage croppedBitmap = Util.ConvertWriteableBitmapToBitmapImage(cropped);

                            int destX = (int)Math.Floor(tile.Point.X / 8) * 8;
                            int destY = (int)Math.Floor(tile.Point.Y / 8) * 8;

                            Util.CopyBitmapImageToWriteableBitmap(ref mapBitmap, destX, destY, croppedBitmap);
                        }
                    }
                }
            }
        }

        private static WriteableBitmap GetCacheBitmap(string tileSetID)
        {
            if (!TileSetModel.BitmapCache.TryGetValue(tileSetID, out WriteableBitmap tileSetBitmap))
            {
                // The tileset exists but the bitmap is not in the cache, so I will try to load it here
                TileSetModel tileSetModel = ProjectFiles.GetModel<TileSetModel>(tileSetID);

                return tileSetModel != null ? TileSetModel.LoadBitmap(tileSetModel) : null;
            }

            return tileSetBitmap;
        }

        private static void CacheColorsFromBank(int group, AttributeTable attributeTable, MapModel mapModel, BankModel bankModel)
        {
            string paletteId = mapModel.PaletteIDs[(int)attributeTable.PaletteIndex];

            PaletteModel paletteModel = ProjectFiles.GetModel<PaletteModel>(paletteId);

            Color firstColor = paletteModel == null ? Util.NullColor : Util.GetColorFromInt(paletteModel.Color0);

            Tuple<int, PaletteIndex> tuple = Tuple.Create(group, (PaletteIndex)attributeTable.PaletteIndex);

            if (!MapViewModel.GroupedPalettes.TryGetValue(tuple, out Dictionary<Color, Color> colors))
            {
                colors = new Dictionary<Color, Color>
                {
                    // always add the first color of the palette as the background color
                    { Util.NullColor, firstColor }
                };

                MapViewModel.GroupedPalettes.Add(tuple, colors);
            }

            // only 4 color per tile
            if (colors.Count >= 4)
            {
                return;
            }

            foreach (PTTileModel tile in bankModel.PTTiles)
            {
                if (tile.Group != group)
                {
                    continue;
                }

                TileSetModel.BitmapCache.TryGetValue(tile.TileSetID, out WriteableBitmap tileSetBitmap);

                if (tileSetBitmap == null)
                {
                    continue;
                }

                WriteableBitmap bitmap = tileSetBitmap.Crop((int)tile.Point.X, (int)tile.Point.Y, 8, 8);

                for (int y = 0; y < 8; ++y)
                {
                    for (int x = 0; x < 8; ++x)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        color.A = 255;

                        if (!colors.TryGetValue(color, out _))
                        {
                            int paletteColor;
                            switch (colors.Count)
                            {
                                case 0: paletteColor = paletteModel == null ? 0 : paletteModel.Color0; break;
                                case 1: paletteColor = paletteModel == null ? 0 : paletteModel.Color1; break;
                                case 2: paletteColor = paletteModel == null ? 0 : paletteModel.Color2; break;
                                case 3: paletteColor = paletteModel == null ? 0 : paletteModel.Color3; break;
                                default: paletteColor = paletteModel == null ? 0 : paletteModel.Color0; break;
                            }

                            Color newColor = Util.GetColorFromInt(paletteColor);
                            colors.Add(color, newColor);
                        }

                        if (colors.Count >= 4)
                        {
                            return;
                        }
                    }
                }
            }
        }

        private static void PaintPixelsBasedOnPalettes(ref WriteableBitmap bitmap, Tuple<int, PaletteIndex> colorsKey)
        {
            if (!MapViewModel.GroupedPalettes.TryGetValue(colorsKey, out Dictionary<Color, Color> colors))
            {
                colors = new Dictionary<Color, Color>();
            }

            // read pixels in the 8x8 quadrant
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    Color color = bitmap.GetPixel(x, y);
                    color.A = 255;

                    if (!colors.TryGetValue(color, out Color newColor))
                    {
                        newColor = Util.NullColor;
                    }

                    bitmap.SetPixel(x, y, newColor);
                }
            }
        }
    }
}
