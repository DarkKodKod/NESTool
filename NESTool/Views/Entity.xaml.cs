using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;
using NESTool.Utils;
using NESTool.ViewModels;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;

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
            if (!(fileModel.Model is BankModel))
            {
                return;
            }

            if (DataContext is EntityViewModel viewModel)
            {
                if (viewModel.ShowBankView == Visibility.Visible)
                {
                    LoadBankImage();
                }
            }
        }

        private void LoadBankImage()
        {
            if (DataContext is EntityViewModel viewModel)
            {
                if (viewModel.Banks?.Length == 0)
                {
                    return;
                }

                if (!(viewModel.Banks?[viewModel.SelectedBank].Model is BankModel model))
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
                EntityModel? model = viewModel.GetModel();

                if (model != null && model.Frame.Tiles != null)
                {
                    model.Frame.Tiles[selectedFrameTile].FlipX = flipX;
                    model.Frame.Tiles[selectedFrameTile].FlipY = flipY;
                    model.Frame.Tiles[selectedFrameTile].PaletteIndex = paletteIndex;
                    model.Frame.Tiles[selectedFrameTile].BackBackground = backBackground;

                    viewModel.Save();

                    LoadFrameImage();
                }
            }
        }

        private void OnPaintTile(int selectedFrameTile, Point framePoint)
        {
            if (DataContext is EntityViewModel viewModel)
            {
                BankModel? model = viewModel.Banks?[viewModel.SelectedBank].Model as BankModel;

                string? guid = model?.PTTiles[bankViewer.SelectedBankTile].GUID;

                EntityModel? entityModel = viewModel.GetModel();

                if (entityModel != null && entityModel.Frame.Tiles != null)
                {
                    entityModel.Frame.Tiles[selectedFrameTile].Point = framePoint;

                    if (model != null)
                        entityModel.Frame.Tiles[selectedFrameTile].BankID = model.GUID;

                    if (guid != null)
                        entityModel.Frame.Tiles[selectedFrameTile].BankTileID = guid;

                    viewModel.Save();

                    LoadFrameImage();
                }
            }
        }

        private void OnEraseTile(int selectedFrameTile)
        {
            if (DataContext is EntityViewModel viewModel)
            {
                EntityModel? model = viewModel.GetModel();

                if (model != null && model.Frame.Tiles != null)
                {
                    model.Frame.Tiles[selectedFrameTile].BankID = string.Empty;
                    model.Frame.Tiles[selectedFrameTile].BankTileID = string.Empty;

                    viewModel.Save();

                    LoadFrameImage();
                }
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
                EntityModel? model = viewModel.GetModel();

                if (model == null)
                {
                    return;
                }

                ImageVO vo = EntityUtils.CreateImage(model);

                if (vo != null && vo.Image != null)
                {
                    frameView.FrameImage = vo.Image;
                }
            }
        }
    }
}
