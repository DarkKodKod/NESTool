using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.UserControls.ViewModels;
using NESTool.Utils;
using NESTool.VOs;

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

        private void OnOutputSelectedQuadrant(Image sender, WriteableBitmap bitmap, Point point)
        {
            if (sender.Name != "imgFrame")
            {
                return;
            }

            if (DataContext is CharacterFrameEditorViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }

                viewModel.RectangleLeft = point.X;
                viewModel.RectangleTop = point.Y;

                int index = ((int)point.X / 8) + ((int)point.Y / 8 * 8);

                viewModel.SelectedFrameTile = index;

                if (viewModel.EditFrameTools == EditFrameTools.Select)
                {
                    SelectTile(viewModel);
                }
                else if (viewModel.EditFrameTools == EditFrameTools.Paint && bankViewer.SelectionRectangleVisibility == Visibility.Visible)
                {
                    PaintTile(viewModel);
                }
                else if (viewModel.EditFrameTools == EditFrameTools.Erase)
                {
                    EraseTile(viewModel);
                }
            }
        }

        private void SelectTile(CharacterFrameEditorViewModel viewModel)
        {
            viewModel.RectangleVisibility = Visibility.Visible;

            viewModel.FlipX = viewModel.SpritePropertiesX[viewModel.SelectedFrameTile];
            viewModel.FlipY = viewModel.SpritePropertiesY[viewModel.SelectedFrameTile];
            viewModel.BackBackground = viewModel.SpritePropertiesBack[viewModel.SelectedFrameTile];

            SignalManager.Get<SelectPaletteIndexSignal>().Dispatch(viewModel.SpritePaletteIndices[viewModel.SelectedFrameTile]);
        }

        private void PaintTile(CharacterFrameEditorViewModel viewModel)
        {
            Point characterPoint = new Point
            {
                X = viewModel.RectangleLeft,
                Y = viewModel.RectangleTop
            };

            BankModel model = viewModel.Banks[viewModel.SelectedBank].Model as BankModel;

            string guid = model.PTTiles[bankViewer.SelectedBankTile].GUID;

            viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[viewModel.SelectedFrameTile].Point = characterPoint;
            viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[viewModel.SelectedFrameTile].BankID = model.GUID;
            viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[viewModel.SelectedFrameTile].BankTileID = guid;

            viewModel.FileHandler.Save();

            viewModel.LoadFrameImage();
        }

        private void EraseTile(CharacterFrameEditorViewModel viewModel)
        {
            viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[viewModel.SelectedFrameTile].BankID = string.Empty;
            viewModel.CharacterModel.Animations[viewModel.AnimationIndex].Frames[viewModel.FrameIndex].Tiles[viewModel.SelectedFrameTile].BankTileID = string.Empty;

            viewModel.FileHandler.Save();

            viewModel.LoadFrameImage();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            bankViewer.OnActivate();

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().Listener += OnOutputSelectedQuadrant;
            SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener += OnFileModelVOSelectionChanged;
            #endregion
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            bankViewer.OnDeactivate();

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().Listener -= OnOutputSelectedQuadrant;
            SignalManager.Get<FileModelVOSelectionChangedSignal>().Listener -= OnFileModelVOSelectionChanged;
            #endregion
        }

        private void OnFileModelVOSelectionChanged(FileModelVO fileModel)
        {
            LoadBankImage();
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

                WriteableBitmap bankBitmap = BanksUtils.CreateImage(model, ref bankViewer.BitmapCache);

                bankViewer.BankImage = Util.ConvertWriteableBitmapToBitmapImage(bankBitmap);
            }
        }
    }
}
