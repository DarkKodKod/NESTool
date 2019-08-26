using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Utils;
using NESTool.VOs;
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

        private const int NESFPS = 60;

        public BuildProjectCommand()
        {
            _patterntableOutputFile = (string)Application.Current.FindResource(_patterntableOutputFileKey);
        }

        public override void Execute(object parameter)
        {
            BuildPatternTables();
            BuildMetaSprites();
            BuildBackgrounds();

            MessageBox.Show("Build completed!", "Build", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BuildBackgrounds()
        {
            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            List<FileModelVO> models = ProjectFiles.GetModels<MapModel>();

            foreach (FileModelVO item in models)
            {
                MapModel model = item.Model as MapModel;

                string fullPath = Path.Combine(Path.GetFullPath(projectModel.Build.OutputFilePath), item.Name + ".s");

                using (StreamWriter outputFile = new StreamWriter(fullPath))
                {
                    WriteNametable(outputFile, model, item.Name);
                    WriteAttributes(outputFile, model, item.Name);
                }
            }
        }

        private void WriteNametable(StreamWriter outputFile, MapModel model, string name)
        {
            outputFile.WriteLine($"nt_{name}:");

            // iterate vertically
            for (int j = 0; j < 15; ++j)
            {
                // each meta tile has two rows, so we have to iterate same index twice
                for (int k = 0; k < 2; ++k)
                {
                    outputFile.Write("    .byte ");

                    // iterate horizontally
                    for (int i = 0; i < 16; ++i)
                    {
                        WriteMapTile(outputFile, model.AttributeTable[i + (j * 16)].MapTile[0 + (k * 2)]);
                        outputFile.Write(",");
                        WriteMapTile(outputFile, model.AttributeTable[i + (j * 16)].MapTile[1 + (k * 2)]);

                        if (i < 15)
                        {
                            outputFile.Write(",");
                        }
                    }

                    outputFile.Write("\n");
                }
            }

            outputFile.WriteLine("");
        }

        private void WriteMapTile(StreamWriter outputFile, MapTile mapTile)
        {
            PatternTableModel patternTable = ProjectFiles.GetModel<PatternTableModel>(mapTile.BankID);
            byte tile = (byte)patternTable.GetTileIndex(mapTile.BankTileID);

            outputFile.Write($"${tile.ToString("X2")}");
        }

        private void WriteAttributes(StreamWriter outputFile, MapModel model, string name)
        {
            outputFile.WriteLine($"att_{name}:");

            for (int j = 0; j < 8; ++j)
            {
                outputFile.Write("    .byte ");

                for (int i = 0; i < 8; ++i)
                {
                    byte top_left = (byte)model.AttributeTable[0 + (i * 2 + (j * 32))].PaletteIndex;
                    byte top_right = (byte)model.AttributeTable[1 + (i * 2 + (j * 32))].PaletteIndex;

                    byte bottom_left = 0;
                    byte bottom_right = 0;

                    int bottomLeftIndex = 16 + (i * 2 + (j * 32));
                    int bottomRightIndex = 17 + (i * 2 + (j * 32));

                    if (bottomLeftIndex < model.AttributeTable.Length)
                    {
                        bottom_left = (byte)model.AttributeTable[bottomLeftIndex].PaletteIndex;
                    }
                    
                    if (bottomRightIndex < model.AttributeTable.Length)
                    {
                        bottom_right = (byte)model.AttributeTable[bottomRightIndex].PaletteIndex;
                    }

                    byte attribute = (byte)((bottom_left << 3) | (bottom_right << 2) | (top_left << 1) | top_right);

                    outputFile.Write($"${attribute.ToString("X2")}");

                    if (i < 7)
                    {
                        outputFile.Write(",");
                    }
                }

                outputFile.Write("\n");
            }

            outputFile.WriteLine("");
        }

        private void BuildMetaSprites()
        {
            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            List<FileModelVO> models = ProjectFiles.GetModels<CharacterModel>();

            foreach (FileModelVO item in models)
            {
                CharacterModel model = item.Model as CharacterModel;

                string fullPath = Path.Combine(Path.GetFullPath(projectModel.Build.OutputFilePath), item.Name + ".s");

                using (StreamWriter outputFile = new StreamWriter(fullPath))
                {
                    WriteMetaSpriteHeader(outputFile);
                    WriteMetaSprites(outputFile, model, item.Name);
                }
            }
        }

        private void WriteMetaSprites(StreamWriter outputFile, CharacterModel model, string name)
        {
            List<string> animationIndices = new List<string>();

            foreach (CharacterAnimation animation in model.Animations)
            {
                if (string.IsNullOrEmpty(animation.ID))
                {
                    continue;
                }

                int frameIndex = 0;
                List<string> frameNames = new List<string>();

                for (int i = 0; i < animation.Frames.Length; ++i)
                {
                    if (animation.Frames[i].Tiles == null)
                    {
                        continue;
                    }

                    string frameName = $"{name}_{animation.Name}_data_{frameIndex++}";

                    bool foundFrame = false;

                    for (int j = 0; j < animation.Frames[i].Tiles.Length; ++j)
                    {
                        CharacterTile charTile = animation.Frames[i].Tiles[j];

                        if (string.IsNullOrEmpty(charTile.BankID) || string.IsNullOrEmpty(charTile.BankTileID))
                        {
                            continue;
                        }

                        if (foundFrame == false)
                        {
                            frameNames.Add(frameName);

                            outputFile.WriteLine($"{frameName}:");
                            outputFile.WriteLine(";          vert    tile    attr    horiz");

                            foundFrame = true;
                        }

                        byte horiz = (byte)charTile.Point.X;
                        byte vert = (byte)charTile.Point.Y;

                        PatternTableModel patternTable = ProjectFiles.GetModel<PatternTableModel>(charTile.BankID);
                        byte tile = (byte)patternTable.GetTileIndex(charTile.BankTileID);

                        byte attrs = (byte)charTile.PaletteIndex;
                        attrs |= charTile.FrontBackground ? (byte)0 : (byte)32;
                        attrs |= charTile.FlipX ? (byte)64 : (byte)0;
                        attrs |= charTile.FlipY ? (byte)128 : (byte)0;

                        outputFile.WriteLine($"    .byte   ${vert.ToString("X2")},    ${tile.ToString("X2")},	${attrs.ToString("X2")},	${horiz.ToString("X2")}");
                    }

                    if (foundFrame == true)
                    {
                        // Add the termination byte
                        outputFile.WriteLine("    .byte   $FF");
                        outputFile.WriteLine("");
                    }   
                }

                outputFile.WriteLine($"{name}_{animation.Name}_data:");

                animationIndices.Add($"{animation.Name}");

                int frameDuration = (int)(NESFPS * animation.Speed);

                outputFile.WriteLine($"    ; number of frames");
                outputFile.WriteLine($"    .byte ${frameNames.Count.ToString("X2")} ; decimal {frameNames.Count}");
                outputFile.WriteLine($"    ; frame duration");
                outputFile.WriteLine($"    .byte ${frameDuration.ToString("X2")} ; decimal {frameDuration}");

                foreach (string frameName in frameNames)
                {
                    outputFile.WriteLine($"    .addr {frameName}");
                }

                outputFile.WriteLine("");
            }

            if (animationIndices.Count > 0)
            {
                outputFile.WriteLine("; aninmation indices");

                for (int i = 0; i < animationIndices.Count; ++i)
                {
                    string index = animationIndices[i].ToUpper();

                    outputFile.WriteLine($"ANIM_{index} = {i}");
                }

                outputFile.WriteLine("");

                outputFile.WriteLine($"{name}_anim_num_map:");

                for (int i = 0; i < animationIndices.Count; ++i)
                {
                    string index = animationIndices[i];

                    outputFile.WriteLine($"    .addr {name}_{index}_data");
                }
            }
        }

        private void WriteMetaSpriteHeader(StreamWriter outputFile)
        {
            outputFile.WriteLine("; This file is autogenerated!");
            outputFile.WriteLine("");
            outputFile.WriteLine("; PPU OAM");
            outputFile.WriteLine("; http://wiki.nesdev.com/w/index.php/PPU_OAM");
            outputFile.WriteLine("");
            outputFile.WriteLine("; Byte 0 - Y position of top of sprite");
            outputFile.WriteLine("; Byte 1 - Tile index number");
            outputFile.WriteLine("; Byte 2 - Attributes");
            outputFile.WriteLine(";           76543210");
            outputFile.WriteLine(";           ||||||||");
            outputFile.WriteLine(";           ||||||++- Palette (4 to 7) of sprite");
            outputFile.WriteLine(";           |||+++--- Unimplemented");
            outputFile.WriteLine(";           ||+------ Priority (0: in front of background; 1: behind background)");
            outputFile.WriteLine(";           |+------- Flip sprite horizontally");
            outputFile.WriteLine(";           +-------- Flip sprite vertically");
            outputFile.WriteLine("; Byte 3 - X position of left side of sprite.");
            outputFile.WriteLine("");
        }

        private void BuildPatternTables()
        {
            const int cells = 16 * 16;
            const int tables = 2;
            const int sizeCell = 128; // 16 bytes each cell

            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            BitArray outputBits = new BitArray(sizeCell * cells * tables);
            outputBits.SetAll(false);

            int currentIndex = 0;

            if (!string.IsNullOrEmpty(projectModel.Build.PatternTableSpriteId))
            {
                PatternTableModel model = ProjectFiles.GetModel<PatternTableModel>(projectModel.Build.PatternTableSpriteId);

                if (model != null)
                {
                    WriteableBitmap bitmap = PatternTableUtils.CreateImage(model, ref _bitmapCache, false);

                    using (bitmap.GetBitmapContext())
                    {
                        WriteIntoBitArray(model, bitmap, ref outputBits, ref currentIndex);
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
                        WriteIntoBitArray(model, bitmap, ref outputBits, ref currentIndex);
                    }
                }
            }

            for (int i = 0; i < tables * sizeCell * cells;)
            {
                Reverse(ref outputBits, i, 8);
                i += 8;
            }

            byte[] bytes = new byte[outputBits.Length / 8];

            outputBits.CopyTo(bytes, 0);

            string fullPath = Path.Combine(Path.GetFullPath(projectModel.Build.OutputFilePath), _patterntableOutputFile);

            File.WriteAllBytes(fullPath, bytes);
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

        private void WriteIntoBitArray(PatternTableModel model, WriteableBitmap bitmap, ref BitArray bits, ref int currentIndex)
        {
            int currentX = 0;
            int currentY = 0;
            int matrixIndex = 0;

            Dictionary<int, Dictionary<Color, int>> groupedPalettes = new Dictionary<int, Dictionary<Color, int>>();

            // go throug the 16x16 tiles
            for (int j = 0; j < 16; ++j)
            {
                for (int i = 0; i < 16; ++i)
                {
                    int group = model.PTTiles[matrixIndex++].Group;

                    if (!groupedPalettes.TryGetValue(group, out Dictionary<Color, int> colors))
                    {
                        colors = new Dictionary<Color, int>();

                        groupedPalettes.Add(group, colors);
                    }

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
