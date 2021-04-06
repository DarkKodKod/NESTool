﻿using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Model;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Utils;
using NESTool.VOs;
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
        private const int NESFPS = 60;

        public override void Execute(object parameter)
        {
            if (!CheckValidOutputFolder())
            {
                MessageBox.Show("Invalid output folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BuildPalettes();
            BuildBanks();
            BuildMetaSprites();
            BuildBackgrounds();
            BuildTilesDefinition();

            MessageBox.Show("Build completed!", "Build", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CheckValidOutputFolder()
        {
            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            try
            {
                string result = Path.GetFullPath(projectModel.Build.OutputFilePath);

                return Directory.Exists(result);
            }
            catch
            {
                return false;
            }
        }

        private void BuildPalettes()
        {
            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            string fullPath = Path.Combine(Path.GetFullPath(projectModel.Build.OutputFilePath), "palettes.s");

			using (StreamWriter outputFile = new StreamWriter(fullPath))
            {
				List<FileModelVO> paletteModelVOs = ProjectFiles.GetModels<PaletteModel>();

                outputFile.WriteLine("; This file is autogenerated!");

                SortedDictionary<string, StringWriter> pals = new SortedDictionary<string, StringWriter>();

                foreach (FileModelVO vo in paletteModelVOs)
				{
					PaletteModel model = vo.Model as PaletteModel;

					string name = "palette_" + vo.Name.Replace(' ', '_').ToLower();

					Color color0 = Util.GetColorFromInt(model.Color0);
					Color color1 = Util.GetColorFromInt(model.Color1);
					Color color2 = Util.GetColorFromInt(model.Color2);
					Color color3 = Util.GetColorFromInt(model.Color3);

                    StringWriter stringWriter = new StringWriter();

                    stringWriter.Write($"    .byte ");
                    stringWriter.Write($"${Util.ColorToColorHex(color0)},");
                    stringWriter.Write($"${Util.ColorToColorHex(color1)},");
                    stringWriter.Write($"${Util.ColorToColorHex(color2)},");
                    stringWriter.Write($"${Util.ColorToColorHex(color3)}");

                    pals.Add(name, stringWriter);
                }

                foreach (var item in pals)
                {
                    outputFile.WriteLine("");
                    outputFile.WriteLine($"{item.Key}:");

                    outputFile.Write(item.Value.ToString());
                    outputFile.Write(Environment.NewLine);
                }
			}
        }

        private void BuildTilesDefinition()
        {
            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            string fullPath = Path.Combine(Path.GetFullPath(projectModel.Build.OutputFilePath), "tiles_definition.s");

            List<string> namesUsed = new List<string>();

            using (StreamWriter outputFile = new StreamWriter(fullPath))
            {
                List<FileModelVO> banksModelVOs = ProjectFiles.GetModels<BankModel>();

                outputFile.WriteLine("; This file is autogenerated!");
                outputFile.WriteLine("");

                foreach (FileModelVO vo in banksModelVOs)
                {
                    BankModel model = vo.Model as BankModel;

                    if (model.BankUseType != BankUseType.Background)
                    {
                        continue;
                    }

                    for (int i = 0; i < model.PTTiles.Length; i++)
                    {
                        PTTileModel tile = model.PTTiles[i];

                        FileModelVO tileSetModelVo = ProjectFiles.GetFileModel(tile.TileSetID);

                        TileSetModel tileSetModel = tileSetModelVo.Model as TileSetModel;

                        if (tileSetModelVo != null && tileSetModel != null)
                        {
                            string hexX = $"{(int)tile.Point.X:X2}";
                            string hexY = $"{(int)tile.Point.Y:X2}";

                            int index = tileSetModel.GetIndexFromPosition(tile.Point);

                            string tilePseudo = tileSetModel.TilePseudonyms[index];

                            string name;

                            if (string.IsNullOrEmpty(tilePseudo))
                            {
                                name = $"{tileSetModelVo.Name}_{hexX}_{hexY}";
                            }
                            else
                            {
                                name = tilePseudo;
                            }

                            if (!namesUsed.Contains(name))
                            {
                                outputFile.Write($"{name} = ${i:X2}{Environment.NewLine}");

                                namesUsed.Add(name);
                            }
                        }
                    }
                }
            }
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
                    outputFile.WriteLine("; This file is autogenerated!");
                    outputFile.Write(Environment.NewLine);
                    outputFile.WriteLine($"nt_{item.Name}:");

                    List<byte> serializedMap = new List<byte>();

                    SerializeNametable(model, ref serializedMap);

                    if (projectModel.Build.UseRLEOnMaps)
                    {
                        RLE.Compress(serializedMap, out List<byte> compressedData);

                        serializedMap = compressedData;
                    }

                    FormatBytes(serializedMap, outputFile, 32);

                    serializedMap.Clear();

                    if (model.ExportAttributeTable)
                    {
                        outputFile.WriteLine($"att_{item.Name}:");

                        SerializeAttributes(model, ref serializedMap);

                        if (projectModel.Build.UseRLEOnMaps)
                        {
                            RLE.Compress(serializedMap, out List<byte> compressedData);

                            serializedMap = compressedData;
                        }

                        FormatBytes(serializedMap, outputFile, 8);
                    }

                    PrintMetaData(model, item, outputFile);
                }
            }
        }

        private void FormatBytes(List<byte> data, StreamWriter outputFile, int rowSize)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (i % rowSize == 0 && i < data.Count)
                {
                    if (i > 0)
                    {
                        outputFile.Write(Environment.NewLine);
                    }

                    outputFile.Write("    .byte ");
                }

                outputFile.Write($"${data[i]:X2}");

                if (i < data.Count - 1 && i % rowSize != rowSize - 1)
                {
                    outputFile.Write(",");
                }
            }

            outputFile.Write(Environment.NewLine);
            outputFile.Write(Environment.NewLine);
        }

        private void PrintMetaData(MapModel model, FileModelVO item, StreamWriter outputFile)
        {
            if (!string.IsNullOrEmpty(model.MetaData))
            {
                outputFile.WriteLine($"metadata_{item.Name}:");

                string[] lines = model.MetaData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                for (int i = 0; i < lines.Length; ++i)
                {
                    string line = lines[i].Trim();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    string[] bytes = line.Split(new[] { "," }, StringSplitOptions.None);

                    outputFile.Write("    .byte ");

                    bool mapElement = false;
                    int cacheX = 0;
                    int cacheY = 0;
                    int bigCellPosX = 0;
                    int bigCellPosY = 0;
                    int type = 0;

                    for (int j = 0; j < bytes.Length; ++j)
                    {
                        if (!int.TryParse(bytes[j], out int nValue))
                        {
                            continue;
                        }

                        // This is not generic at all, but I need it for my game
                        if (j == 0)
                        {
                            type = nValue;

                            if (nValue >= 5 && nValue <= 14)
                            {
                                mapElement = true;
                            }
                        }

                        if (mapElement && j == 1)
                        {
                            cacheX = nValue;
                        }
                        else if (mapElement && j == 2)
                        {
                            cacheY = nValue;

                            // write PPU name table addresses
                            int dec = 8192 + (32 * nValue) + cacheX;
                            string str = $"{dec:X4}";

                            string strLow = str.Substring(2, 2);
                            string strHigh = str.Substring(0, 2);
                            
                            outputFile.Write($"${strLow}, ${strHigh}, ");

                            if (type != 13 /* 13 = "add element" type */)
                            {
                                // write PPU attribute table addresses
                                bigCellPosX = (int)Math.Floor(cacheX / 32.0f * 8.0f);
                                bigCellPosY = (int)Math.Floor(nValue / 32.0f * 8.0f);

                                dec = /*PPU address = $23C0*/9152 + (8 * bigCellPosY) + bigCellPosX;
                                str = $"{dec:X4}";

                                strLow = str.Substring(2, 2);
                                strHigh = str.Substring(0, 2);

                                outputFile.Write($"${strLow}, ${strHigh}, ");
                            }

                            // write x, y coordinates
                            outputFile.Write($"${cacheX:X2}, ${nValue:X2}, ");
                        }
                        else if (mapElement && j == 3 && (type == 14 || type == 13) /* 14 = "Door", 13 = "add element" */)
                        {
                            outputFile.Write($"${nValue:X2}, ");
                        }
                        else if (mapElement && j == 3 && type != 14 /* 14 = "Door" */)
                        {
                            // check if is left or right and bottom or top
                            int middleX = (4 * bigCellPosX) + 2;
                            int middleY = (4 * bigCellPosY) + 2;

                            int paletteIndex;

                            // to the right
                            if (cacheX >= middleX)
                            {
                                // to the top
                                if (cacheY < middleY)
                                {
                                    paletteIndex = nValue << 2;
                                }
                                else
                                // to the bottom
                                {
                                    paletteIndex = nValue << 6;
                                }
                            }
                            // to the left
                            else
                            {
                                // to the top
                                if (cacheY < middleY)
                                {
                                    paletteIndex = nValue;
                                }
                                else
                                // to the bottom
                                {
                                    paletteIndex = nValue << 4;
                                }
                            }    

                            outputFile.Write($"${paletteIndex:X2}, ");
                        }
                        else
                        {
                            outputFile.Write($"${nValue:X2}");

                            if (j < bytes.Length - 1)
                            {
                                outputFile.Write(", ");
                            }
                        }
                    }

                    outputFile.Write(Environment.NewLine);
                }

                // Add always a null terminator
                outputFile.Write("    .byte $00");
                outputFile.Write(Environment.NewLine);
            }
        }

        private void SerializeNametable(MapModel model, ref List<byte> serializedData)
        {
            // iterate vertically
            for (int j = 0; j < 15; ++j)
            {
                // each meta tile has two rows, so we have to iterate same index twice
                for (int k = 0; k < 2; ++k)
                {
                    // iterate horizontally
                    for (int i = 0; i < 16; ++i)
                    {
                        MapTile[] mapTile = model.AttributeTable[i + (j * 16)].MapTile;

                        if (mapTile == null)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(mapTile[0 + (k * 2)].BankID) ||
                            string.IsNullOrEmpty(mapTile[0 + (k * 2)].BankTileID) ||
                            string.IsNullOrEmpty(mapTile[1 + (k * 2)].BankID) ||
                            string.IsNullOrEmpty(mapTile[1 + (k * 2)].BankTileID))
                        {
                            continue;
                        }

                        SerializeMapTile(ref serializedData, mapTile[0 + (k * 2)]);
                        SerializeMapTile(ref serializedData, mapTile[1 + (k * 2)]);
                    }
                }
            }

            void SerializeMapTile(ref List<byte> serialized, MapTile mapTile)
            {
                BankModel bank = ProjectFiles.GetModel<BankModel>(mapTile.BankID);

                if (bank == null)
                {
                    serialized.Add(0);
                }
                else
                {
                    byte tile = (byte)bank.GetTileIndex(mapTile.BankTileID);

                    serialized.Add(tile);
                }
            }
        }

        private void SerializeAttributes(MapModel model, ref List<byte> serializedData)
        {
            const int MaxHorizontal = 8;
            const int MaxVertical = 8;

            for (int j = 0; j < MaxVertical; ++j)
            {
                for (int i = 0; i < MaxHorizontal; ++i)
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

                    // 7654 3210
                    // |||| ||++- Color bits 3 - 2 for top left quadrant of this byte
                    // |||| ++--- Color bits 3 - 2 for top right quadrant of this byte
                    // ||++------ Color bits 3 - 2 for bottom left quadrant of this byte
                    // ++-------- Color bits 3 - 2 for bottom right quadrant of this byte

                    byte attribute = (byte)((bottom_right << 6) | (bottom_left << 4) | (top_right << 2) | top_left);

                    serializedData.Add(attribute);
                }
            }
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
                            outputFile.WriteLine(";          vert   tile   attr   horiz");

                            foundFrame = true;
                        }

                        byte horiz = (byte)charTile.Point.X;
                        byte vert = (byte)charTile.Point.Y;

                        BankModel bank = ProjectFiles.GetModel<BankModel>(charTile.BankID);
                        byte tile = (byte)bank.GetTileIndex(charTile.BankTileID);

                        int paletteIndex = charTile.PaletteIndex;

                        byte attrs = (byte)paletteIndex;
                        attrs |= charTile.BackBackground ? (byte)32 : (byte)0;
                        attrs |= charTile.FlipX ? (byte)64 : (byte)0;
                        attrs |= charTile.FlipY ? (byte)128 : (byte)0;

                        outputFile.WriteLine($"    .byte   ${vert:X2},   ${tile:X2},   ${attrs:X2},   ${horiz:X2}");
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

                int colBoxAx = animation.CollisionInfo == null ? 0 : animation.CollisionInfo.OffsetX;
                int colBoxAy = animation.CollisionInfo == null ? 0 : animation.CollisionInfo.OffsetY;
                int colBoxBx = animation.CollisionInfo == null ? 0 : animation.CollisionInfo.OffsetX + animation.CollisionInfo.Width;
                int colBoxCy = animation.CollisionInfo == null ? 0 : animation.CollisionInfo.OffsetY + animation.CollisionInfo.Height;

                outputFile.WriteLine($"    ; number of frames");
                outputFile.WriteLine($"    .byte ${frameNames.Count:X2} ; decimal {frameNames.Count}");
                outputFile.WriteLine($"    ; frame duration");
                outputFile.WriteLine($"    .byte ${frameDuration:X2} ; decimal {frameDuration}");
                outputFile.WriteLine($"    ; collision");
                outputFile.WriteLine($"    ; a____b");
                outputFile.WriteLine($"    ; |    |");
                outputFile.WriteLine($"    ; |____|");
                outputFile.WriteLine($"    ; c    d");
                outputFile.WriteLine($"    ;        a       b     c");
                outputFile.WriteLine($"    ;      x    y    x     y");
                outputFile.Write($"    .byte ");
                outputFile.Write($"${colBoxAx:X2}, ");
                outputFile.Write($"${colBoxAy:X2}, ");
                outputFile.Write($"${colBoxBx:X2}, ");
                outputFile.Write($"${colBoxCy:X2}");
                outputFile.Write(Environment.NewLine);

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
                    string nameUpper = name.ToUpper();

                    outputFile.WriteLine($"ANIM_{nameUpper}_{index} = ${i:X2}");
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

        private void BuildBanks()
        {
            ProjectModel projectModel = ModelManager.Get<ProjectModel>();

            string outputPath = Path.GetFullPath(projectModel.Build.OutputFilePath);

            List<FileModelVO> bankModelVOs = ProjectFiles.GetModels<BankModel>();

            const int cellSizeInBytes = 128; // 16 bytes each cell

            foreach (FileModelVO vo in bankModelVOs)
            {
                BankModel bank = vo.Model as BankModel;

                int cellsCount = 0;

                switch (bank.BankSize)
                {
                    case BankSize.Size4Kb: cellsCount = 16 * 16; break;
                    case BankSize.Size2Kb: cellsCount = 16 * 8; break;
                    case BankSize.Size1Kb: cellsCount = 16 * 4; break;
                }

                Dictionary<string, WriteableBitmap> bitmapCache = new Dictionary<string, WriteableBitmap>();

                BitArray outputBits = new BitArray(cellSizeInBytes * cellsCount);
                outputBits.SetAll(false);

                int currentIndex = 0;

                WriteableBitmap bitmap = BanksUtils.CreateImage(bank, ref bitmapCache, false);

                using (bitmap.GetBitmapContext())
                {
                    WriteIntoBitArray(bank, bitmap, ref outputBits, ref currentIndex);
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

        private void WriteIntoBitArray(BankModel model, WriteableBitmap bitmap, ref BitArray bits, ref int currentIndex)
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
                        colors = new Dictionary<Color, int>
                        {
                            { Util.NullColor, 0 }
                        };

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
}
