using NESTool.FileSystem;
using NESTool.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Utils
{
    public static class CharacterUtils
    {
        static Dictionary<int, Dictionary<Color, Color>> _groupedPalettes;

        public static WriteableBitmap CreateImage(CharacterModel characterModel, int animationIndex, int frameIndex, ref Dictionary<string, WriteableBitmap> bitmapCache)
        {
            if (characterModel.Animations[animationIndex].Frames[frameIndex].Tiles == null)
            {
                return null;
            }

            WriteableBitmap patternTableBitmap = BitmapFactory.New(64, 64);

            _groupedPalettes = new Dictionary<int, Dictionary<Color, Color>>();

            using (patternTableBitmap.GetBitmapContext())
            {
                foreach (CharacterTile tile in characterModel.Animations[animationIndex].Frames[frameIndex].Tiles)
                {
                    if (string.IsNullOrEmpty(tile.GUID))
                    {
                        continue;
                    }

                    if (!bitmapCache.TryGetValue(tile.GUID, out WriteableBitmap sourceBitmap))
                    {
                        TileSetModel model = ProjectFiles.GetModel<TileSetModel>(tile.GUID);

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

                        bitmapCache.Add(tile.GUID, sourceBitmap);
                    }

                    using (sourceBitmap.GetBitmapContext())
                    {
                        WriteableBitmap cropped = sourceBitmap.Crop((int)tile.OriginPoint.X, (int)tile.OriginPoint.Y, 8, 8);
                        
                        if (tile.FlipX)
                        {
                            cropped = WriteableBitmapExtensions.Flip(cropped, WriteableBitmapExtensions.FlipMode.Vertical);
                        }

                        if (tile.FlipY)
                        {
                            cropped = WriteableBitmapExtensions.Flip(cropped, WriteableBitmapExtensions.FlipMode.Horizontal);
                        }

                        PaintPixelsBasedOnPalettes(ref cropped, tile, characterModel.Animations[animationIndex]);

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

        private static void PaintPixelsBasedOnPalettes(ref WriteableBitmap bitmap, CharacterTile tile, CharacterAnimation animation)
        {
            int group = tile.Group;

            if (!_groupedPalettes.TryGetValue(group, out Dictionary<Color, Color> colors))
            {
                colors = new Dictionary<Color, Color>();

                _groupedPalettes.Add(group, colors);
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
                        int paletteColor = 0;

                        switch (colors.Count)
                        {
                        case 0: paletteColor = animation.Palettes[tile.PaletteIndex].Color0; break;
                        case 1: paletteColor = animation.Palettes[tile.PaletteIndex].Color1; break;
                        case 2: paletteColor = animation.Palettes[tile.PaletteIndex].Color2; break;
                        case 3: paletteColor = animation.Palettes[tile.PaletteIndex].Color3; break;
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
