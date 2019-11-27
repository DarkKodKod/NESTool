using NESTool.Commands;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace NESTool.UserControls.Views
{
	/// <summary>
	/// Interaction logic for PaletteViewBig.xaml
	/// </summary>
	public partial class PaletteViewBig : UserControl, INotifyPropertyChanged
	{
		public PaletteViewBig()
		{
			InitializeComponent();
		}

		private SolidColorBrush _color0;
		private SolidColorBrush _color1;
		private SolidColorBrush _color2;
		private SolidColorBrush _color3;

        #region Commands
        public ShowColorPaletteCommand ShowColorPaletteCommand { get; } = new ShowColorPaletteCommand();
		#endregion

		#region get/set
		public SolidColorBrush Color0
		{
			get { return _color0; }
			set
			{
				_color0 = value;

				cvsColor0.Background = _color0;
			}
		}

		public SolidColorBrush Color1
		{
			get { return _color1; }
			set
			{
				_color1 = value;

				cvsColor0.Background = _color1;
			}
		}

		public SolidColorBrush Color2
		{
			get { return _color2; }
			set
			{
				_color2 = value;

				cvsColor0.Background = _color2;
			}
		}

		public SolidColorBrush Color3
		{
			get { return _color3; }
			set
			{
				_color3 = value;

				cvsColor0.Background = _color3;
			}
		}
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propname)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
		}
	}
}
