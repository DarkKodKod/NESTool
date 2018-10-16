using System;
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
        public ClosedProjectPropertiesCommand ClosedProjectPropertiesCommand { get; } = new ClosedProjectPropertiesCommand();

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

        public int CHRSize
        {
            get { return _chrSize; }
            set
            {
                _chrSize = value;
                OnPropertyChanged("CHRSize");

                _changed = true;
            }
        }

        public int PRGSize
        {
            get { return _prgSize; }
            set
            {
                _prgSize = value;
                OnPropertyChanged("PRGSize");

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

        private bool _battery;
        private FrameTiming _frameTiming;
        private SpriteSize _spriteSize;
        private int _chrSize;
        private int _prgSize;
        private MapperModel[] _mappers;
        private int _selectedMapper;
        private bool _changed;

        public ProjectPropertiesDialogViewModel()
        {
            var mappers = ModelManager.Get<MappersModel>();

            Mappers = mappers.Mappers;

            ReadProjectData();

            SignalManager.Get<ClosedProjectPropertiesSignal>().AddListener(OnClosedProjectProperties);

            _changed = false;
        }

        private void OnClosedProjectProperties()
        {
            if (_changed == true)
            {
                // Save all changes
                var project = ModelManager.Get<ProjectModel>();

                project.Header.INesMapper = SelectedMapper;
                project.Header.CHRSize = CHRSize;
                project.Header.PRGSize = PRGSize;
                project.Header.FrameTiming = FrameTiming;
                project.Header.SpriteSize = SpriteSize;
                project.Header.Battery = Battery;

                project.Save();
            }

            SignalManager.Get<ClosedProjectPropertiesSignal>().RemoveListener(OnClosedProjectProperties);
        }

        private void ReadProjectData()
        {
            var project = ModelManager.Get<ProjectModel>();

            SelectedMapper = project.Header.INesMapper;
            CHRSize = project.Header.CHRSize;
            PRGSize = project.Header.PRGSize;
            FrameTiming = project.Header.FrameTiming;
            SpriteSize = project.Header.SpriteSize;
            Battery = project.Header.Battery;
        }
    }
}
