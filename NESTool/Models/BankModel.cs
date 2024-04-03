using NESTool.Enums;
using Nett;
using System.Windows;

namespace NESTool.Models;

public struct PTTileModel
{
    public string GUID { get; set; }
    public string TileSetID { get; set; }
    public Point Point { get; set; }
    public int Group { get; set; }
}

public class BankModel : AFileModel
{
    private const string _extensionKey = "extensionBanks";

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

    public BankSize BankSize { get; set; }
    public SpriteSize SpriteSize { get; set; } = SpriteSize.s8x8;
    public BankUseType BankUseType { get; set; }
    public BankTileDistribution Distribution { get; set; }
    public PTTileModel[] PTTiles { get; set; } = new PTTileModel[256];

    public bool IsFull()
    {
        int count = 0;
        int maxTilesPerBank = GetNumberTilesPerBank();

        foreach (PTTileModel tile in PTTiles)
        {
            if (string.IsNullOrEmpty(tile.GUID))
            {
                return false;
            }

            count++;

            if (count >= maxTilesPerBank)
            {
                break;
            }
        }

        return true;
    }

    public int GetTileIndex(string guid)
    {
        int count = 0;
        foreach (PTTileModel tile in PTTiles)
        {
            if (tile.GUID == guid)
            {
                return count;
            }

            count++;
        }

        return count;
    }

    public PTTileModel GetTileModel(string guid)
    {
        foreach (PTTileModel tile in PTTiles)
        {
            if (tile.GUID == guid)
            {
                return tile;
            }
        }

        // return empty
        return new PTTileModel();
    }

    private int GetNumberTilesPerBank()
    {
        switch (BankSize)
        {
            case BankSize.Size4Kb: return 256;
            case BankSize.Size2Kb: return 128;
            case BankSize.Size1Kb: return 64;
            default: return 0;
        }
    }

    public bool GetEmptyTileIndex(out int index)
    {
        int maxTilesPerBank = GetNumberTilesPerBank();
        int i = 0;

        foreach (PTTileModel tile in PTTiles)
        {
            if (string.IsNullOrEmpty(tile.GUID))
            {
                index = i;
                return true;
            }

            i++;

            if (i >= maxTilesPerBank)
            {
                break;
            }
        }

        index = -1;
        return false;
    }
}
