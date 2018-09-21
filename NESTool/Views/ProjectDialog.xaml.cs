using System.Windows.Interop;
using System.Windows;
using System;
using System.Runtime.InteropServices;

namespace NESTool.Views
{
    /// <summary>
    /// Interaction logic for ProjectDialog.xaml
    /// </summary>
    public partial class ProjectDialog : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public ProjectDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void OnClickOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
