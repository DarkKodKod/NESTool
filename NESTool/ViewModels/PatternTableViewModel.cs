using ArchitectureLibrary.Model;
using ArchitectureLibrary.Signals;
using NESTool.Commands;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.VOs;
using System.Windows;
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

        #region Commands
        public PreviewMouseWheelCommand PreviewMouseWheelCommand { get; } = new PreviewMouseWheelCommand();
        public ImageMouseDownCommand ImageMouseDownCommand { get; } = new ImageMouseDownCommand();
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
        #endregion

        public PatternTableViewModel()
        {
            UpdateDialogInfo();

            #region Signals
            SignalManager.Get<MouseWheelSignal>().AddListener(OnMouseWheel);
            SignalManager.Get<OutputSelectedQuadrantSignal>().AddListener(OnOutputSelectedQuadrant);
            SignalManager.Get<ProjectConfigurationSavedSignal>().AddListener(OnProjectConfigurationSaved);
            #endregion
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
        }

        public override void OnActivate()
        {
            PatternTableModel model = GetModel();
            if (model != null)
            {
                SelectedPatternTableType = model.PatternTableType;
            }
        }

        private void OnOutputSelectedQuadrant(WriteableBitmap bitmap, Point point)
        {
            CroppedPoint = point;
            CroppedImage = bitmap;
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

        public PatternTableModel GetModel()
        {
            if (ProjectItem?.FileHandler.FileModel is PatternTableModel model)
            {
                return model;
            }

            return null;
        }

        private void Save()
        {
            PatternTableModel model = GetModel();

            if (model != null)
            {
                model.PatternTableType = SelectedPatternTableType;

                ProjectItem.FileHandler.Save();
            }
        }
    }
}
