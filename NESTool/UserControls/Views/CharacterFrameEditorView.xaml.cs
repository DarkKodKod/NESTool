using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using NESTool.Utils;
using NESTool.ViewModels;
using NESTool.VOs;
using System.Collections.Generic;
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
                CharacterModel? charModel = viewModel.CharacterModel;

                if (charModel == null)
                    return;

                List<CharacterTile>? listCharacterTile = charModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles;

                if (listCharacterTile == null)
                    return;

                for (int i = 0; i < listCharacterTile.Count; ++i)
                {
                    frameView.SpritePropertiesX[i] = listCharacterTile[i].FlipX;
                    frameView.SpritePropertiesY[i] = listCharacterTile[i].FlipY;
                    frameView.SpritePaletteIndices[i] = (PaletteIndex)listCharacterTile[i].PaletteIndex;
                    frameView.SpritePropertiesBack[i] = listCharacterTile[i].BackBackground;
                }
            }
        }

        private void LoadBankImage()
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                if (viewModel.Banks?.Length == 0)
                {
                    return;
                }

                if (viewModel.Banks?[viewModel.SelectedBank].Model is not BankModel model)
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
                if (viewModel.Banks?[viewModel.SelectedBank].Model is not BankModel model)
                {
                    return;
                }

                string guid = model.PTTiles[bankViewer.SelectedBankTile].GUID;

                List<CharacterTile>? tiles = viewModel.CharacterModel?.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles;

                if (tiles?.Count <= selectedFrameTile)
                {
                    int times = selectedFrameTile - tiles.Count + 1;

                    for (int i = 0; i < times; i++)
                    {
                        tiles.Add(new CharacterTile());
                    }
                }

                if (tiles != null)
                {
                    tiles[selectedFrameTile].Point = framePoint;
                    tiles[selectedFrameTile].BankID = model.GUID;
                    tiles[selectedFrameTile].BankTileID = guid;
                }

                viewModel.FileHandler?.Save();

                LoadFrameImage();
            }
        }

        private void OnEraseTile(int selectedFrameTile)
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                List<CharacterTile>? tiles = viewModel.CharacterModel?.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles;

                if (tiles?.Count <= selectedFrameTile)
                {
                    int times = selectedFrameTile - tiles.Count + 1;

                    for (int i = 0; i < times; i++)
                    {
                        tiles.Add(new CharacterTile());
                    }
                }

                if (tiles != null)
                {
                    tiles[selectedFrameTile].BankID = string.Empty;
                    tiles[selectedFrameTile].BankTileID = string.Empty;
                }

                viewModel.FileHandler?.Save();

                LoadFrameImage();
            }
        }

        private void OnSaveProperty(int selectedFrameTile, bool flipX, bool flipY, int paletteIndex, bool backBackground)
        {
            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                List<CharacterTile>? listCharacterTile = viewModel.CharacterModel?.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles;

                if (listCharacterTile == null)
                    return;

                listCharacterTile[selectedFrameTile].FlipX = flipX;
                listCharacterTile[selectedFrameTile].FlipY = flipY;
                listCharacterTile[selectedFrameTile].PaletteIndex = paletteIndex;
                listCharacterTile[selectedFrameTile].BackBackground = backBackground;

                viewModel.FileHandler?.Save();

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

                ImageVO? vo = CharacterUtils.CreateImage(viewModel.CharacterModel, viewModel.AnimationIndex, viewModel.FrameIndex, ref CharacterViewModel.GroupedPalettes);

                if (vo != null && vo.Image != null)
                {
                    frameView.FrameImage = vo.Image;
                }
            }
        }
    }
}
