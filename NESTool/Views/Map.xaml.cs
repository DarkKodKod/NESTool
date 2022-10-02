using ArchitectureLibrary.Signals;
using NESTool.Enums;
using NESTool.FileSystem;
using NESTool.Models;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.UserControls.Views;
using NESTool.Utils;
using NESTool.ViewModels;
using NESTool.VOs;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GroupedPalettes = System.Collections.Generic.Dictionary<System.Tuple<int, NESTool.Enums.PaletteIndex>, System.Collections.Generic.Dictionary<System.Windows.Media.Color, System.Windows.Media.Color>>;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl, ICleanable
    {
        private bool _mouseDown = false;
        private Image _draggingSprite;
        private Point _draggingSpriteInitPos;

        private const int CellSize = 8;
        private const double CellOffset = 0.17;

        public Map()
        {
            InitializeComponent();

            bankViewer.OnActivate();

            #region Signals
            SignalManager.Get<OutputSelectedQuadrantSignal>().Listener += OnOutputSelectedQuadrant;
            SignalManager.Get<ColorPaletteControlSelectedSignal>().Listener += OnColorPaletteControlSelected;
            SignalManager.Get<AddImageToMapSignal>().Listener += OnAddImageToMap;
            SignalManager.Get<RemoveImageToMapSignal>().Listener += OnRemoveImageToMap;
            SignalManager.Get<SetMapElementImagePosSignal>().Listener += OnSetMapElementImagePos;
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

        private void Sprite_MouseMove(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;

            if (image != _draggingSprite)
            {
                return;
            }

            Point position = Mouse.GetPosition(cOverlay);

            double finalPosLeft = -(_draggingSpriteInitPos - position).X;
            double finalPosTop = -(_draggingSpriteInitPos - position).Y;

            Canvas.SetLeft(image, finalPosLeft);
            Canvas.SetTop(image, finalPosTop);
        }

        private void Sprite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_draggingSprite == null)
            {
                Image image = sender as Image;

                _draggingSprite = image;
                _draggingSpriteInitPos = Mouse.GetPosition(cOverlay);

                double imageLeft = Canvas.GetLeft(image);
                double imageTop = Canvas.GetTop(image);

                _draggingSpriteInitPos.X -= imageLeft;
                _draggingSpriteInitPos.Y -= imageTop;

                SignalManager.Get<MapElementSpriteSelecteedSignal>().Dispatch(image.Tag.ToString());
            }
        }

        private void Sprite_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;

            if (image == _draggingSprite)
            {
                _draggingSprite = null;

                AdjustImagePositionToGrid(image);
            }
        }

        private void Sprite_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;

            if (image == _draggingSprite)
            {
                _draggingSprite = null;

                AdjustImagePositionToGrid(image);
            }
        }

        private void Sprite_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;

            int imageLeft = Convert.ToInt32(Canvas.GetLeft(image) / CellSize);
            int imageTop = Convert.ToInt32(Canvas.GetTop(image) / CellSize);

            image.ToolTip = image.Name + Environment.NewLine + $"x: {imageLeft}" + Environment.NewLine + $"y: {imageTop}";
        }

        private void AdjustImagePositionToGrid(Image image)
        {
            Point mousePos = Mouse.GetPosition(cOverlay);

            if (_draggingSpriteInitPos.X != mousePos.X || _draggingSpriteInitPos.Y != mousePos.Y)
            {
                double imageLeft = Canvas.GetLeft(image) / CellSize;
                double imageTop = Canvas.GetTop(image) / CellSize;

                int gridLeft = imageLeft < 0 ? 0 : Convert.ToInt32(imageLeft);
                int gridTop = imageTop < 0 ? 0 : Convert.ToInt32(imageTop);

                gridLeft = Math.Min(gridLeft, 30);
                gridTop = Math.Min(gridTop, 28);

                Canvas.SetLeft(image, gridLeft * CellSize + CellOffset);
                Canvas.SetTop(image, gridTop * CellSize + CellOffset);

                SignalManager.Get<MapElementSpritePosChangedSignal>().Dispatch(image.Tag.ToString(), gridLeft, gridTop);
            }
        }

        private void OnRemoveImageToMap(string entityInstanceId)
        {
            if (DataContext is MapViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }
            }

            foreach (object child in cOverlay.Children)
            {
                if (child is Image image)
                {
                    if (image.Tag.ToString() == entityInstanceId)
                    {
                        cOverlay.Children.Remove(image);

                        break;
                    }
                }
            }
        }

        private void OnAddImageToMap(Models.Entity entity)
        {
            if (DataContext is MapViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }
            }

            FileModelVO fileModelVO = ProjectFiles.GetFileModel(entity.EntityID);

            if (fileModelVO == null)
            {
                return;
            }

            int x = entity.X;
            int y = entity.Y;

            EntityModel entityModel = fileModelVO.Model as EntityModel;

            Image sprite = GetImageFromFileModel(entityModel);

            if (sprite == null)
            {
                return;
            }

            sprite.Name = fileModelVO.Name;
            sprite.Tag = entity.InstanceID;
            sprite.MouseMove += new MouseEventHandler(Sprite_MouseMove);
            sprite.MouseDown += new MouseButtonEventHandler(Sprite_MouseDown);
            sprite.MouseUp += new MouseButtonEventHandler(Sprite_MouseUp);
            sprite.MouseLeave += new MouseEventHandler(Sprite_MouseLeave);
            sprite.MouseEnter += new MouseEventHandler(Sprite_MouseEnter);

            Canvas.SetLeft(sprite, x * CellSize + CellOffset);
            Canvas.SetTop(sprite, y * CellSize + CellOffset);

            cOverlay.Children.Add(sprite);
        }

        private void OnSetMapElementImagePos(string elementId, int xPos, int yPos)
        {
            if (DataContext is MapViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }
            }

            foreach (object child in cOverlay.Children)
            {
                if (child is Image image)
                {
                    if (image.Tag.ToString() == elementId)
                    {
                        Canvas.SetLeft(image, xPos * CellSize + CellOffset);
                        Canvas.SetTop(image, yPos * CellSize + CellOffset);

                        break;
                    }
                }
            }
        }

        private Image GetImageFromFileModel(EntityModel entityModel)
        {
            if (entityModel.Source == EntitySource.Character)
            {
                GroupedPalettes GroupedPalettes = new GroupedPalettes();

                if (string.IsNullOrEmpty(entityModel.CharacterId))
                {
                    return null;
                }

                CharacterModel characterModel = ProjectFiles.GetModel<CharacterModel>(entityModel.CharacterId);

                if (characterModel == null)
                {
                    return null;
                }

                int index = Array.FindIndex(characterModel.Animations, a => a.ID == entityModel.CharacterAnimationId);

                if (index >= 0)
                {
                    ImageVO vo = CharacterUtils.CreateImage(characterModel, index, 0, ref GroupedPalettes);

                    if (vo != null && vo.Image != null)
                    {
                        Image image = new Image
                        {
                            Source = vo.Image,
                            Width = vo.Width,
                            Height = vo.Height,
                            Stretch = Stretch.None
                        };

                        return image;
                    }
                }
            }
            else
            {
                ImageVO vo = EntityUtils.CreateImage(entityModel);

                if (vo != null && vo.Image != null)
                {
                    Image image = new Image
                    {
                        Source = vo.Image,
                        Width = vo.Width,
                        Height = vo.Height,
                        Stretch = Stretch.None
                    };

                    return image;
                }
            }

            return null;
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
            SignalManager.Get<AddImageToMapSignal>().Listener -= OnAddImageToMap;
            SignalManager.Get<RemoveImageToMapSignal>().Listener -= OnRemoveImageToMap;
            SignalManager.Get<SetMapElementImagePosSignal>().Listener -= OnSetMapElementImagePos;
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

                int index = ((int)point.X / CellSize) + ((int)point.Y / CellSize * 32);

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
                        int y = array[0] / 32 * CellSize;
                        int x = (array[0] - (32 * (array[0] / 32))) * CellSize;

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

        delegate void TestDelegate();

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem)
            {
                TestDelegate myDel = () =>
                {
                    _ = tbEditableSelectedProperty.Focus();
                    tbEditableSelectedProperty.SelectAll();
                };

                Dispatcher.BeginInvoke(myDel, null);
            }
        }
    }
}
