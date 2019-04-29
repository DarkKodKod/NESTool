using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Commands
{
    public class BuildProjectCommand : Command
    {
        private Dictionary<string, WriteableBitmap> _bitmapCache = new Dictionary<string, WriteableBitmap>();

        public override void Execute(object parameter)
        {
            BuildPatternTables();

            MessageBox.Show("Build completed!", "Build", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BuildPatternTables()
        {
            const int cells = 16 * 16;
            const int tables = 2;
            const int sizeCell = 16;

            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            BitArray outputBits = new BitArray(sizeCell * cells * tables);

            if (!string.IsNullOrEmpty(projectModel.Build.PatternTableSpriteId))
            {
                PatternTableModel model = ProjectFiles.GetModel<PatternTableModel>(projectModel.Build.PatternTableSpriteId);

                if (model != null)
                {
                    WriteableBitmap bitmap = PatternTableUtils.CreateImage(model, ref _bitmapCache);

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

            // Save bit array into output file
            // projectModel.Build.OutputFilePath
        }

        private void WriteIntoBitArray(WriteableBitmap bitmap, ref BitArray bits)
        {
            int currentX = 0;
            int currentY = 0;

            Color[] colors = new Color[4] { Color.FromRgb(0,0,0), Color.FromRgb(0, 0, 0), Color.FromRgb(0, 0, 0), Color.FromRgb(0, 0, 0) };

            //https://wiki.nesdev.com/w/index.php/PPU_pattern_tables

            // go throug the 16x16 tiles
            for (int j = 0; j < 16; ++j)
            {
                for (int i = 0; i < 16; ++i)
                {
                    // read pixels in the 8x8 quadrant
                    for (int x = currentX; x < currentX + 8; ++x)
                    {
                        for (int y = currentY; y < currentY + 8; ++y)
                        {
                            Color color = bitmap.GetPixel(x, y);
                        }
                    }

                    currentX += 8;
                }

                currentX = 0;
                currentY += 8;
            }
        }
    }
}
