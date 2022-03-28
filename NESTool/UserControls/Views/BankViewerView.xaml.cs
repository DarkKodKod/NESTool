using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using NESTool.Commands;
using NESTool.Signals;
using ArchitectureLibrary.Signals;
using System.Windows;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for BankViewer.xaml
    /// </summary>
    public partial class BankViewerView : UserControl, INotifyPropertyChanged
    {
        private int _scale = 3;
        private ImageSource _bankImage;
        private Visibility _selectionRectangleVisibility = Visibility.Hidden;
        private double _selectionRectangleTop = 0.0;
        private double _selectionRectangleLeft = 0.0;
        private int _selectedBankTile;

        public Dictionary<string, WriteableBitmap> BitmapCache = new Dictionary<string, WriteableBitmap>();
        public event PropertyChangedEventHandler PropertyChanged;

        #region Commands
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
        #endregion

        #region get/set
        public int Scale
        {
            get => _scale;
            set
            {
                _scale = value;

                OnPropertyChanged("Scale");
            }
        }

        public int SelectedBankTile
        {
            get => _selectedBankTile;
            set
            {
                _selectedBankTile = value;

                OnPropertyChanged("SelectedBankTile");
            }
        }

        public double SelectionRectangleLeft
        {
            get => _selectionRectangleLeft;
            set
            {
                _selectionRectangleLeft = value;

                OnPropertyChanged("SelectionRectangleLeft");
            }
        }

        public double SelectionRectangleTop
        {
            get => _selectionRectangleTop;
            set
            {
                _selectionRectangleTop = value;

                OnPropertyChanged("SelectionRectangleTop");
            }
        }

        public ImageSource BankImage
        {
            get => _bankImage;
            set
            {
                _bankImage = value;

                OnPropertyChanged("BankImage");
            }
        }

        public Visibility SelectionRectangleVisibility
        {
            get => _selectionRectangleVisibility;
            set
            {
                _selectionRectangleVisibility = value;

                OnPropertyChanged("SelectionRectangleVisibility");
            }
        }
        #endregion

        public BankViewerView()
        {
            InitializeComponent();
        }

        public void OnActivate()
        {
            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().Listener += OnOutputSelectedQuadrant;
            #endregion
        }

        public void OnDeactivate()
        {
            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().Listener -= OnOutputSelectedQuadrant;
            #endregion
        }

        private void OnOutputSelectedQuadrant(Image sender, WriteableBitmap bitmap, Point point)
        {
            if (point.X < 0 || point.Y < 0)
            {
                return;
            }

            if (sender.Name == "imgBank")
            {
                SelectionRectangleVisibility = Visibility.Visible;
                SelectionRectangleLeft = point.X;
                SelectionRectangleTop = point.Y;

                int index = ((int)point.X / 8) + ((int)point.Y / 8 * 16);

                SelectedBankTile = index;
            }
        }

        protected virtual void OnPropertyChanged(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }
    }
}
