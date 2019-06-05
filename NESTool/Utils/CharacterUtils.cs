using NESTool.FileSystem;
using NESTool.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace NESTool.Utils
{
    public static class CharacterUtils
    {
        public static WriteableBitmap CreateImage(CharacterModel characterModel, int animationIndex, int frameIndex, ref Dictionary<string, WriteableBitmap> bitmapCache)
        {
            WriteableBitmap patternTableBitmap = BitmapFactory.New(64, 64);

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
    }
}
