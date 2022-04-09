using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using System.Windows;

namespace NESTool.ViewModels
{
    public class EntityViewModel : ItemViewModel
    {
        private EntitySource _selectedSourceType;
        private FileModelVO[] _banks;
        private int _selectedBank;
        private bool _cantSave = false;
        private Visibility _showBankView;
        private Visibility _showcharacterView;
        private int _entityId;

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
                _selectedSourceType = value;

                OnPropertyChanged("SelectedSourceType");

                Save();
            }
        }

        public int EntityId
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;

                    OnPropertyChanged("EntityId");

                    Save();
                }
            }
        }

        public Visibility ShowBankView
        {
            get => _showBankView;
            set
            {
                _showBankView = value;

                OnPropertyChanged("ShowBankView");
            }
        }

        public Visibility ShowcharacterView
        {
            get => _showcharacterView;
            set
            {
                _showcharacterView = value;

                OnPropertyChanged("ShowcharacterView");
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
            _cantSave = true;

            base.OnActivate();

            #region Signals
            SignalManager.Get<EntitySourceSelectionChangedSignal>().Listener += OnEntitySourceSelectionChanged;
            #endregion

            SelectedSourceType = GetModel().Source;
            EntityId = GetModel().EntityId;

            ShowHidePanel();

            _cantSave = false;
        }

        private void ShowHidePanel()
        {
            if (SelectedSourceType == EntitySource.Bank)
            {
                ShowBankView = Visibility.Visible;
                ShowcharacterView = Visibility.Collapsed;
            }
            else
            {
                ShowBankView = Visibility.Collapsed;
                ShowcharacterView = Visibility.Visible;
            }
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
            if (_cantSave)
            {
                return;
            }

            if (GetModel() == null)
            {
                return;
            }

            GetModel().Source = SelectedSourceType;
            GetModel().EntityId = EntityId;

            ProjectItem.FileHandler.Save();
        }

        private void OnEntitySourceSelectionChanged(EntitySource entitySource)
        {
            ShowHidePanel();
        }
    }
}
