using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.UserControls.ViewModels
{
    public class CharacterFrameEditorViewModel : ViewModel
    {
        private FileModelVO[] _banks;
        private int _selectedBank;
        private string _tabId;
        private int _frameIndex;
        private string _projectGridSize;
        private Dictionary<string, WriteableBitmap> _bitmapCache = new Dictionary<string, WriteableBitmap>();
        private ImageSource _bankImage;
        private Visibility _selectionRectangleVisibility = Visibility.Hidden;
        private double _selectionRectangleTop = 0.0;
        private double _selectionRectangleLeft = 0.0;
        private int _selectedPatternTableTile;
        private CharacterModel _characterModel;
        private FileHandler _fileHandler;

        #region Commands
        public SwitchCharacterFrameViewCommand SwitchCharacterFrameViewCommand { get; } = new SwitchCharacterFrameViewCommand();
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        #endregion

        #region get/set
        public FileHandler FileHandler
        {
            get { return _fileHandler; }
            set
            {
                _fileHandler = value;

                OnPropertyChanged("FileHandler");
            }
        }

        public CharacterModel CharacterModel
        {
            get { return _characterModel; }
            set
            {
                _characterModel = value;

                OnPropertyChanged("CharacterModel");
            }
        }

        public ImageSource BankImage
        {
            get
            {
                return _bankImage;
            }
            set
            {
                _bankImage = value;

                OnPropertyChanged("BankImage");
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

        public FileModelVO[] Banks
        {
            get { return _banks; }
            set
            {
                _banks = value;

                OnPropertyChanged("Banks");
            }
        }

        public int SelectedBank
        {
            get { return _selectedBank; }
            set
            {
                _selectedBank = value;

                OnPropertyChanged("SelectedBank");
            }
        }

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

        public string TabID
        {
            get { return _tabId; }
            set
            {
                _tabId = value;

                OnPropertyChanged("TabID");
            }
        }

        public int FrameIndex
        {
            get { return _frameIndex; }
            set
            {
                _frameIndex = value;

                OnPropertyChanged("FrameIndex");
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
        #endregion

        public CharacterFrameEditorViewModel()
        {
            #region Signals
            SignalManager.Get<FileModelVOSelectionChangedSignal>().AddListener(OnFileModelVOSelectionChanged);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            #endregion

            UpdateDialogInfo();
        }

        private void UpdateDialogInfo()
        {
            IEnumerable<FileModelVO> banks = ProjectFiles.GetModels<PatternTableModel>().ToArray()
                .Where(p => (p.Model as PatternTableModel).PatternTableType == PatternTableType.Characters);

            Banks = new FileModelVO[banks.Count()];

            int index = 0;

            foreach (FileModelVO item in banks)
            {
                item.Id = index;

                Banks[index] = item;

                index++;
            }

            ProjectModel project = ModelManager.Get<ProjectModel>();

            switch (project.Header.SpriteSize)
            {
                case SpriteSize.s8x8: ProjectGridSize = "8x8"; break;
                case SpriteSize.s8x16: ProjectGridSize = "8x16"; break;
            }

            LoadBankImage();
        }

        private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
        {
            if (!IsActive)
            {
                return;
            }

            LoadBankImage();
        }

        private void OnOutputSelectedQuadrant(Image sender, WriteableBitmap bitmap, Point point)
        {
            if (!IsActive)
            {
                return;
            }

            SelectionRectangleVisibility = Visibility.Visible;
            SelectionRectangleLeft = point.X;
            SelectionRectangleTop = point.Y;

            int index = ((int)point.X / 8) + (((int)point.Y / 8) * 16);

            SelectedPatternTableTile = index;
        }

        private void LoadBankImage()
        {
            if (Banks.Length == 0)
            {
                return;
            }

            if (!(Banks[SelectedBank].Model is PatternTableModel model))
            {
                return;
            }

            WriteableBitmap patternTableBitmap = PatternTableUtils.CreateImage(model, ref _bitmapCache);

            BankImage = Util.ConvertWriteableBitmapToBitmapImage(patternTableBitmap);
        }
    }
}
