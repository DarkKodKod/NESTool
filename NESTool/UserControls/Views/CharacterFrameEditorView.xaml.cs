using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using NESTool.Utils;
using NESTool.ViewModels;
using NESTool.VOs;
using System.Windows;
using System.Windows.Controls;

namespace NESTool.UserControls.Views
{
    /// <summary>
    /// Interaction logic for CharacterFrameEditorView.xaml
    /// </summary>
    public partial class CharacterFrameEditorView : UserControl
    {
        public CharacterFrameEditorView()
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
            SignalManager.Get<SavePropertySignal>().Listener += OnSaveProperty;
            SignalManager.Get<UpdateCharacterImageSignal>().Listener += OnUpdateCharacterImage;
            #endregion

            LoadSpritesProperties();
            LoadFrameImage();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            bankViewer.OnDeactivate();
            frameView.OnDeactivate();

            #region Signals
            SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener -= OnFileModelVOSelectionChanged;
            SignalManager.Get<PaintTileSignal>().Listener -= OnPaintTile;
            SignalManager.Get<EraseTileSignal>().Listener -= OnEraseTile;
            SignalManager.Get<SavePropertySignal>().Listener -= OnSaveProperty;
            SignalManager.Get<UpdateCharacterImageSignal>().Listener -= OnUpdateCharacterImage;
            #endregion
        }

        private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
        {
            LoadBankImage();
        }

        private void LoadSpritesProperties()
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                for (int i = 0; i < viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles.Length; ++i)
                {
                    frameView.SpritePropertiesX[i] = viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[i].FlipX;
                    frameView.SpritePropertiesY[i] = viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[i].FlipY;
                    frameView.SpritePaletteIndices[i] = (PaletteIndex)viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[i].PaletteIndex;
                    frameView.SpritePropertiesBack[i] = viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[i].BackBackground;
                }
            }
        }

        private void LoadBankImage()
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
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

        private void OnPaintTile(int selectedFrameTile, Point framePoint)
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                BankModel model = viewModel.Banks[viewModel.SelectedBank].Model as BankModel;

                string guid = model.PTTiles[bankViewer.SelectedBankTile].GUID;

                viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[selectedFrameTile].Point = framePoint;
                viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[selectedFrameTile].BankID = model.GUID;
                viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[selectedFrameTile].BankTileID = guid;

                viewModel.FileHandler.Save();

                LoadFrameImage();
            }
        }

        private void OnEraseTile(int selectedFrameTile)
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[selectedFrameTile].BankID = string.Empty;
                viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[selectedFrameTile].BankTileID = string.Empty;

                viewModel.FileHandler.Save();

                LoadFrameImage();
            }
        }

        private void OnSaveProperty(int selectedFrameTile, bool flipX, bool flipY, int paletteIndex, bool backBackground)
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[selectedFrameTile].FlipX = flipX;
                viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[selectedFrameTile].FlipY = flipY;
                viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[selectedFrameTile].PaletteIndex = paletteIndex;
                viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[selectedFrameTile].BackBackground = backBackground;

                viewModel.FileHandler.Save();

                LoadFrameImage();
            }
        }

        private void OnUpdateCharacterImage()
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
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
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                if (viewModel.CharacterModel == null)
                {
                    return;
                }

                ImageVO vo = CharacterUtils.CreateImage(viewModel.CharacterModel, viewModel.AnimationIndex, viewModel.FrameIndex, ref CharacterViewModel.GroupedPalettes);

                if (vo != null && vo.Image != null)
                {
                    frameView.FrameImage = vo.Image;
                }
            }
        }
    }
}
