using NESTool.FileSystem;
using NESTool.Models;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Utils;

public static class CreateImageUtil
{
    public static WriteableBitmap GetCacheBitmap(string tileSetID)
    {
        if (!TileSetModel.BitmapCache.TryGetValue(tileSetID, out WriteableBitmap tileSetBitmap))
        {
            // The tileset exists but the bitmap is not in the cache, so I will try to load it here
            TileSetModel tileSetModel = ProjectFiles.GetModel<TileSetModel>(tileSetID);

            return tileSetModel != null ? TileSetModel.LoadBitmap(tileSetModel) : null;
        }

        return tileSetBitmap;
    }

    public static void PaintPixelsBasedOnPalettes(ref WriteableBitmap bitmap, ref Dictionary<Color, Color> colors)
    {
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
