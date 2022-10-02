using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.VOs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GroupedPalettes = System.Collections.Generic.Dictionary<System.Tuple<int, NESTool.Enums.PaletteIndex>, System.Collections.Generic.Dictionary<System.Windows.Media.Color, System.Windows.Media.Color>>;

namespace NESTool.ViewModels
{
    public class EntityViewModel : ItemViewModel
    {
        private EntitySource _selectedSourceType;
        private FileModelVO[] _banks;
        private FileModelVO[] _characters;
        private CharacterAnimationVO[] _animations;
        private int _selectedBank = 0;
        private int _selectedCharacter = 0;
        private int _selectedAnimation = 0;
        private bool _cantSave = false;
        private Visibility _showBankView;
        private Visibility _showcharacterView;
        private int _entityId;
        private string _characterId;
        private string _characterAnimationId;
        private ImageSource _characterImage;
        private ObservableCollection<string> _properties = new ObservableCollection<string>();
        private string _selectedProperty;

        public static GroupedPalettes GroupedPalettes;

        #region Commands
        public SourceSelectionChangedCommand SourceSelectionChangedCommand { get; } = new SourceSelectionChangedCommand();
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        public CharacterAnimationVOSelectionChangedCommand CharacterAnimationVOSelectionChangedCommand { get; } = new CharacterAnimationVOSelectionChangedCommand();
        public OpenAddPropertyCommand OpenAddPropertyCommand { get; } = new OpenAddPropertyCommand();
        public DeleteSelectedProperty DeleteSelectedProperty { get; } = new DeleteSelectedProperty();
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

        public string SelectedProperty
        {
            get => _selectedProperty;
            set
            {
                _selectedProperty = value;

                OnPropertyChanged("SelectedProperty");
            }
        }

        public ImageSource CharacterImage
        {
            get => _characterImage;
            set
            {
                _characterImage = value;

                OnPropertyChanged("CharacterImage");
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

        public ObservableCollection<string> Properties
        {
            get => _properties;
            set
            {
                _properties = value;

                OnPropertyChanged("Properties");
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

        public FileModelVO[] Characters
        {
            get => _characters;
            set
            {
                if (_characters != value)
                {
                    _characters = value;

                    OnPropertyChanged("Characters");
                }
            }
        }

        public CharacterAnimationVO[] Animations
        {
            get => _animations;
            set
            {
                if (_animations != value)
                {
                    _animations = value;

                    OnPropertyChanged("Animations");
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

        public int SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                _selectedCharacter = value;

                OnPropertyChanged("SelectedCharacter");
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

        public int SelectedAnimation
        {
            get => _selectedAnimation;
            set
            {
                _selectedAnimation = value;

                OnPropertyChanged("SelectedAnimation");
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
            SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener += OnFileModelVOSelectionChanged;
            SignalManager.Get<CharacterAnimationVOSelectionChangedSignal>().Listener += OnCharacterAnimationVOSelectionChanged;
            SignalManager.Get<DeleteSelectedPropertySignal>().Listener += OnDeleteSelectedProperty;
            SignalManager.Get<AddPropertySignal>().Listener += OnAddProperty;
            #endregion

            SelectedSourceType = GetModel().Source;
            EntityId = GetModel().EntityId;

            foreach (string item in GetModel().Properties)
            {
                Properties.Add(item);
            }

            _characterId = GetModel().CharacterId;
            _characterAnimationId = GetModel().CharacterAnimationId;


            // Select the character on the list
            if (_characterId != null)
            {
                foreach (FileModelVO vo in Characters)
                {
                    if (vo.Model.GUID == _characterId)
                    {
                        SelectedCharacter = vo.Index;
                        break;
                    }
                }
            }

            SelectCharacterAndAnimation();
            LoadCharacterSprite();
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
            SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener -= OnFileModelVOSelectionChanged;
            SignalManager.Get<CharacterAnimationVOSelectionChangedSignal>().Listener -= OnCharacterAnimationVOSelectionChanged;
            SignalManager.Get<DeleteSelectedPropertySignal>().Listener -= OnDeleteSelectedProperty;
            SignalManager.Get<AddPropertySignal>().Listener -= OnAddProperty;
            #endregion
        }

        private void SelectCharacterAndAnimation()
        {
            foreach (FileModelVO vo in Characters)
            {
                if (vo.Index == SelectedCharacter)
                {
                    _characterId = Characters[SelectedCharacter].Model.GUID;

                    // Now fill the list with animations
                    CharacterModel model = Characters[SelectedCharacter].Model as CharacterModel;

                    int count = model.Animations.Count(e => !string.IsNullOrEmpty(e.ID));

                    int i = 0;
                    CharacterAnimationVO[] animations = new CharacterAnimationVO[count];

                    foreach (CharacterAnimation anim in model.Animations)
                    {
                        if (string.IsNullOrEmpty(anim.ID))
                        {
                            continue;
                        }

                        animations[i] = new CharacterAnimationVO(i, anim.ID, anim.Name);

                        i++;
                    }

                    Animations = animations;

                    bool foundAnimation = false;

                    if (i > 0 && !string.IsNullOrEmpty(_characterAnimationId))
                    {
                        // Get the actual index from the model
                        foreach (CharacterAnimationVO animVo in Animations)
                        {
                            if (animVo.ID == _characterAnimationId)
                            {
                                SelectedAnimation = animVo.Index;
                                foundAnimation = true;
                                break;
                            }
                        }
                    }

                    if (!foundAnimation)
                    {
                        _characterAnimationId = "";
                        SelectedAnimation = 0;
                    }

                    break;
                }
            }
        }

        private void LoadCharacterSprite()
        {
            GroupedPalettes = new GroupedPalettes();

            if (!string.IsNullOrEmpty(_characterId))
            {
                CharacterModel characterModel = ProjectFiles.GetModel<CharacterModel>(_characterId);

                if (characterModel != null)
                {
                    int index = Array.FindIndex(characterModel.Animations, a => a.ID == _characterAnimationId);

                    if (index >= 0)
                    {
                        ImageVO vo = CharacterUtils.CreateImage(characterModel, index, 0, ref GroupedPalettes);

                        if (vo != null && vo.Image != null)
                        {
                            CharacterImage = vo.Image;
                        }
                    }
                }
            }
        }

        private void OnCharacterAnimationVOSelectionChanged(CharacterAnimationVO animationVO)
        {
            // while loading Im not supposed to change these values
            if (_cantSave)
            {
                return;
            }

            CharacterModel model = Characters[SelectedCharacter].Model as CharacterModel;

            foreach (CharacterAnimation anim in model.Animations)
            {
                if (anim.ID == animationVO.ID)
                {
                    SelectedAnimation = animationVO.Index;

                    foreach (CharacterAnimationVO animVo in Animations)
                    {
                        if (animVo.Index == SelectedAnimation)
                        {
                            _characterAnimationId = animVo.ID;

                            LoadCharacterSprite();

                            Save();
                            break;
                        }
                    }

                    break;
                }
            }
        }

        private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
        {
            // while loading Im not supposed to change these values
            if (_cantSave)
            {
                return;
            }

            if (fileModel.Model is CharacterModel)
            {
                SelectCharacterAndAnimation();
            }
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

            index = 0;

            FileModelVO[] characters = ProjectFiles.GetModels<CharacterModel>().ToArray();

            Characters = new FileModelVO[characters.Length];

            foreach (FileModelVO item in characters)
            {
                item.Index = index;

                Characters[index] = item;

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
            GetModel().CharacterId = _characterId;
            GetModel().CharacterAnimationId = _characterAnimationId;
            GetModel().Properties = Properties.ToList();

            ProjectItem.FileHandler.Save();
        }

        private void OnEntitySourceSelectionChanged(EntitySource entitySource)
        {
            if (!IsActive)
            {
                return;
            }

            ShowHidePanel();
        }

        private void OnAddProperty(string property)
        {
            if (!IsActive)
            {
                return;
            }

            if (string.IsNullOrEmpty(property))
            {
                return;
            }

            Properties.Add(property);

            Save();
        }

        private void OnDeleteSelectedProperty(string selectedProperty)
        {
            if (!IsActive)
            {
                return;
            }

            if (string.IsNullOrEmpty(selectedProperty))
            {
                return;
            }

            if (Properties.Remove(selectedProperty))
            {
                Save();
            }
        }
    }
}
