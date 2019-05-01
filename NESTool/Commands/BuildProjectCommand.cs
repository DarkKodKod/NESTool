using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Commands
{
    public class BuildProjectCommand : Command
    {
        private const string _patterntableOutputFileKey = "PatterntableOutputFile";
        private Dictionary<string, WriteableBitmap> _bitmapCache = new Dictionary<string, WriteableBitmap>();
        private readonly string _patterntableOutputFile;

        public BuildProjectCommand()
        {
            _patterntableOutputFile = (string)Application.Current.FindResource(_patterntableOutputFileKey);
        }

        public override void Execute(object parameter)
        {
            BuildPatternTables();

            MessageBox.Show("Build completed!", "Build", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BuildPatternTables()
        {
            const int cells = 16 * 16;
            const int tables = 2;
            const int sizeCell = 128; // 16 bytes each cell

            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            BitArray outputBits = new BitArray(sizeCell * cells * tables);
            outputBits.SetAll(false);

            if (!string.IsNullOrEmpty(projectModel.Build.PatternTableSpriteId))
            {
                PatternTableModel model = ProjectFiles.GetModel<PatternTableModel>(projectModel.Build.PatternTableSpriteId);

                if (model != null)
                {
                    WriteableBitmap bitmap = PatternTableUtils.CreateImage(model, ref _bitmapCache, false);

                    using (bitmap.GetBitmapContext())
                    {
                        WriteIntoBitArray(bitmap, ref outputBits);
                    }
                }
            }

            if (!string.IsNullOrEmpty(projectModel.Build.PatternTableBackgroundId))
            {
                PatternTableModel model = ProjectFiles.GetModel<PatternTableModel>(projectModel.Build.PatternTableBackgroundId);

                if (model != null)
                {
                    WriteableBitmap bitmap = PatternTableUtils.CreateImage(model, ref _bitmapCache);

                    using (bitmap.GetBitmapContext())
                    {
                        WriteIntoBitArray(bitmap, ref outputBits);
                    }
                }
            }

            for (int i = 0; i < 16*16*16;)
            {
                Reverse(ref outputBits, i, 8);
                i += 8;
            }

            byte[] bytes = new byte[outputBits.Length / 8];

            outputBits.CopyTo(bytes, 0);

            File.WriteAllBytes(Path.Combine(projectModel.Build.OutputFilePath, _patterntableOutputFile), bytes);
        }

        private void Reverse(ref BitArray array, int start, int length)
        {
            int mid = (length / 2);

            for (int i = start; i < start + mid; i++)
            {
                bool bit = array[i];
                array[i] = array[start + start + length - i - 1];
                array[start + start + length - i - 1] = bit;
            }
        }

        private void WriteIntoBitArray(WriteableBitmap bitmap, ref BitArray bits)
        {
            int currentX = 0;
            int currentY = 0;
            int currentIndex = 0;

            // go throug the 16x16 tiles
            for (int j = 0; j < 16; ++j)
            {
                for (int i = 0; i < 16; ++i)
                {
                    int colorIndex = 0;
                    Dictionary<Color, int> colors = new Dictionary<Color, int>();

                    // read pixels in the 8x8 quadrant
                    for (int y = currentY; y < currentY + 8; ++y)
                    {
                        for (int x = currentX; x < currentX + 8; ++x)
                        {
                            Color color = bitmap.GetPixel(x, y);
                            color.A = 255;

                            if (!Colors.Black.Equals(color))
                            {
                                if (!colors.TryGetValue(color, out int value))
                                {
                                    if (colors.Count < 4)
                                    {
                                        value = colorIndex;

                                        colors.Add(color, colorIndex++);
                                    }
                                    else
                                    {
                                        value = -1;
                                    }
                                }

                                switch (value)
                                {
                                    case 0:
                                        bits[currentIndex] = true;
                                        break;
                                    case 1:
                                        bits[currentIndex + 64] = true;
                                        break;
                                    case 2:
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
}
