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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
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
        private ObservableCollection<ActionTabItem> _items;

        #region Commands
        public FileModelVOSelectionChangedCommand FileModelVOSelectionChangedCommand { get; } = new FileModelVOSelectionChangedCommand();
        public CharacterCloseTabCommand CharacterCloseTabCommand { get; } = new CharacterCloseTabCommand();
        public CharacterNewTabCommand CharacterNewTabCommand { get; } = new CharacterNewTabCommand();
        #endregion

        #region get/set
        public ObservableCollection<ActionTabItem> Tabs
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<ActionTabItem>();
                    var itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(_items);
                    itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
                }

                return _items;
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
            SignalManager.Get<AnimationTabDeletedSignal>().AddListener(OnAnimationTabDeleted);
            SignalManager.Get<AnimationTabNewSignal>().AddListener(OnAnimationTabNew);
            #endregion
        }

        public override void OnActivate()
        {
            base.OnActivate();

            LoadBankImage();

            PopulateTabs();
        }

        public void PopulateTabs()
        {
            // Add A tab to TabControl With a specific header and Content(UserControl)
            Tabs.Add(new ActionTabItem { Header = "UserControl 1", Content = new UserControl() });
            // Add A tab to TabControl With a specific header and Content(UserControl)
            Tabs.Add(new ActionTabItem { Header = "UserControl 2", Content = new UserControl() });
        }

        private void OnAnimationTabNew()
        {
            // Add A tab to TabControl With a specific header and Content(UserControl)
            Tabs.Add(new ActionTabItem { Header = "UserControl 3", Content = new UserControl() });
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

        private void OnAnimationTabDeleted(ActionTabItem tabItem)
        {
            foreach (ActionTabItem tab in Tabs)
            {
                if (tab == tabItem)
                {
                    Tabs.Remove(tab);
                    return;
                }
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
