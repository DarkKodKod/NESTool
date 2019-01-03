using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using ArchitectureLibrary.ViewModel;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.ViewModels
{
    public class ProjectPropertiesDialogViewModel : ViewModel
    {
        public CloseDialogCommand CloseDialogCommand { get; } = new CloseDialogCommand();

        #region get/set
        public bool Battery
        {
            get { return _battery; }
            set
            {
                _battery = value;
                OnPropertyChanged("Battery");

                _changed = true;
            }
        }

        public int[] CHRSizes
        {
            get { return _chrSizes; }
            set
            {
                _chrSizes = value;
                OnPropertyChanged("CHRSizes");
            }
        }

        public int SelectedCHRSize
        {
            get { return _selectedCHRSize; }
            set
            {
                _selectedCHRSize = value;
                OnPropertyChanged("SelectedCHRSize");

                _changed = true;
            }
        }

        public MirroringType SelectedMirroring
        {
            get { return _selectedMirroring; }
            set
            {
                _selectedMirroring = value;
                OnPropertyChanged("SelectedMirroring");

                _changed = true;
            }
        }

        public int[] PRGSizes
        {
            get { return _prgSizes; }
            set
            {
                _prgSizes = value;
                OnPropertyChanged("PRGSizes");
            }
        }

        public int SelectedPRGSize
        {
            get { return _selectedPRGSize; }
            set
            {
                _selectedPRGSize = value;
                OnPropertyChanged("SelectedPRGSize");

                _changed = true;
            }
        }

        public FrameTiming FrameTiming
        {
            get { return _frameTiming; }
            set
            {
                _frameTiming = value;
                OnPropertyChanged("FrameTiming");

                _changed = true;
            }
        }

        public SpriteSize SpriteSize
        {
            get { return _spriteSize; }
            set
            {
                _spriteSize = value;
                OnPropertyChanged("SpriteSize");

                _changed = true;
            }
        }

        public MapperModel[] Mappers
        {
            get { return _mappers; }
            set
            {
                _mappers = value;
                OnPropertyChanged("Mappers");

                _changed = true;
            }
        }

        public int SelectedMapper
        {
            get { return _selectedMapper; }
            set
            {
                _selectedMapper = value;
                OnPropertyChanged("SelectedMapper");

                _changed = true;
            }
        }
        #endregion

        private bool _battery;
        private FrameTiming _frameTiming;
        private SpriteSize _spriteSize;
        private int[] _chrSizes;
        private int[] _prgSizes;
        private MapperModel[] _mappers;
        private int _selectedMapper;
        private MirroringType _selectedMirroring = MirroringType.Vertical;
        private int _selectedCHRSize;
        private int _selectedPRGSize;
        private bool _changed;

        public ProjectPropertiesDialogViewModel()
        {
            MappersModel mappers = ModelManager.Get<MappersModel>();

            Mappers = mappers.Mappers;

            ReadProjectData();

            SignalManager.Get<CloseDialogSignal>().AddListener(OnCloseDialog);

            _changed = false;
        }

        private void OnCloseDialog()
        {
            if (_changed == true)
            {
                // Save all changes
                ProjectModel project = ModelManager.Get<ProjectModel>();

                project.Header.INesMapper = SelectedMapper;
                project.Header.CHRSize = SelectedCHRSize;
                project.Header.PRGSize = SelectedPRGSize;
                project.Header.FrameTiming = FrameTiming;
                project.Header.SpriteSize = SpriteSize;
                project.Header.MirroringType = SelectedMirroring;
                project.Header.Battery = Battery;

                project.Save();
            }

            SignalManager.Get<CloseDialogSignal>().RemoveListener(OnCloseDialog);
        }

        private void ReadProjectData()
        {
            ProjectModel project = ModelManager.Get<ProjectModel>();
            MappersModel mappers = ModelManager.Get<MappersModel>();

            SelectedMapper = project.Header.INesMapper;
            CHRSizes = mappers.Mappers[SelectedMapper].CHR;
            PRGSizes = mappers.Mappers[SelectedMapper].PRG;
            SelectedCHRSize = project.Header.CHRSize;
            SelectedPRGSize = project.Header.PRGSize;
            FrameTiming = project.Header.FrameTiming;
            SpriteSize = project.Header.SpriteSize;
            SelectedMirroring = project.Header.MirroringType;
            Battery = project.Header.Battery;
        }
    }
}
