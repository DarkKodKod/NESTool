using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using GroupedPalettes = System.Collections.Generic.Dictionary<System.Tuple<int, NESTool.Enums.PaletteIndex>, System.Collections.Generic.Dictionary<System.Windows.Media.Color, System.Windows.Media.Color>>;

namespace NESTool.Utils;

public static class CharacterUtils
{
    public static ImageVO? CreateImage(CharacterModel characterModel, int animationIndex, int frameIndex, ref GroupedPalettes? groupedPalettes)
    {
        if (characterModel.Animations[animationIndex].Frames == null ||
            characterModel.Animations[animationIndex].Frames.Count == 0)
        {
            return null;
        }

        if (characterModel.Animations[animationIndex].Frames[frameIndex].Tiles == null)
        {
            return null;
        }

        int maxWidth = 0;
        int maxHeight = 0;

        WriteableBitmap? bankBitmap = BitmapFactory.New(64, 64);

        using (bankBitmap.GetBitmapContext())
        {
            List<CharacterTile>? listCharacterTile = characterModel.Animations[animationIndex].Frames[frameIndex].Tiles;

            if (listCharacterTile == null)
                return null;

            foreach (CharacterTile tile in listCharacterTile)
            {
                if (string.IsNullOrEmpty(tile.BankID) || string.IsNullOrEmpty(tile.BankTileID))
                {
                    continue;
                }

                BankModel? ptModel = ProjectFiles.GetModel<BankModel>(tile.BankID);
                if (ptModel == null)
                {
                    continue;
                }

                PTTileModel bankModel = ptModel.GetTileModel(tile.BankTileID);

                if (string.IsNullOrEmpty(bankModel.GUID) && string.IsNullOrEmpty(bankModel.TileSetID))
                {
                    continue;
                }

                WriteableBitmap? sourceBitmap = CreateImageUtil.GetCacheBitmap(bankModel.TileSetID);

                if (sourceBitmap == null)
                {
                    continue;
                }

                WriteableBitmap cropped = sourceBitmap.Crop((int)bankModel.Point.X, (int)bankModel.Point.Y, 8, 8);

                using (cropped.GetBitmapContext())
                {
                    if (tile.FlipX)
                    {
                        cropped = WriteableBitmapExtensions.Flip(cropped, WriteableBitmapExtensions.FlipMode.Vertical);
                    }

                    if (tile.FlipY)
                    {
                        cropped = WriteableBitmapExtensions.Flip(cropped, WriteableBitmapExtensions.FlipMode.Horizontal);
                    }

                    Tuple<int, PaletteIndex> tuple = Tuple.Create(bankModel.Group, (PaletteIndex)tile.PaletteIndex);

                    if (groupedPalettes != null)
                    {
                        if (!groupedPalettes.TryGetValue(tuple, out Dictionary<Color, Color>? colors))
                        {
                            colors = FillColorCacheByGroup(tile, bankModel.Group, characterModel.PaletteIDs[tile.PaletteIndex]);

                            groupedPalettes.Add(tuple, colors);
                        }

                        CreateImageUtil.PaintPixelsBasedOnPalettes(ref cropped, ref colors);
                    }
                }

                int destX = (int)Math.Floor(tile.Point.X / 8) * 8;
                int destY = (int)Math.Floor(tile.Point.Y / 8) * 8;

                if (destX + 8 > maxWidth)
                {
                    maxWidth = destX + 8;
                }
                if (destY + 8 > maxHeight)
                {
                    maxHeight = destY + 8;
                }

                Util.CopyBitmapImageToWriteableBitmap(ref bankBitmap, destX, destY, cropped);
            }
        }

        return new ImageVO() { Image = bankBitmap, Width = maxWidth, Height = maxHeight };
    }

    private static Dictionary<Color, Color> FillColorCacheByGroup(CharacterTile characterTile, int group, string paletteId)
    {
        Color nullColor = Util.NullColor;

        BankModel? bankModel = ProjectFiles.GetModel<BankModel>(characterTile.BankID);

        PaletteModel? paletteModel = ProjectFiles.GetModel<PaletteModel>(paletteId);

        Dictionary<Color, Color> colors = new() { { nullColor, nullColor } };

        if (bankModel == null)
            return colors;

        foreach (PTTileModel tile in bankModel.PTTiles)
        {
            if (string.IsNullOrEmpty(tile.GUID))
            {
                continue;
            }

            if (tile.Group != group)
            {
                continue;
            }

            TileSetModel? model = ProjectFiles.GetModel<TileSetModel>(tile.TileSetID);

            if (model == null)
            {
                continue;
            }

            if (!TileSetModel.BitmapCache.TryGetValue(tile.TileSetID, out WriteableBitmap? tileSetBitmap))
            {
                continue;
            }

            WriteableBitmap cropped = tileSetBitmap.Crop((int)tile.Point.X, (int)tile.Point.Y, 8, 8);

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

        return colors;
    }
}
