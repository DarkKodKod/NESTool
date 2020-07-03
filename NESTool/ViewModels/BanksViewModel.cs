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

namespace NESTool.ViewModels
{
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
            get
            {
                return _items[i];
            }
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

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class BanksViewModel : ItemViewModel
    {
        private double _actualWidth;
        private double _actualHeight;
        private Point _croppedPoint;
        private PatternTableType _selectedPatternTableType = PatternTableType.Characters;
        private WriteableBitmap _croppedImage;
        private string _projectGridSize;
        private string _selectedGroup;
        private string _selectedIndex;
        private FileModelVO[] _tileSets;
        private int _selectedTileSet;
        private int _selectedPatternTableTile;
        private ImageSource _imgSource;
        private ImageSource _pTImage;
        private Visibility _rectangleVisibility = Visibility.Hidden;
        private double _rectangleTop = 0.0;
        private double _rectangleLeft = 0.0;
        private Visibility _selectionRectangleVisibility = Visibility.Hidden;
        private double _selectionRectangleTop = 0.0;
        private double _selectionRectangleLeft = 0.0;
        private BankModel _model = null;
        private Dictionary<string, WriteableBitmap> _bitmapCache = new Dictionary<string, WriteableBitmap>();
        private Visibility _groupMarkVisible = Visibility.Hidden;

        #region Commands
        public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        public MoveTileToBankCommand MoveTileToBankCommand { get; } = new MoveTileToBankCommand();
        public DeleteBankTileCommand DeleteBankTileCommand { get; } = new DeleteBankTileCommand();
        #endregion

        #region get/set
        public string ProjectGridSize
        {
            get
            {
                return _projectGridSize;
            }
            set
            {
                _projectGridSize = value;

                OnPropertyChanged("ProjectGridSize");
            }
        }

        public CellGroup CellGroup { get; set; } = new CellGroup();

        public string SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _selectedIndex = value;

                    OnPropertyChanged("SelectedIndex");
                }
            }
        }

        public Visibility GroupMarkVisible
        {
            get
            {
                return _groupMarkVisible;
            }
            set
            {
                _groupMarkVisible = value;

                OnPropertyChanged("GroupMarkVisible");
            }
        }

        public string SelectedGroup
        {
            get
            {
                return _selectedGroup;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (_selectedGroup != value)
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            CellGroup[SelectedPatternTableTile] = intValue;

                            Model.PTTiles[SelectedPatternTableTile].Group = intValue;

                            ProjectItem.FileHandler.Save();
                        }
                    }

                    _selectedGroup = value;

                    OnPropertyChanged("SelectedGroup");
                }
            }
        }

        public PatternTableType SelectedPatternTableType
        {
            get { return _selectedPatternTableType; }
            set
            {
                if (_selectedPatternTableType != value)
                {
                    _selectedPatternTableType = value;

                    OnPropertyChanged("SelectedPatternTableType");

                    Save();
                }
            }
        }

        public double RectangleLeft
        {
            get { return _rectangleLeft; }
            set
            {
                _rectangleLeft = value;

                OnPropertyChanged("RectangleLeft");
            }
        }

        public double RectangleTop
        {
            get { return _rectangleTop; }
            set
            {
                _rectangleTop = value;

                OnPropertyChanged("RectangleTop");
            }
        }

        public Visibility RectangleVisibility
        {
            get { return _rectangleVisibility; }
            set
            {
                _rectangleVisibility = value;

                OnPropertyChanged("RectangleVisibility");
            }
        }

        public double SelectionRectangleLeft
        {
            get { return _selectionRectangleLeft; }
            set
            {
                _selectionRectangleLeft = value;

                OnPropertyChanged("SelectionRectangleLeft");
            }
        }

        public double SelectionRectangleTop
        {
            get { return _selectionRectangleTop; }
            set
            {
                _selectionRectangleTop = value;

                OnPropertyChanged("SelectionRectangleTop");
            }
        }

        public Visibility SelectionRectangleVisibility
        {
            get { return _selectionRectangleVisibility; }
            set
            {
                _selectionRectangleVisibility = value;

                OnPropertyChanged("SelectionRectangleVisibility");
            }
        }

        public ImageSource PTImage
        {
            get
            {
                return _pTImage;
            }
            set
            {
                _pTImage = value;

                OnPropertyChanged("PTImage");
            }
        }

        public ImageSource ImgSource
        {
            get
            {
                return _imgSource;
            }
            set
            {
                _imgSource = value;

                OnPropertyChanged("ImgSource");
            }
        }

        public Point CroppedPoint
        {
            get
            {
                return _croppedPoint;
            }
            set
            {
                _croppedPoint = value;

                OnPropertyChanged("CroppedPoint");
            }
        }

        public double ActualHeight
        {
            get
            {
                return _actualHeight;
            }
            set
            {
                _actualHeight = value;

                OnPropertyChanged("ActualHeight");
            }
        }

        public double ActualWidth
        {
            get
            {
                return _actualWidth;
            }
            set
            {
                _actualWidth = value;

                OnPropertyChanged("ActualWidth");
            }
        }

        public WriteableBitmap CroppedImage
        {
            get
            {
                return _croppedImage;
            }
            set
            {
                _croppedImage = value;

                OnPropertyChanged("CroppedImage");
            }
        }

        public FileModelVO[] TileSets
        {
            get { return _tileSets; }
            set
            {
                _tileSets = value;

                OnPropertyChanged("TileSets");
            }
        }

        public int SelectedTileSet
        {
            get { return _selectedTileSet; }
            set
            {
                _selectedTileSet = value;

                OnPropertyChanged("SelectedTileSet");
            }
        }

        public int SelectedPatternTableTile
        {
            get { return _selectedPatternTableTile; }
            set
            {
                _selectedPatternTableTile = value;

                OnPropertyChanged("SelectedPatternTableTile");
            }
        }

        public BankModel Model
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

            foreach (FileModelVO tileset in TileSets)
            {
                if (tileset.Model.GUID == id)
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

            ProjectItem.FileHandler.Save();

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
            ProjectModel project = ModelManager.Get<ProjectModel>();

            switch (project.Header.SpriteSize)
            {
                case SpriteSize.s8x8: ProjectGridSize = "8x8"; break;
                case SpriteSize.s8x16: ProjectGridSize = "8x16"; break;
            }

            TileSets = ProjectFiles.GetModels<TileSetModel>().ToArray();
        }

        public override void OnActivate()
        {
            base.OnActivate();

            GroupMarkVisible = MainWindow.ToolBarBanksShowHideGroupMarks ? Visibility.Visible : Visibility.Hidden;

            #region Signals
            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ProjectConfigurationSavedSignal>().AddListener(OnProjectConfigurationSaved);
            SignalManager.Get<FileModelVOSelectionChangedSignal>().AddListener(OnFileModelVOSelectionChanged);
            SignalManager.Get<BankImageUpdatedSignal>().AddListener(OnBankImageUpdated);
            SignalManager.Get<SelectTileSetSignal>().AddListener(OnSelectTileSet);
            SignalManager.Get<BankTileDeletedSignal>().AddListener(OnBankTileDeleted);
            SignalManager.Get<ShowGroupMarksSignal>().AddListener(OnShowGroupMarks);
            SignalManager.Get<HideGroupMarksSignal>().AddListener(OnHideGroupMarks);
            #endregion

            if (Model != null)
            {
                SelectedPatternTableType = Model.PatternTableType;

                for (int i = 0; i < Model.PTTiles.Length; i++)
                {
                    CellGroup[i] = Model.PTTiles[i].Group;
                }
            }

            LoadTileSetImage();
            LoadImage();

            SelectedPatternTableTile = -1;
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<MouseWheelSignal>().RemoveListener(OnMouseWheel);
            SignalManager.Get<OutputSelectedQuadrantSignal>().RemoveListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ProjectConfigurationSavedSignal>().RemoveListener(OnProjectConfigurationSaved);
            SignalManager.Get<FileModelVOSelectionChangedSignal>().RemoveListener(OnFileModelVOSelectionChanged);
            SignalManager.Get<BankImageUpdatedSignal>().RemoveListener(OnBankImageUpdated);
            SignalManager.Get<SelectTileSetSignal>().RemoveListener(OnSelectTileSet);
            SignalManager.Get<BankTileDeletedSignal>().RemoveListener(OnBankTileDeleted);
            SignalManager.Get<ShowGroupMarksSignal>().RemoveListener(OnShowGroupMarks);
            SignalManager.Get<HideGroupMarksSignal>().RemoveListener(OnHideGroupMarks);
            #endregion
        }

        private void LoadTileSetImage()
        {
            if (TileSets.Length == 0)
            {
                return;
            }

            RectangleVisibility = Visibility.Hidden;
            CroppedImage = null;

            TileSetModel model = TileSets[SelectedTileSet].Model as TileSetModel;

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

            if (sender.Name == "imgPatternTable")
            {
                SelectionRectangleVisibility = Visibility.Visible;
                SelectionRectangleLeft = point.X;
                SelectionRectangleTop = point.Y;

                int index = ((int)point.X / 8) + (((int)point.Y / 8) * 16);

                SelectedPatternTableTile = index;

                SelectedGroup = Model.PTTiles[index].Group.ToString();
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

            GroupMarkVisible = Visibility.Hidden;
        }

        private void OnShowGroupMarks()
        {
            if (!IsActive)
            {
                return;
            }

            GroupMarkVisible = Visibility.Visible;
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
            SelectedPatternTableTile = -1;

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
            if (Model != null)
            {
                Model.PatternTableType = SelectedPatternTableType;
                Model.Distribution = BankTileDistribution.Compact;
                Model.BankType = BankType.PatternTable;

                ProjectItem.FileHandler.Save();
            }
        }

        private void LoadImage()
        {
            if (Model == null)
            {
                return;
            }

            WriteableBitmap bitmap = BanksUtils.CreateImage(Model, ref _bitmapCache);

            PTImage = Util.ConvertWriteableBitmapToBitmapImage(bitmap);
        }
    }
}
