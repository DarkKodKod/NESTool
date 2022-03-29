using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.Models;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.UserControls.Views;
using NESTool.Utils;
using NESTool.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl, ICleanable
    {
        private bool _mouseDown = false;

        public Map()
        {
            InitializeComponent();

            bankViewer.OnActivate();

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().Listener += OnOutputSelectedQuadrant;
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener += OnColorPaletteControlSelected;
            #endregion

            LoadBankImage();
        }

        private void LoadBankImage()
        {
            if (DataContext is MapViewModel viewModel)
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

        private void OnColorPaletteControlSelected(Color color, PaletteIndex paletteIndex, int colorPosition)
        {
            if (DataContext is MapViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }
            }

            SolidColorBrush scb = new SolidColorBrush
            {
                Color = color
            };

            PaletteView palette = null;

            switch (paletteIndex)
            {
                case PaletteIndex.Palette0: palette = palette0; break;
                case PaletteIndex.Palette1: palette = palette1; break;
                case PaletteIndex.Palette2: palette = palette2; break;
                case PaletteIndex.Palette3: palette = palette3; break;
            }

            switch (colorPosition)
            {
                case 0: palette.cvsColor0.Background = scb; break;
                case 1: palette.cvsColor1.Background = scb; break;
                case 2: palette.cvsColor2.Background = scb; break;
                case 3: palette.cvsColor3.Background = scb; break;
            }
        }

        private void ColorPaletteCleanup()
        {
            Color color = Color.FromRgb(0, 0, 0);
            SolidColorBrush brush = new SolidColorBrush(color);

            void SetColorBack(PaletteView palette)
            {
                palette.cvsColor0.Background = brush;
                palette.cvsColor1.Background = brush;
                palette.cvsColor2.Background = brush;
                palette.cvsColor3.Background = brush;
            }

            SetColorBack(palette0);
            SetColorBack(palette1);
            SetColorBack(palette2);
            SetColorBack(palette3);
        }

        public void CleanUp()
        {
            ColorPaletteCleanup();

            bankViewer.OnDeactivate();

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().Listener -= OnOutputSelectedQuadrant;
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener -= OnColorPaletteControlSelected;
            #endregion
        }

        private void ImgFrame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image)
            {
                if (image.IsMouseCaptured)
                {
                    image.ReleaseMouseCapture();
                }
            }

            _mouseDown = false;
        }

        private void ImgFrame_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Image image)
            {
                if (image.IsMouseCaptured)
                {
                    image.ReleaseMouseCapture();
                }
            }

            _mouseDown = false;
        }

        private void ImgFrame_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown)
            {
                return;
            }

            if (sender is Image image)
            {
                if (!image.IsMouseCaptured)
                {
                    return;
                }

                Point point = e.GetPosition(image);

                Util.SendSelectedQuadrantSignal(image, point);
            }
        }

        private void ImgFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image)
            {
                if (!image.IsMouseCaptured)
                {
                    image.CaptureMouse();
                }
            }

            _mouseDown = true;
        }

        private void OnOutputSelectedQuadrant(Image sender, WriteableBitmap bitmap, Point point)
        {
            if (point.X < 0 || point.Y < 0)
            {
                return;
            }

            if (sender.Name != "imgFrame")
            {
                return;
            }

            if (DataContext is MapViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }

                int index = ((int)point.X / 8) + ((int)point.Y / 8 * 32);

                viewModel.RectangleLeft = point.X;
                viewModel.RectangleTop = point.Y;

                (int, int) tuple = viewModel.GetModel().GetAttributeTileIndex(index);

                viewModel.SelectedAttributeTile = tuple.Item1;

                if (MainWindow.ToolBarMapTool == EditFrameTools.Paint && bankViewer.SelectionRectangleVisibility == Visibility.Visible)
                {
                    PaintTile(index, viewModel);
                }
                else if (MainWindow.ToolBarMapTool == EditFrameTools.Erase)
                {
                    EraseTile(index, viewModel);
                }
                else if (MainWindow.ToolBarMapTool == EditFrameTools.Select)
                {
                    // get the first element in the selected meta tile
                    int[] array = viewModel.GetModel().GetMetaTableArray(tuple.Item1);

                    if (array != null)
                    {
                        // from the first element, get the corresponding X, Y coordinates to put the rectangle in the right place
                        int y = array[0] / 32 * 8;
                        int x = (array[0] - (32 * (array[0] / 32))) * 8;

                        viewModel.RectangleLeft = x;
                        viewModel.RectangleTop = y;

                        SelectTile(viewModel);
                    }
                }
            }
        }

        private void SelectTile(MapViewModel viewModel)
        {
            if (viewModel.SelectedAttributeTile == -1)
            {
                return;
            }

            viewModel.RectangleVisibility = Visibility.Visible;

            SignalManager.Get<SelectPaletteIndexSignal>().Dispatch(viewModel.SpritePaletteIndices[viewModel.SelectedAttributeTile]);
        }

        private void PaintTile(int index, MapViewModel viewModel)
        {
            MapViewModel.FlagMapBitmapChanges[viewModel.SelectedAttributeTile] = TileUpdate.Normal;

            Point selectedTilePoint = new Point
            {
                X = viewModel.RectangleLeft,
                Y = viewModel.RectangleTop
            };

            BankModel model = viewModel.Banks[viewModel.SelectedBank].Model as BankModel;

            string guid = model.PTTiles[bankViewer.SelectedBankTile].GUID;

            ref MapTile tile = ref viewModel.GetModel().GetTile(index);

            tile.Point = selectedTilePoint;
            tile.BankID = model.GUID;
            tile.BankTileID = guid;

            MapViewModel.PointMapBitmapChanges[viewModel.SelectedAttributeTile] = selectedTilePoint;

            viewModel.ProjectItem?.FileHandler.Save();

            viewModel.LoadFrameImage(true);
        }

        private void EraseTile(int index, MapViewModel viewModel)
        {
            MapViewModel.FlagMapBitmapChanges[viewModel.SelectedAttributeTile] = TileUpdate.Erased;

            Point selectedTilePoint = new Point
            {
                X = viewModel.RectangleLeft,
                Y = viewModel.RectangleTop
            };

            MapViewModel.PointMapBitmapChanges[viewModel.SelectedAttributeTile] = selectedTilePoint;

            ref MapTile tile = ref viewModel.GetModel().GetTile(index);

            viewModel.LoadFrameImage(true);

            tile.BankID = string.Empty;
            tile.BankTileID = string.Empty;

            viewModel.ProjectItem?.FileHandler.Save();
        }
    }
}
