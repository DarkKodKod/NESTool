using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for Entity.xaml
    /// </summary>
    public partial class Entity : UserControl
    {
        public Entity()
        {
            InitializeComponent();

            LoadBankImage();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            bankViewer.OnActivate();
            frameView.OnActivate();

            #region Signals
            SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener += OnFileModelVOSelectionChanged;
            SignalManager.Get<PaintTileSignal>().Listener += OnPaintTile;
            SignalManager.Get<EraseTileSignal>().Listener += OnEraseTile;
            SignalManager.Get<UpdateCharacterImageSignal>().Listener += OnUpdateCharacterImage;
            SignalManager.Get<SavePropertySignal>().Listener += OnSaveProperty;
            #endregion

            LoadFrameImage();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            #region Signals
            SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener -= OnFileModelVOSelectionChanged;
            SignalManager.Get<PaintTileSignal>().Listener -= OnPaintTile;
            SignalManager.Get<EraseTileSignal>().Listener -= OnEraseTile;
            SignalManager.Get<UpdateCharacterImageSignal>().Listener -= OnUpdateCharacterImage;
            SignalManager.Get<SavePropertySignal>().Listener -= OnSaveProperty;
            #endregion

            bankViewer.OnDeactivate();
            frameView.OnDeactivate();
        }

        private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
        {
            LoadBankImage();
        }

        private void LoadBankImage()
        {
            if (DataContext is EntityViewModel viewModel)
            {
                if (viewModel.Banks.Length == 0)
                {
                    return;
                }

                if (!(viewModel.Banks[viewModel.SelectedBank].Model is BankModel model))
                {
                    return;
                }

                bankViewer.BankImage = BanksUtils.CreateImage(model, ref bankViewer.BitmapCache);
            }
        }

        private void OnSaveProperty(int selectedFrameTile, bool flipX, bool flipY, int paletteIndex, bool backBackground)
        {
            if (DataContext is EntityViewModel viewModel)
            {
                viewModel.GetModel().Frame.Tiles[selectedFrameTile].FlipX = flipX;
                viewModel.GetModel().Frame.Tiles[selectedFrameTile].FlipY = flipY;
                viewModel.GetModel().Frame.Tiles[selectedFrameTile].PaletteIndex = paletteIndex;
                viewModel.GetModel().Frame.Tiles[selectedFrameTile].BackBackground = backBackground;

                viewModel.Save();

                LoadFrameImage();
            }
        }

        private void OnPaintTile(int selectedFrameTile, Point framePoint)
        {
            if (DataContext is EntityViewModel viewModel)
            {
                BankModel model = viewModel.Banks[viewModel.SelectedBank].Model as BankModel;

                string guid = model.PTTiles[bankViewer.SelectedBankTile].GUID;

                viewModel.GetModel().Frame.Tiles[selectedFrameTile].Point = framePoint;
                viewModel.GetModel().Frame.Tiles[selectedFrameTile].BankID = model.GUID;
                viewModel.GetModel().Frame.Tiles[selectedFrameTile].BankTileID = guid;

                viewModel.Save();

                LoadFrameImage();
            }
        }

        private void OnEraseTile(int selectedFrameTile)
        {
            if (DataContext is EntityViewModel viewModel)
            {
                viewModel.GetModel().Frame.Tiles[selectedFrameTile].BankID = string.Empty;
                viewModel.GetModel().Frame.Tiles[selectedFrameTile].BankTileID = string.Empty;

                viewModel.Save();

                LoadFrameImage();
            }
        }

        private void OnUpdateCharacterImage()
        {
            if (DataContext is EntityViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }

                LoadFrameImage();
            }
        }

        public void LoadFrameImage()
        {
            if (DataContext is EntityViewModel viewModel)
            {
                if (viewModel.GetModel() == null)
                {
                    return;
                }

                WriteableBitmap frameBitmap = EntityUtils.CreateImage(viewModel.GetModel());

                if (frameBitmap != null)
                {
                    frameView.FrameImage = frameBitmap;
                }
            }
        }
    }
}
