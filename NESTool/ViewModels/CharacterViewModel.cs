using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.ViewModels
{
    public class CharacterViewModel : ItemViewModel
    {
        private string _projectGridSize;
        private FileModelVO[] _banks;
        private int _selectedBank;
        private Dictionary<string, WriteableBitmap> _bitmapCache = new Dictionary<string, WriteableBitmap>();
        private ImageSource _bankImage;

        #region Commands
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        #endregion

        #region get/set
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
        #endregion

        public CharacterViewModel()
        {
            UpdateDialogInfo();

            #region Signals
            SignalManager.Get<FileModelVOSelectionChangedSignal>().AddListener(OnFileModelVOSelectionChanged);
            #endregion
        }

        public override void OnActivate()
        {
            base.OnActivate();

            LoadBankImage();
        }

        private void UpdateDialogInfo()
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();

            switch (project.Header.SpriteSize)
            {
                case SpriteSize.s8x8: ProjectGridSize = "8x8"; break;
                case SpriteSize.s8x16: ProjectGridSize = "8x16"; break;
            }

            IEnumerable<FileModelVO> banks = ProjectFiles.GetModels<PatternTableModel>().ToArray().Where(p => (p.Model as PatternTableModel).PatternTableType == PatternTableType.Characters);

            Banks = new FileModelVO[banks.Count()];

            int index = 0;

            foreach (FileModelVO item in banks)
            {
                item.Id = index;

                Banks[index] = item;

                index++;
            }
        }

        private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
        {
            if (!IsActive)
            {
                return;
            }

            LoadBankImage();
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
