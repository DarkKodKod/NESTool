using ArchitectureLibrary.Model;
using NESTool.Enums;
using Nett;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace NESTool.Models;

public class MappersModel : ISingletonModel
{
    public List<MapperModel> Mappers { get; set; } = new(1);

    private const string _mappersfileNameKey = "mappersFileName";
    private const string _versionKey = "mappersVersion";
    private readonly string _configFileName = "";
    private readonly int _version = 0;

    public MappersModel()
    {
        _configFileName = @".\" + (string)Application.Current.FindResource(_mappersfileNameKey) + Toml.FileExtension;
        _version = (int)Application.Current.FindResource(_versionKey);
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
        Mappers.Add(new()
        {
            Id = 0,
            Name = "None",
            PRG = [16, 32],
            CHR = [8],
            Mirroring = [MirroringType.Horizontal, MirroringType.Vertical]
        });
    }

    private void CreateAndLoadDefault()
    {
        LoadDefaultMappers();

        File.Create(_configFileName).Dispose();

        Toml.WriteFile(this, _configFileName);
    }
}
