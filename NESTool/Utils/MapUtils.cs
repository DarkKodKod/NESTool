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
        public static void CreateImage(MapModel mapModel, ref WriteableBitmap mapBitmap, bool update)
        {
            if (!update)
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

                    // Only update the tiles that were affected by any changes
                    if (update == true)
                    {
                        if (MapViewModel.FlagMapBitmapChanges[i] == false)
                        {
                            continue;
                        }

                        MapViewModel.FlagMapBitmapChanges[i] = false;
                    }

                    foreach (MapTile tile in attTable.MapTile)
                    {
                        if (string.IsNullOrEmpty(tile.BankID) || string.IsNullOrEmpty(tile.BankTileID))
                        {
                            continue;
                        }

                        BankModel ptModel = ProjectFiles.GetModel<BankModel>(tile.BankID);

                        if (ptModel == null)
                        {
                            continue;
                        }

                        PTTileModel bankModel = ptModel.GetTileModel(tile.BankTileID);

                        if (!MapViewModel.FrameBitmapCache.TryGetValue(bankModel.TileSetID, out WriteableBitmap sourceBitmap))
                        {
                            TileSetModel model = ProjectFiles.GetModel<TileSetModel>(bankModel.TileSetID);

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

                            MapViewModel.FrameBitmapCache.Add(bankModel.TileSetID, sourceBitmap);
                        }

                        using (sourceBitmap.GetBitmapContext())
                        {
                            WriteableBitmap cropped = sourceBitmap.Crop((int)bankModel.Point.X, (int)bankModel.Point.Y, 8, 8);

                            PaintPixelsBasedOnPalettes(ref cropped, attTable, mapModel, bankModel.Group);

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

        private static void PaintPixelsBasedOnPalettes(ref WriteableBitmap bitmap, AttributeTable attributeTable, MapModel model, int group)
        {
            Tuple<int, int> tuple = Tuple.Create(group, attributeTable.PaletteIndex);

            if (!MapViewModel.GroupedPalettes.TryGetValue(tuple, out Dictionary<Color, Color> colors))
            {
                colors = new Dictionary<Color, Color>
                {
                    // always add the background by default
                    { Color.FromRgb(0, 0, 0), Color.FromRgb(0, 0, 0) }
                };

                MapViewModel.GroupedPalettes.Add(tuple, colors);
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
                        int paletteColor;
                        switch (colors.Count)
                        {
                            case 0: paletteColor = model.Palettes[attributeTable.PaletteIndex].Color0; break;
                            case 1: paletteColor = model.Palettes[attributeTable.PaletteIndex].Color1; break;
                            case 2: paletteColor = model.Palettes[attributeTable.PaletteIndex].Color2; break;
                            case 3: paletteColor = model.Palettes[attributeTable.PaletteIndex].Color3; break;
                            default: paletteColor = model.Palettes[attributeTable.PaletteIndex].Color0; break;
                        }

                        byte R = (byte)(paletteColor >> 16);
                        byte G = (byte)(paletteColor >> 8);
                        byte B = (byte)paletteColor;

                        newColor = Color.FromRgb(R, G, B);

                        colors.Add(color, newColor);
                    }

                    bitmap.SetPixel(x, y, newColor);
                }
            }
        }
    }
}
