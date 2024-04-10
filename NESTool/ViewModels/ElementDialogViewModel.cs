using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels;

public class ElementDialogViewModel : ViewModel
{
    public DispatchSignalCommand<CloseDialogSignal> CloseDialogCommand { get; } = new();
    public CreateElementCommand CreateElementCommand { get; } = new();

    public List<ElementTypeModel> ElementTypes { get; set; } = [];

    public ElementTypeModel? SelectedType
    {
        get => _selectedType;
        set
        {
            _selectedType = value;
            OnPropertyChanged("SelectedType");
        }
    }

    private const string _folderBanksKey = "folderBanks";
    private const string _folderCharactersKey = "folderCharacters";
    private const string _folderMapsKey = "folderMaps";
    private const string _folderTileSetsKey = "folderTileSets";
    private const string _folderPalettesKey = "folderPalettes";
    private const string _folderWorldsKey = "folderWorlds";
    private const string _folderEntitiesKey = "folderEntities";

    private ElementTypeModel? _selectedType;

    public ElementDialogViewModel()
    {
        SignalManager.Get<CloseDialogSignal>().Listener += OnCloseDialog;

        InitializeElements();
    }

    private void OnCloseDialog()
    {
        SignalManager.Get<CloseDialogSignal>().Listener -= OnCloseDialog;
    }

    private void InitializeElements()
    {
        ProjectModel projectModel = ModelManager.Get<ProjectModel>();

        string banks = (string)Application.Current.FindResource(_folderBanksKey);
        string characters = (string)Application.Current.FindResource(_folderCharactersKey);
        string maps = (string)Application.Current.FindResource(_folderMapsKey);
        string tileSets = (string)Application.Current.FindResource(_folderTileSetsKey);
        string palettes = (string)Application.Current.FindResource(_folderPalettesKey);
        string worlds = (string)Application.Current.FindResource(_folderWorldsKey);
        string entity = (string)Application.Current.FindResource(_folderEntitiesKey);

        ElementTypes.Add(new ElementTypeModel()
        {
            Name = banks,
            Image = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/pattern_table.png", UriKind.RelativeOrAbsolute)),
            Type = ProjectItemType.Bank,
            Path = Path.Combine(projectModel.ProjectPath, banks)
        });
        ElementTypes.Add(new ElementTypeModel()
        {
            Name = characters,
            Image = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/character.png", UriKind.RelativeOrAbsolute)),
            Type = ProjectItemType.Character,
            Path = Path.Combine(projectModel.ProjectPath, characters)
        });
        ElementTypes.Add(new ElementTypeModel()
        {
            Name = maps,
            Image = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/map.png", UriKind.RelativeOrAbsolute)),
            Type = ProjectItemType.Map,
            Path = Path.Combine(projectModel.ProjectPath, maps)
        });
        ElementTypes.Add(new ElementTypeModel()
        {
            Name = tileSets,
            Image = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/tileset.png", UriKind.RelativeOrAbsolute)),
            Type = ProjectItemType.TileSet,
            Path = Path.Combine(projectModel.ProjectPath, tileSets)
        });
        ElementTypes.Add(new ElementTypeModel()
        {
            Name = palettes,
            Image = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/palette.png", UriKind.RelativeOrAbsolute)),
            Type = ProjectItemType.Palette,
            Path = Path.Combine(projectModel.ProjectPath, palettes)
        });
        ElementTypes.Add(new ElementTypeModel()
        {
            Name = worlds,
            Image = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/world.png", UriKind.RelativeOrAbsolute)),
            Type = ProjectItemType.World,
            Path = Path.Combine(projectModel.ProjectPath, worlds)
        });
        ElementTypes.Add(new ElementTypeModel()
        {
            Name = entity,
            Image = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/entity.png", UriKind.RelativeOrAbsolute)),
            Type = ProjectItemType.Entity,
            Path = Path.Combine(projectModel.ProjectPath, entity)
        });
    }
}
