﻿using ArchitectureLibrary.Model;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Utils;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.IO;

namespace NESTool.Building
{
    public static class BackgroundsBuilding
    {
        private enum MetaType : int
        {
            Snake = 1,
            Spider = 2,
            Skull = 3,
            JumpingSkull = 4,
            WhiteKey = 5,
            BlueKey = 6,
            RedKey = 7,
            Talisman = 9,
            Sword = 10,
            Crystal = 11,
            DoorWhiteElement = 14,
            DoorBlueElement = 15,
            DoorRedElement = 16,
            AddBrick = 17,
            AddChain = 19,
            AddLadderLeft = 20,
            AddLadderRight = 21,
            AddTopLadderLeft = 22,
            AddTopLadderRight = 23
        }

        public static void Execute()
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

                    PrintMapEntities(model, item, outputFile);
                }
            }
        }

        private static void FormatBytes(List<byte> data, StreamWriter outputFile, int rowSize)
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

        private static void PrintMapEntities(MapModel model, FileModelVO item, StreamWriter outputFile)
        {
            void WriteData(string type, int x, int y, int palette, int lowLevel, int upperLevel, bool isItem, bool isMapElement)
            {
                // name
                outputFile.Write(type);
                outputFile.Write(", ");

                // write PPU name table addresses
                int dec = /*PPU address = $2000*/8192 + (32 * y) + x;
                string str = $"{dec:X4}";

                string strLow = str.Substring(2, 2);
                string strHigh = str.Substring(0, 2);

                outputFile.Write($"${strLow}, ${strHigh}, ");

                // write PPU attribute table addresses
                int bigCellPosX = (int)Math.Floor(x / 32.0f * 8.0f);
                int bigCellPosY = (int)Math.Floor(y / 32.0f * 8.0f);

                if (!isMapElement)
                {
                    dec = /*PPU address = $23C0*/9152 + (8 * bigCellPosY) + bigCellPosX;
                    str = $"{dec:X4}";

                    strLow = str.Substring(2, 2);
                    strHigh = str.Substring(0, 2);

                    outputFile.Write($"${strLow}, ${strHigh}, ");
                }

                // write x, y coordinates
                outputFile.Write($"${x:X2}, ${y:X2}, ");

                // check if is left or right and bottom or top
                int middleX = (4 * bigCellPosX) + 2;
                int middleY = (4 * bigCellPosY) + 2;

                if (isItem)
                {
                    int paletteIndex;

                    // to the right
                    if (x >= middleX)
                    {
                        // to the top
                        if (y < middleY)
                        {
                            paletteIndex = palette << 2;
                        }
                        else
                        // to the bottom
                        {
                            paletteIndex = palette << 6;
                        }
                    }
                    // to the left
                    else
                    {
                        // to the top
                        if (y < middleY)
                        {
                            paletteIndex = palette;
                        }
                        else
                        // to the bottom
                        {
                            paletteIndex = palette << 4;
                        }
                    }

                    outputFile.Write($"${paletteIndex:X2}, ");
                }

                // Levels
                outputFile.Write("(");
                outputFile.Write($"${lowLevel:X2}");
                outputFile.Write(" | ");
                outputFile.Write($"${upperLevel:X2}");
                outputFile.Write("<<4)");
            }

            void WriteEnemyData(string type, int x, int y, int att, int lowLevel, int upperLevel)
            {
                // name
                outputFile.Write(type);
                outputFile.Write(", ");
                // X
                outputFile.Write($"${x:X2}");
                outputFile.Write(", ");
                // Y
                outputFile.Write($"${y:X2}");
                outputFile.Write(", ");
                // Attributes
                outputFile.Write($"${att:X2}");
                outputFile.Write(", ");
                // Levels
                outputFile.Write("(");
                outputFile.Write($"${lowLevel:X2}");
                outputFile.Write(" | ");
                outputFile.Write($"${upperLevel:X2}");
                outputFile.Write("<<4)");
            }

            if (model.Entities.Count == 0)
            {
                return;
            }

            outputFile.WriteLine($"metadata_{item.Name}:");

            foreach (Models.Entity entity in model.Entities)
            {
                EntityModel entityModel = ProjectFiles.GetModel<EntityModel>(entity.EntityID);

                if (entityModel == null)
                {
                    continue;
                }

                outputFile.Write("    .byte ");

                string typeName = Enum.GetName(typeof(MetaType), entityModel.EntityId);

                int minLevel = int.Parse(model.GetPropertyValue(entity, "MinLevel"));
                int maxLevel = int.Parse(model.GetPropertyValue(entity, "MaxLevel"));

                switch ((MetaType)entityModel.EntityId)
                {
                    case MetaType.Snake:
                    case MetaType.Spider:
                    case MetaType.Skull:
                    case MetaType.JumpingSkull:
                        {
                            string attributes = model.GetPropertyValue(entity, "Attributes");

                            WriteEnemyData(typeName, entity.X, entity.Y, int.Parse(attributes), minLevel, maxLevel);
                        }
                        break;
                    case MetaType.WhiteKey:
                    case MetaType.BlueKey:
                    case MetaType.RedKey:
                    case MetaType.Talisman:
                    case MetaType.Sword:
                    case MetaType.Crystal:
                        {
                            string paletteIndex = model.GetPropertyValue(entity, "PaletteIndex");

                            WriteData(typeName, entity.X, entity.Y, int.Parse(paletteIndex), minLevel, maxLevel, isItem: true, isMapElement: false);
                        }
                        break;
                    case MetaType.DoorWhiteElement:
                    case MetaType.DoorBlueElement:
                    case MetaType.DoorRedElement:
                        {
                            WriteData(typeName, entity.X, entity.Y, 0, minLevel, maxLevel, isItem: false, isMapElement: false);
                        }
                        break;
                    case MetaType.AddBrick:
                    case MetaType.AddChain:
                    case MetaType.AddLadderLeft:
                    case MetaType.AddLadderRight:
                    case MetaType.AddTopLadderLeft:
                    case MetaType.AddTopLadderRight:
                        WriteData(typeName, entity.X, entity.Y, 0, minLevel, maxLevel, isItem: false, isMapElement: true);
                        break;
                }

                outputFile.Write(Environment.NewLine);
            }

            // Add always a null terminator
            outputFile.Write("    .byte $00");
            outputFile.Write(Environment.NewLine);
        }

        private static void SerializeNametable(MapModel model, ref List<byte> serializedData)
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

        private static void SerializeAttributes(MapModel model, ref List<byte> serializedData)
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
    }
}
