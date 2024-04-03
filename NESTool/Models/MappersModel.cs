using ArchitectureLibrary.Model;
using NESTool.Enums;
using Nett;
using System.IO;
using System.Windows;

namespace NESTool.Models;

public class MappersModel : ISingletonModel
{
    public MapperModel[] Mappers { get; set; }

    private const string _mappersfileNameKey = "mappersFileName";
    private const string _versionKey = "mappersVersion";
    private readonly string _configFileName = "";
    private readonly int _version = 0;

    public MappersModel()
    {
        _configFileName = @".\" + (string)Application.Current.FindResource(_mappersfileNameKey) + Toml.FileExtension;
        _version = (int)Application.Current.FindResource(_versionKey);

        Mappers = new MapperModel[1];
    }

    public void Copy(MappersModel copy)
    {
        Mappers = copy.Mappers;
    }

    public void Load()
    {
        bool exists = File.Exists(_configFileName);

        if (exists)
        {
            Copy(Toml.ReadFile<MappersModel>(_configFileName));
        }
        else
        {
            CreateAndLoadDefault();
        }
    }

    private void LoadDefaultMappers()
    {
        Mappers[0] = new MapperModel()
        {
            Id = 0,
            Name = "None",
            PRG = new int[] { 16, 32 },
            CHR = new int[] { 8 },
            Mirroring = new MirroringType[] { MirroringType.Horizontal, MirroringType.Vertical }
        };
    }

    private void CreateAndLoadDefault()
    {
        LoadDefaultMappers();

        File.Create(_configFileName).Dispose();

        Toml.WriteFile(this, _configFileName);
    }
}
