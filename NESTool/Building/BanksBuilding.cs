using ArchitectureLibrary.Model;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Utils;
using NESTool.VOs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Building;

public static class BanksBuilding
{
    public static void Execute()
    {
        ProjectModel projectModel = ModelManager.Get<ProjectModel>();

        string outputPath = Path.GetFullPath(projectModel.Build.OutputFilePath);

        List<FileModelVO> bankModelVOs = ProjectFiles.GetModels<BankModel>();

        const int cellSizeInBytes = 128; // 16 bytes each cell

        foreach (FileModelVO vo in bankModelVOs)
        {
            BankModel? bank = vo.Model as BankModel;

            if (bank == null)
                continue;

            int cellsCount = 0;

            switch (bank.BankSize)
            {
                case BankSize.Size4Kb: cellsCount = 16 * 16; break;
                case BankSize.Size2Kb: cellsCount = 16 * 8; break;
                case BankSize.Size1Kb: cellsCount = 16 * 4; break;
            }

            Dictionary<string, WriteableBitmap> bitmapCache = new();

            BitArray outputBits = new(cellSizeInBytes * cellsCount);
            outputBits.SetAll(false);

            int currentIndex = 0;

            WriteableBitmap? bitmap = BanksUtils.CreateImage(bank, ref bitmapCache, false);

            if (bitmap != null)
            {
                using (bitmap.GetBitmapContext())
                {
                    WriteIntoBitArray(bank, bitmap, ref outputBits, ref currentIndex);
                }
            }

            for (int i = 0; i < cellSizeInBytes * cellsCount;)
            {
                Reverse(ref outputBits, i, 8);
                i += 8;
            }

            byte[] bytes = new byte[outputBits.Length / 8];

            outputBits.CopyTo(bytes, 0);

            string fullPath = Path.Combine(outputPath, vo.Name.ToLower() + ".bin");

            File.WriteAllBytes(fullPath, bytes);
        }
    }

    private static void Reverse(ref BitArray array, int start, int length)
    {
        int mid = length / 2;

        for (int i = start; i < start + mid; i++)
        {
            bool bit = array[i];
            array[i] = array[start + start + length - i - 1];
            array[start + start + length - i - 1] = bit;
        }
    }

    private static void WriteIntoBitArray(BankModel model, WriteableBitmap bitmap, ref BitArray bits, ref int currentIndex)
    {
        int currentX = 0;
        int currentY = 0;
        int matrixIndex = 0;

        Dictionary<int, Dictionary<Color, int>> groupedPalettes = new();

        // go throug the 16x16 tiles
        for (int j = 0; j < 16; ++j)
        {
            for (int i = 0; i < 16; ++i)
            {
                int group = model.PTTiles[matrixIndex++].Group;

                if (!groupedPalettes.TryGetValue(group, out Dictionary<Color, int>? colors))
                {
                    colors = new Dictionary<Color, int> { { Util.NullColor, 0 } };

                    groupedPalettes.Add(group, colors);
                }

                // read pixels in the 8x8 quadrant
                for (int y = currentY; y < currentY + 8; ++y)
                {
                    for (int x = currentX; x < currentX + 8; ++x)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        color.A = 255;

                        if (!Util.NullColor.Equals(color))
                        {
                            if (!colors.TryGetValue(color, out int value))
                            {
                                if (colors.Count < 4)
                                {
                                    value = colors.Count;

                                    colors.Add(color, value);
                                }
                                else
                                {
                                    value = -1;
                                }
                            }

                            switch (value)
                            {
                                case 1:
                                    bits[currentIndex] = true;
                                    break;
                                case 2:
                                    bits[currentIndex + 64] = true;
                                    break;
                                case 3:
                                    bits[currentIndex] = true;
                                    bits[currentIndex + 64] = true;
                                    break;
                            }
                        }

                        currentIndex++;
                    }
                }

                currentX += 8;
                currentIndex += 64;
            }

            currentX = 0;
            currentY += 8;
        }
    }
}
