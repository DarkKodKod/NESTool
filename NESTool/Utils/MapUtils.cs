﻿using ArchitectureLibrary.Model;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Utils
{
    public static class MapUtils
    {
        private static Color NullColor = Util.GetColorFromInt(0);

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

                    if (isUpdate == true)
                    {
                        // Only update the tiles that were affected by the change
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

                        if (bankModel.TileSetID == null)
                        {
                            continue;
                        }

                        if (!MapViewModel.FrameBitmapCache.TryGetValue(bankModel.TileSetID, out WriteableBitmap sourceBitmap))
                        {
                            TileSetModel model = ProjectFiles.GetModel<TileSetModel>(bankModel.TileSetID);

                            if (model == null)
                            {
                                continue;
                            }

                            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

                            BitmapImage bmImage = new BitmapImage();

                            bmImage.BeginInit();
                            bmImage.CacheOption = BitmapCacheOption.OnLoad;
                            bmImage.UriSource = new Uri(Path.Combine(projectModel.ProjectPath, model.ImagePath), UriKind.RelativeOrAbsolute);
                            bmImage.EndInit();
                            bmImage.Freeze();

                            sourceBitmap = BitmapFactory.ConvertToPbgra32Format(bmImage as BitmapSource);

                            MapViewModel.FrameBitmapCache.Add(bankModel.TileSetID, sourceBitmap);
                        }

                        CacheColorsFromBank(sourceBitmap, bankModel.Group, attTable, mapModel, ptModel);

                        using (sourceBitmap.GetBitmapContext())
                        {
                            WriteableBitmap cropped = sourceBitmap.Crop((int)bankModel.Point.X, (int)bankModel.Point.Y, 8, 8);

                            Tuple<int, int> colorsKey = Tuple.Create(bankModel.Group, attTable.PaletteIndex);

                            PaintPixelsBasedOnPalettes(ref cropped, colorsKey);

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

        private static void CacheColorsFromBank(WriteableBitmap bankImage, int group, AttributeTable attributeTable, MapModel mapModel, BankModel bankModel)
        {
            string paletteId = mapModel.PaletteIDs[attributeTable.PaletteIndex];

            PaletteModel paletteModel = ProjectFiles.GetModel<PaletteModel>(paletteId);

            Color firstColor = paletteModel == null ? NullColor : Util.GetColorFromInt(paletteModel.Color0);

            Tuple<int, int> tuple = Tuple.Create(group, attributeTable.PaletteIndex);

            if (!MapViewModel.GroupedPalettes.TryGetValue(tuple, out Dictionary<Color, Color> colors))
            {
                colors = new Dictionary<Color, Color>
                {
                    // always add the first color of the palette as the background color
                    { NullColor, firstColor }
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

                WriteableBitmap bitmap = bankImage.Crop((int)tile.Point.X, (int)tile.Point.Y, 8, 8);

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
                                case 0: paletteColor = paletteModel.Color0; break;
                                case 1: paletteColor = paletteModel.Color1; break;
                                case 2: paletteColor = paletteModel.Color2; break;
                                case 3: paletteColor = paletteModel.Color3; break;
                                default: paletteColor = paletteModel.Color0; break;
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

        private static void PaintPixelsBasedOnPalettes(ref WriteableBitmap bitmap, Tuple<int, int> colorsKey)
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
                        newColor = NullColor;
                    }

                    bitmap.SetPixel(x, y, newColor);
                }
            }
        }
    }
}
