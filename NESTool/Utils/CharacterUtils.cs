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
    public static class CharacterUtils
    {
        private static readonly int NullColor = 0;

        public static WriteableBitmap CreateImage(CharacterModel characterModel, int animationIndex, int frameIndex)
        {
            if (characterModel.Animations[animationIndex].Frames == null)
            {
                return null;
            }

            if (characterModel.Animations[animationIndex].Frames[frameIndex].Tiles == null)
            {
                return null;
            }

            WriteableBitmap bankBitmap = BitmapFactory.New(64, 64);

            using (bankBitmap.GetBitmapContext())
            {
                foreach (CharacterTile tile in characterModel.Animations[animationIndex].Frames[frameIndex].Tiles)
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

                    if (!CharacterViewModel.FrameBitmapCache.TryGetValue(bankModel.TileSetID, out WriteableBitmap sourceBitmap))
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

                        CharacterViewModel.FrameBitmapCache.Add(bankModel.TileSetID, sourceBitmap);
                    }

                    using (sourceBitmap.GetBitmapContext())
                    {
                        WriteableBitmap cropped = sourceBitmap.Crop((int)bankModel.Point.X, (int)bankModel.Point.Y, 8, 8);

                        if (tile.FlipX)
                        {
                            cropped = WriteableBitmapExtensions.Flip(cropped, WriteableBitmapExtensions.FlipMode.Vertical);
                        }

                        if (tile.FlipY)
                        {
                            cropped = WriteableBitmapExtensions.Flip(cropped, WriteableBitmapExtensions.FlipMode.Horizontal);
                        }

                        PaintPixelsBasedOnPalettes(ref cropped, tile, characterModel, bankModel.Group);

                        cropped.Freeze();

                        BitmapImage croppedBitmap = Util.ConvertWriteableBitmapToBitmapImage(cropped);

                        int destX = (int)Math.Floor(tile.Point.X / 8) * 8;
                        int destY = (int)Math.Floor(tile.Point.Y / 8) * 8;

                        Util.CopyBitmapImageToWriteableBitmap(ref bankBitmap, destX, destY, croppedBitmap);
                    }
                }
            }

            bankBitmap.Freeze();

            return bankBitmap;
        }

        private static void PaintPixelsBasedOnPalettes(ref WriteableBitmap bitmap, CharacterTile tile, CharacterModel model, int group)
        {
            Tuple<int, int> tuple = Tuple.Create(group, tile.PaletteIndex);

            if (!CharacterViewModel.GroupedPalettes.TryGetValue(tuple, out Dictionary<Color, Color> colors))
            {
                colors = FillColorCacheByGroup(tile, group, model.PaletteIDs[tile.PaletteIndex]);

                CharacterViewModel.GroupedPalettes.Add(tuple, colors);
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
                        newColor = Util.GetColorFromInt(NullColor);
                    }

                    bitmap.SetPixel(x, y, newColor);
                }
            }
        }

        private static Dictionary<Color, Color> FillColorCacheByGroup(CharacterTile characterTile, int group, string paletteId)
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();

            Color nullColor = Util.GetColorFromInt(NullColor);

            BankModel bank = ProjectFiles.GetModel<BankModel>(characterTile.BankID);

            PaletteModel paletteModel = ProjectFiles.GetModel<PaletteModel>(paletteId);

            Dictionary<Color, Color> colors = new Dictionary<Color, Color>() { { nullColor, nullColor } };

            for (int i = 0; i < bank.PTTiles.Length; ++i)
            {
                PTTileModel tile = bank.PTTiles[i];

                if (string.IsNullOrEmpty(tile.GUID))
                {
                    continue;
                }

                if (tile.Group == group)
                {
                    TileSetModel model = ProjectFiles.GetModel<TileSetModel>(tile.TileSetID);

                    if (model == null)
                    {
                        continue;
                    }

                    bool exists = CharacterViewModel.FrameBitmapCache.TryGetValue(tile.TileSetID, out WriteableBitmap writeableBmp);

                    if (!exists)
                    {
                        BitmapImage bmImage = new BitmapImage();

                        string path = Path.Combine(project.ProjectPath, model.ImagePath);

                        bmImage.BeginInit();
                        bmImage.CacheOption = BitmapCacheOption.OnLoad;
                        bmImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                        bmImage.EndInit();
                        bmImage.Freeze();

                        writeableBmp = BitmapFactory.ConvertToPbgra32Format(bmImage as BitmapSource);

                        CharacterViewModel.FrameBitmapCache.Add(tile.TileSetID, writeableBmp);
                    }

                    using (writeableBmp.GetBitmapContext())
                    {
                        WriteableBitmap cropped = writeableBmp.Crop((int)tile.Point.X, (int)tile.Point.Y, 8, 8);

                        for (int y = 0; y < 8; ++y)
                        {
                            for (int x = 0; x < 8; ++x)
                            {
                                Color color = cropped.GetPixel(x, y);
                                color.A = 255;

                                if (!colors.TryGetValue(color, out Color newColor))
                                {
                                    if (paletteModel == null)
                                    {
                                        newColor = nullColor;
                                    }
                                    else
                                    {
                                        switch (colors.Count)
                                        {
                                            case 1: newColor = Util.GetColorFromInt(paletteModel.Color1); break;
                                            case 2: newColor = Util.GetColorFromInt(paletteModel.Color2); break;
                                            case 3: newColor = Util.GetColorFromInt(paletteModel.Color3); break;
                                        }
                                    }

                                    colors.Add(color, newColor);
                                }
                            }
                        }
                    }
                }
            }

            return colors;
        }
    }
}
