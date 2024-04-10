using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels;

public class CellGroup : INotifyPropertyChanged
{
    public int[] _items = new int[16 * 16];

    public CellGroup()
    {
        for (int i = 0; i < _items.Length; ++i)
        {
            _items[i] = -1;
        }
    }

    public int this[int i]
    {
        get => _items[i];
        set
        {
            _items[i] = value;

            OnPropertyChanged(Binding.IndexerName);
        }
    }

    protected virtual void OnPropertyChanged(string propname)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public class BanksViewModel : ItemViewModel
{
    private double _actualWidth;
    private double _actualHeight;
    private Point _croppedPoint;
    private bool _doNotSave = false;
    private BankUseType _selectedBankUseType = BankUseType.None;
    private BankSize _selectedBankSize = BankSize.Size4Kb;
    private WriteableBitmap? _croppedImage;
    private string _selectedGroup = string.Empty;
    private string _selectedIndex = string.Empty;
    private FileModelVO[]? _tileSets;
    private int _selectedTileSet;
    private int _selectedBankTile;
    private ImageSource? _imgSource;
    private ImageSource? _pTImage;
    private Visibility _rectangleVisibility = Visibility.Hidden;
    private double _rectangleTop = 0.0;
    private double _rectangleLeft = 0.0;
    private Visibility _selectionRectangleVisibility = Visibility.Hidden;
    private double _selectionRectangleTop = 0.0;
    private double _selectionRectangleLeft = 0.0;
    private BankModel? _model = null;
    private Dictionary<string, WriteableBitmap> _bitmapCache = [];
    private Visibility _groupMarkVisible1k = Visibility.Hidden;
    private Visibility _groupMarkVisible2k = Visibility.Hidden;
    private Visibility _groupMarkVisible4k = Visibility.Hidden;
    private int _canvasHeght = 128;
    private SpriteSize _spriteSize = SpriteSize.s8x8;

    #region Commands
    public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
    public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
    public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
    public MoveTileToBankCommand MoveTileToBankCommand { get; } = new MoveTileToBankCommand();
    public DeleteBankTileCommand DeleteBankTileCommand { get; } = new DeleteBankTileCommand();
    #endregion

    #region get/set
    public int CanvasHeight
    {
        get => _canvasHeght;
        set
        {
            _canvasHeght = value;

            OnPropertyChanged("CanvasHeight");
        }
    }

    public SpriteSize SpriteSize
    {
        get => _spriteSize;
        set
        {
            if (_spriteSize != value)
            {
                _spriteSize = value;

                OnPropertyChanged("SpriteSize");

                Save();
            }
        }
    }

    public CellGroup CellGroup { get; set; } = new CellGroup();

    public string SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                _selectedIndex = value;

                OnPropertyChanged("SelectedIndex");
            }
        }
    }

    public Visibility GroupMarkVisible1k
    {
        get => _groupMarkVisible1k;
        set
        {
            _groupMarkVisible1k = value;

            OnPropertyChanged("GroupMarkVisible1k");
        }
    }

    public Visibility GroupMarkVisible2k
    {
        get => _groupMarkVisible2k;
        set
        {
            _groupMarkVisible2k = value;

            OnPropertyChanged("GroupMarkVisible2k");
        }
    }

    public Visibility GroupMarkVisible4k
    {
        get => _groupMarkVisible4k;
        set
        {
            _groupMarkVisible4k = value;

            OnPropertyChanged("GroupMarkVisible4k");
        }
    }

    public string SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (_selectedGroup != value)
                {
                    if (int.TryParse(value, out int intValue))
                    {
                        CellGroup[SelectedBankTile] = intValue;

                        BankModel? model = Model;

                        if (model != null)
                            model.PTTiles[SelectedBankTile].Group = intValue;

                        ProjectItem?.FileHandler.Save();
                    }
                }

                _selectedGroup = value;

                OnPropertyChanged("SelectedGroup");
            }
        }
    }

    public BankSize SelectedBankSize
    {
        get => _selectedBankSize;
        set
        {
            if (_selectedBankSize != value)
            {
                _selectedBankSize = value;

                OnPropertyChanged("SelectedBankSize");

                AdjustCanvasHeight();
                SetMarksVisibility();

                Save();
            }
        }
    }

    public BankUseType SelectedBankUseType
    {
        get => _selectedBankUseType;
        set
        {
            if (_selectedBankUseType != value)
            {
                _selectedBankUseType = value;

                OnPropertyChanged("SelectedBankUseType");

                Save();
            }
        }
    }

    public double RectangleLeft
    {
        get => _rectangleLeft;
        set
        {
            _rectangleLeft = value;

            OnPropertyChanged("RectangleLeft");
        }
    }

    public double RectangleTop
    {
        get => _rectangleTop;
        set
        {
            _rectangleTop = value;

            OnPropertyChanged("RectangleTop");
        }
    }

    public Visibility RectangleVisibility
    {
        get => _rectangleVisibility;
        set
        {
            _rectangleVisibility = value;

            OnPropertyChanged("RectangleVisibility");
        }
    }

    public double SelectionRectangleLeft
    {
        get => _selectionRectangleLeft;
        set
        {
            _selectionRectangleLeft = value;

            OnPropertyChanged("SelectionRectangleLeft");
        }
    }

    public double SelectionRectangleTop
    {
        get => _selectionRectangleTop;
        set
        {
            _selectionRectangleTop = value;

            OnPropertyChanged("SelectionRectangleTop");
        }
    }

    public Visibility SelectionRectangleVisibility
    {
        get => _selectionRectangleVisibility;
        set
        {
            _selectionRectangleVisibility = value;

            OnPropertyChanged("SelectionRectangleVisibility");
        }
    }

    public ImageSource? PTImage
    {
        get => _pTImage;
        set
        {
            _pTImage = value;

            OnPropertyChanged("PTImage");
        }
    }

    public ImageSource? ImgSource
    {
        get => _imgSource;
        set
        {
            _imgSource = value;

            OnPropertyChanged("ImgSource");
        }
    }

    public Point CroppedPoint
    {
        get => _croppedPoint;
        set
        {
            _croppedPoint = value;

            OnPropertyChanged("CroppedPoint");
        }
    }

    public double ActualHeight
    {
        get => _actualHeight;
        set
        {
            _actualHeight = value;

            OnPropertyChanged("ActualHeight");
        }
    }

    public double ActualWidth
    {
        get => _actualWidth;
        set
        {
            _actualWidth = value;

            OnPropertyChanged("ActualWidth");
        }
    }

    public WriteableBitmap? CroppedImage
    {
        get => _croppedImage;
        set
        {
            _croppedImage = value;

            OnPropertyChanged("CroppedImage");
        }
    }

    public FileModelVO[]? TileSets
    {
        get => _tileSets;
        set
        {
            _tileSets = value;

            OnPropertyChanged("TileSets");
        }
    }

    public int SelectedTileSet
    {
        get => _selectedTileSet;
        set
        {
            _selectedTileSet = value;

            OnPropertyChanged("SelectedTileSet");
        }
    }

    public int SelectedBankTile
    {
        get => _selectedBankTile;
        set
        {
            _selectedBankTile = value;

            OnPropertyChanged("SelectedBankTile");
        }
    }

    public BankModel? Model
    {
        get
        {
            if (_model == null)
            {
                if (ProjectItem?.FileHandler.FileModel is BankModel model)
                {
                    Model = model;
                }
            }

            return _model;
        }
        set
        {
            _model = value;

            OnPropertyChanged("Model");
        }
    }
    #endregion

    public BanksViewModel()
    {
        UpdateDialogInfo();
    }

    private void OnSelectTileSet(string id)
    {
        int index = 0;

        if (TileSets == null)
            return;

        foreach (FileModelVO tileset in TileSets)
        {
            if (tileset.Model?.GUID == id)
            {
                SelectedTileSet = index;
                break;
            }

            index++;
        }
    }

    private void OnBankImageUpdated()
    {
        if (!IsActive)
        {
            return;
        }

        ProjectItem?.FileHandler.Save();

        LoadImage();

        if (Model != null)
        {
            for (int i = 0; i < Model.PTTiles.Length; i++)
            {
                CellGroup[i] = Model.PTTiles[i].Group;
            }
        }
    }

    private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
    {
        if (!IsActive)
        {
            return;
        }

        LoadTileSetImage();
    }

    private void OnProjectConfigurationSaved()
    {
        UpdateDialogInfo();
    }

    private void UpdateDialogInfo()
    {
        TileSets = ProjectFiles.GetModels<TileSetModel>().ToArray();
    }

    public override void OnActivate()
    {
        base.OnActivate();

        #region Signals
        SignalManager.Get<MouseWheelSignal>().Listener += OnMouseWheel;
        SignalManager.Get<OutputSelectedQuadrantSignal>().Listener += OnOutputSelectedQuadrant;
        SignalManager.Get<ProjectConfigurationSavedSignal>().Listener += OnProjectConfigurationSaved;
        SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener += OnFileModelVOSelectionChanged;
        SignalManager.Get<BankImageUpdatedSignal>().Listener += OnBankImageUpdated;
        SignalManager.Get<SelectTileSetSignal>().Listener += OnSelectTileSet;
        SignalManager.Get<BankTileDeletedSignal>().Listener += OnBankTileDeleted;
        SignalManager.Get<ShowGroupMarksSignal>().Listener += OnShowGroupMarks;
        SignalManager.Get<HideGroupMarksSignal>().Listener += OnHideGroupMarks;
        #endregion

        _doNotSave = true;

        if (Model != null)
        {
            SelectedBankUseType = Model.BankUseType;
            SelectedBankSize = Model.BankSize;

            AdjustCanvasHeight();
            SetMarksVisibility();

            SpriteSize = Model.SpriteSize;

            for (int i = 0; i < Model.PTTiles.Length; i++)
            {
                CellGroup[i] = Model.PTTiles[i].Group;
            }
        }

        _doNotSave = false;

        LoadTileSetImage();
        LoadImage();

        SelectedBankTile = -1;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        #region Signals
        SignalManager.Get<MouseWheelSignal>().Listener -= OnMouseWheel;
        SignalManager.Get<OutputSelectedQuadrantSignal>().Listener -= OnOutputSelectedQuadrant;
        SignalManager.Get<ProjectConfigurationSavedSignal>().Listener -= OnProjectConfigurationSaved;
        SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener -= OnFileModelVOSelectionChanged;
        SignalManager.Get<BankImageUpdatedSignal>().Listener -= OnBankImageUpdated;
        SignalManager.Get<SelectTileSetSignal>().Listener -= OnSelectTileSet;
        SignalManager.Get<BankTileDeletedSignal>().Listener -= OnBankTileDeleted;
        SignalManager.Get<ShowGroupMarksSignal>().Listener -= OnShowGroupMarks;
        SignalManager.Get<HideGroupMarksSignal>().Listener -= OnHideGroupMarks;
        #endregion
    }

    private void SetMarksVisibility()
    {
        if (SelectedBankSize == BankSize.Size4Kb)
        {
            GroupMarkVisible1k = MainWindow.ToolBarBanksShowHideGroupMarks ? Visibility.Visible : Visibility.Hidden;
            GroupMarkVisible2k = MainWindow.ToolBarBanksShowHideGroupMarks ? Visibility.Visible : Visibility.Hidden;
            GroupMarkVisible4k = MainWindow.ToolBarBanksShowHideGroupMarks ? Visibility.Visible : Visibility.Hidden;
        }
        else if (SelectedBankSize == BankSize.Size2Kb)
        {
            GroupMarkVisible1k = MainWindow.ToolBarBanksShowHideGroupMarks ? Visibility.Visible : Visibility.Hidden;
            GroupMarkVisible2k = MainWindow.ToolBarBanksShowHideGroupMarks ? Visibility.Visible : Visibility.Hidden;
            GroupMarkVisible4k = Visibility.Hidden;
        }
        else
        {
            GroupMarkVisible1k = MainWindow.ToolBarBanksShowHideGroupMarks ? Visibility.Visible : Visibility.Hidden;
            GroupMarkVisible2k = Visibility.Hidden;
            GroupMarkVisible4k = Visibility.Hidden;
        }
    }

    private void AdjustCanvasHeight()
    {
        switch (SelectedBankSize)
        {
            case BankSize.Size4Kb:
                CanvasHeight = 128;
                break;
            case BankSize.Size2Kb:
                CanvasHeight = 64;
                break;
            case BankSize.Size1Kb:
                CanvasHeight = 32;
                break;
        }
    }

    private void LoadTileSetImage()
    {
        if (TileSets?.Length == 0)
        {
            return;
        }

        RectangleVisibility = Visibility.Hidden;
        CroppedImage = null;

        if (TileSets == null)
        {
            return;
        }

        TileSetModel? model = TileSets[SelectedTileSet].Model as TileSetModel;

        if (model == null)
            return;

        ProjectModel projectModel = ModelManager.Get<ProjectModel>();

        string path = Path.Combine(projectModel.ProjectPath, model.ImagePath);

        if (File.Exists(path))
        {
            ImgSource = null;

            ActualWidth = model.ImageWidth;
            ActualHeight = model.ImageHeight;

            BitmapImage bmImage = new BitmapImage();

            bmImage.BeginInit();
            bmImage.CacheOption = BitmapCacheOption.OnLoad;
            bmImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bmImage.EndInit();
            bmImage.Freeze();

            ImgSource = bmImage;
        }
    }

    private void OnOutputSelectedQuadrant(Image sender, WriteableBitmap bitmap, Point point)
    {
        if (!IsActive)
        {
            return;
        }

        if (sender.Name == "imgBank")
        {
            SelectionRectangleVisibility = Visibility.Visible;
            SelectionRectangleLeft = point.X;
            SelectionRectangleTop = point.Y;

            int index = ((int)point.X / 8) + ((int)point.Y / 8 * 16);

            SelectedBankTile = index;

            BankModel? model = Model;

            if (model != null)
            {
                SelectedGroup = model.PTTiles[index].Group.ToString();
            }

            SelectedIndex = $"${index:X2}";
        }
        else if (sender.Name == "imgBig")
        {
            CroppedPoint = point;
            CroppedImage = bitmap;

            RectangleVisibility = Visibility.Visible;
            RectangleLeft = point.X;
            RectangleTop = point.Y;
        }
    }

    private void OnHideGroupMarks()
    {
        if (!IsActive)
        {
            return;
        }

        SetMarksVisibility();
    }

    private void OnShowGroupMarks()
    {
        if (!IsActive)
        {
            return;
        }

        SetMarksVisibility();
    }

    private void OnBankTileDeleted()
    {
        if (!IsActive)
        {
            return;
        }

        SignalManager.Get<CleanupTileSetLinksSignal>().Dispatch();

        _bitmapCache.Clear();

        OnBankImageUpdated();

        SelectionRectangleVisibility = Visibility.Hidden;
        SelectedBankTile = -1;

        if (Model != null)
        {
            for (int i = 0; i < Model.PTTiles.Length; i++)
            {
                CellGroup[i] = Model.PTTiles[i].Group;
            }
        }
    }

    private void OnMouseWheel(MouseWheelVO vo)
    {
        const double ScaleRate = 1.1;

        if (vo.Delta > 0)
        {
            ActualWidth *= ScaleRate;
            ActualHeight *= ScaleRate;
        }
        else
        {
            ActualWidth /= ScaleRate;
            ActualHeight /= ScaleRate;
        }
    }

    private void Save()
    {
        if (_doNotSave)
        {
            return;
        }

        if (Model == null)
        {
            return;
        }

        Model.BankUseType = SelectedBankUseType;
        Model.BankSize = SelectedBankSize;
        Model.Distribution = BankTileDistribution.Compact;
        Model.SpriteSize = SpriteSize;

        ProjectItem?.FileHandler.Save();
    }

    private void LoadImage()
    {
        if (Model == null)
        {
            return;
        }

        WriteableBitmap? bitmap = BanksUtils.CreateImage(Model, ref _bitmapCache);

        if (bitmap != null)
            PTImage = bitmap;
    }
}
