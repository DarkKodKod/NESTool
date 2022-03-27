using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.UserControls;
using NESTool.UserControls.ViewModels;
using NESTool.UserControls.Views;
using NESTool.ViewModels;
using NESTool.VOs;
using System.Windows.Controls;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for Banks.xaml
    /// </summary>
    public partial class Banks : UserControl, ICleanable
    {
        public Banks()
        {
            InitializeComponent();

            SignalManager.Get<MouseWheelSignal>().Listener += OnMouseWheel;
            SignalManager.Get<AddNewTileSetLinkSignal>().Listener += OnAddNewTileSetLink;
            SignalManager.Get<CleanupTileSetLinksSignal>().Listener += OnCleanupTileSetLinks;
        }

        private void OnCleanupTileSetLinks()
        {
            if (DataContext is BanksViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }
            }

            wpLinks.Children.Clear();
        }

        private void OnAddNewTileSetLink(BankLinkVO vo)
        {
            if (DataContext is BanksViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }

                BankLinkView link = new BankLinkView();

                ((BankLinkViewModel)link.DataContext).Caption = vo.Caption;
                ((BankLinkViewModel)link.DataContext).TileSetId = vo.Id;

                _ = wpLinks.Children.Add(link);
            }
        }

        private void OnMouseWheel(MouseWheelVO vo)
        {
            if (DataContext is BanksViewModel viewModel)
            {
                if (!viewModel.IsActive)
                {
                    return;
                }
            }

            const double ScaleRate = 1.1;

            if (vo.Delta > 0)
            {
                scaleCanvas.ScaleX *= ScaleRate;
                scaleCanvas.ScaleY *= ScaleRate;
            }
            else
            {
                scaleCanvas.ScaleX /= ScaleRate;
                scaleCanvas.ScaleY /= ScaleRate;
            }
        }

        public void CleanUp()
        {
            SignalManager.Get<MouseWheelSignal>().Listener -= OnMouseWheel;
            SignalManager.Get<AddNewTileSetLinkSignal>().Listener -= OnAddNewTileSetLink;
            SignalManager.Get<CleanupTileSetLinksSignal>().Listener -= OnCleanupTileSetLinks;
        }
    }
}
