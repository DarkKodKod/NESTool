using Nett;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NESTool.Models
{
    public struct Entity
    {
        public string GUID { get; set; }
        public int SortIndex { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }

    public struct MapTile
    {
        public Point Point { get; set; }
        public string BankID { get; set; }
        public string BankTileID { get; set; }
    }

    public struct AttributeTable
    {
        public MapTile[] MapTile { get; set; }
        public int PaletteIndex { get; set; }
    }

    public class MapModel : AFileModel
    {
        private const string _extensionKey = "extensionMaps";
        private int[,] _indicesCache = null;

        [TomlIgnore]
        public static int MetaTileMax = 16 * 15; // 16 tiles horizontal * 15 tiles vertical

        private int[,] IndicesCache
        {
            get
            {
                if (_indicesCache == null)
                {
                    _indicesCache = new int[MetaTileMax, 4];

                    int iIndex = 0;
                    int jIndex = 0;
                    int index = 0;
                    int index2 = 32;

                    for (int i = 0; i < 30 * 32; ++i)
                    {
                        if (jIndex < 2)
                        {
                            _indicesCache[iIndex, jIndex] = index;
                            index++;
                        }
                        else
                        {
                            _indicesCache[iIndex, jIndex] = index2;
                            index2++;
                        }

                        jIndex++;

                        if (jIndex == 4)
                        {
                            jIndex = 0;
                            iIndex++;

                            if (iIndex % 16 == 0)
                            {
                                index += 32;
                                index2 += 32;
                            }
                        }
                    }
                }

                return _indicesCache;
            }
        }

        [TomlIgnore]
        public override string FileExtension
        {
            get
            {
                if (string.IsNullOrEmpty(_fileExtension))
                {
                    _fileExtension = (string)Application.Current.FindResource(_extensionKey);
                }

                return _fileExtension;
            }
        }

        public int[] GetMetaTableArray(int index)
        {
            if (index == -1)
            {
                return null;
            }

            return Enumerable.Range(0, IndicesCache.GetLength(1))
                             .Select(x => IndicesCache[index, x])
                             .ToArray();
        }

        // First number is: Meta table index
        // Second number is: Selected index inside the meta table
        public (int, int) GetAttributeTileIndex(int index)
        {
            for (int i = 0; i < IndicesCache.GetLength(0); ++i)
            {
                for (int j = 0; j < IndicesCache.GetLength(1); ++j)
                {
                    int value = IndicesCache[i, j];

                    if (value == index)
                    {
                        return (i, j);
                    }
                }
            }

            return (-1, -1);
        }

        public ref MapTile GetTile(int index)
        {
            (int, int) tuple = GetAttributeTileIndex(index);

            int attributeTableIndex = tuple.Item1;
            int tileIndex = tuple.Item2;

            if (AttributeTable[attributeTableIndex].MapTile == null)
            {
                AttributeTable[attributeTableIndex].MapTile = new MapTile[4];
            }

            return ref AttributeTable[attributeTableIndex].MapTile[tileIndex];
        }

        public AttributeTable[] AttributeTable { get; set; } = new AttributeTable[MetaTileMax];
        public string[] PaletteIDs { get; set; } = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty };
        public string MetaData { get; set; }
        public bool ExportAttributeTable { get; set; } = true;
        public List<Entity> Entities { get; set; } = new List<Entity>();
    }
}
