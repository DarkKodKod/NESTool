using NESTool.FileSystem;
using NESTool.Models;
using NESTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Utils
{
    public static class CharacterUtils
    {
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

            WriteableBitmap patternTableBitmap = BitmapFactory.New(64, 64);

            using (patternTableBitmap.GetBitmapContext())
            {
                foreach (CharacterTile tile in characterModel.Animations[animationIndex].Frames[frameIndex].Tiles)
                {
                    if (string.IsNullOrEmpty(tile.BankID) || string.IsNullOrEmpty(tile.BankTileID))
                    {
                        continue;
                    }

                    PatternTableModel ptModel = ProjectFiles.GetModel<PatternTableModel>(tile.BankID);
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

                        BitmapImage bmImage = new BitmapImage();

                        bmImage.BeginInit();
                        bmImage.CacheOption = BitmapCacheOption.OnLoad;
                        bmImage.UriSource = new Uri(model.ImagePath, UriKind.Absolute);
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

                        Util.CopyBitmapImageToWriteableBitmap(ref patternTableBitmap, destX, destY, croppedBitmap);
                    }
                }
            }

            patternTableBitmap.Freeze();

            return patternTableBitmap;
        }

        private static void PaintPixelsBasedOnPalettes(ref WriteableBitmap bitmap, CharacterTile tile, CharacterModel model, int group)
        {
            var tuple = Tuple.Create(group, tile.PaletteIndex);

            if (!CharacterViewModel.GroupedPalettes.TryGetValue(tuple, out Dictionary<Color, Color> colors))
            {
                colors = new Dictionary<Color, Color>
                {
                    // always add the background by default
                    { Color.FromRgb(0, 0, 0), Color.FromRgb(0, 0, 0) }
                };

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
                        int paletteColor;
                        switch (colors.Count)
                        {
                            case 0: paletteColor = model.Palettes[tile.PaletteIndex].Color0; break;
                            case 1: paletteColor = model.Palettes[tile.PaletteIndex].Color1; break;
                            case 2: paletteColor = model.Palettes[tile.PaletteIndex].Color2; break;
                            case 3: paletteColor = model.Palettes[tile.PaletteIndex].Color3; break;
                            default: paletteColor = model.Palettes[tile.PaletteIndex].Color0; break;
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
