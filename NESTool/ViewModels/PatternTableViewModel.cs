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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class PatternTableViewModel : ItemViewModel
    {
        private double _actualWidth;
        private double _actualHeight;
        private Point _croppedPoint;
        private PatternTableType _selectedPatternTableType = PatternTableType.Characters;
        private WriteableBitmap _croppedImage;
        private string _projectGridSize;
        private string _selectedGroup;
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
        private PatternTableModel _model = null;
        private Dictionary<string, WriteableBitmap> _bitmapCache = new Dictionary<string, WriteableBitmap>();

        #region Commands
        public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        public MoveTileToPatternTableCommand MoveTileToPatternTableCommand { get; } = new MoveTileToPatternTableCommand();
        public DeletePatternTableTileCommand DeletePatternTableTileCommand { get; } = new DeletePatternTableTileCommand();
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

        public PatternTableModel Model
        {
            get
            {
                if (_model == null)
                {
                    if (ProjectItem?.FileHandler.FileModel is PatternTableModel model)
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

        public PatternTableViewModel()
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

        private void OnPatternTableImageUpdated()
        {
            if (!IsActive)
            {
                return;
            }

            ProjectItem.FileHandler.Save();

            LoadPatternTableImage();
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

            #region Signals
            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ProjectConfigurationSavedSignal>().AddListener(OnProjectConfigurationSaved);
            SignalManager.Get<FileModelVOSelectionChangedSignal>().AddListener(OnFileModelVOSelectionChanged);
            SignalManager.Get<PatternTableImageUpdatedSignal>().AddListener(OnPatternTableImageUpdated);
            SignalManager.Get<SelectTileSetSignal>().AddListener(OnSelectTileSet);
            SignalManager.Get<PatternTableTileDeletedSignal>().AddListener(OnPatternTableTileDeleted);
            #endregion

            if (Model != null)
            {
                SelectedPatternTableType = Model.PatternTableType;
            }

            LoadTileSetImage();
            LoadPatternTableImage();

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
            SignalManager.Get<PatternTableImageUpdatedSignal>().RemoveListener(OnPatternTableImageUpdated);
            SignalManager.Get<SelectTileSetSignal>().RemoveListener(OnSelectTileSet);
            SignalManager.Get<PatternTableTileDeletedSignal>().RemoveListener(OnPatternTableTileDeleted);
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

            if (File.Exists(model.ImagePath))
            {
                ImgSource = null;

                ActualWidth = model.ImageWidth;
                ActualHeight = model.ImageHeight;

                BitmapImage bmImage = new BitmapImage();

                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bmImage.UriSource = new Uri(model.ImagePath, UriKind.Absolute);
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

        private void OnPatternTableTileDeleted()
        {
            if (!IsActive)
            {
                return;
            }

            SignalManager.Get<CleanupTileSetLinksSignal>().Dispatch();

            _bitmapCache.Clear();

            OnPatternTableImageUpdated();

            SelectionRectangleVisibility = Visibility.Hidden;
            SelectedPatternTableTile = -1;
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
                Model.Distribution = PatterTableDistribution.Compact;

                ProjectItem.FileHandler.Save();
            }
        }

        private void LoadPatternTableImage()
        {
            if (Model == null)
            {
                return;
            }

            WriteableBitmap patternTableBitmap = PatternTableUtils.CreateImage(Model, ref _bitmapCache);

            PTImage = Util.ConvertWriteableBitmapToBitmapImage(patternTableBitmap);
        }
    }
}
