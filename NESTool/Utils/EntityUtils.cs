using NESTool.FileSystem;
using NESTool.Models;
using System;
using System.Windows.Media.Imaging;

namespace NESTool.Utils
{
    public static class EntityUtils
    {
        public static WriteableBitmap CreateImage(EntityModel entityModel)
        {
            WriteableBitmap bankBitmap = BitmapFactory.New(64, 64);

            using (bankBitmap.GetBitmapContext())
            {
                foreach (CharacterTile tile in entityModel.Frame.Tiles)
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

                    if (string.IsNullOrEmpty(bankModel.GUID) && string.IsNullOrEmpty(bankModel.TileSetID))
                    {
                        continue;
                    }

                    WriteableBitmap sourceBitmap = CreateImageUtil.GetCacheBitmap(bankModel.TileSetID);

                    if (sourceBitmap == null)
                    {
                        continue;
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

                        int destX = (int)Math.Floor(tile.Point.X / 8) * 8;
                        int destY = (int)Math.Floor(tile.Point.Y / 8) * 8;

                        Util.CopyBitmapImageToWriteableBitmap(ref bankBitmap, destX, destY, cropped);
                    }
                }
            }

            return bankBitmap;
        }
    }
}
