using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;

namespace NESTool.ViewModels
{
    public class EntityViewModel : ItemViewModel
    {
        private EntitySource _selectedSourceType = EntitySource.Character;
        private FileModelVO[] _banks;
        private int _selectedBank;

        #region Commands
        public SourceSelectionChangedCommand SourceSelectionChangedCommand { get; } = new SourceSelectionChangedCommand();
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        #endregion

        #region get/set
        public EntitySource SelectedSourceType
        {
            get => _selectedSourceType;
            set
            {
                if (_selectedSourceType != value)
                {
                    _selectedSourceType = value;

                    OnPropertyChanged("SelectedSourceType");
                }
            }
        }

        public FileModelVO[] Banks
        {
            get => _banks;
            set
            {
                _banks = value;

                OnPropertyChanged("Banks");
            }
        }

        public int SelectedBank
        {
            get => _selectedBank;
            set
            {
                _selectedBank = value;

                OnPropertyChanged("SelectedBank");
            }
        }
        #endregion

        public EntityModel GetModel()
        {
            return ProjectItem?.FileHandler.FileModel is EntityModel model ? model : null;
        }

        public EntityViewModel()
        {
            UpdateDialogInfo();
        }

        public override void OnActivate()
        {
            base.OnActivate();

            #region Signals
            SignalManager.Get<EntitySourceSelectionChangedSignal>().Listener += OnEntitySourceSelectionChanged;
            #endregion

            SelectedSourceType = GetModel().Source;
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            #region Signals
            SignalManager.Get<EntitySourceSelectionChangedSignal>().Listener -= OnEntitySourceSelectionChanged;
            #endregion
        }

        private void UpdateDialogInfo()
        {
            FileModelVO[] banks = ProjectFiles.GetModels<BankModel>().ToArray();

            Banks = new FileModelVO[banks.Length];

            int index = 0;

            foreach (FileModelVO item in banks)
            {
                item.Index = index;

                Banks[index] = item;

                index++;
            }
        }

        public void Save()
        {
            if (GetModel() == null)
            {
                return;
            }

            GetModel().Source = SelectedSourceType;

            ProjectItem.FileHandler.Save();
        }

        private void OnEntitySourceSelectionChanged(EntitySource entitySource)
        {
            Save();
        }
    }
}
