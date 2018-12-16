using System;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace NESTool.Utils
{
    static public class WindowUtility
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_DLGMODALFRAME = 0x0001;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int WM_SETICON = 0x0080;
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        static public void RemoveIcon(Window window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            int currentStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

            SetWindowLong(hWnd, GWL_EXSTYLE, currentStyle | WS_EX_DLGMODALFRAME);

            // reset the icon, both calls important
            SendMessage(hWnd, WM_SETICON, (IntPtr)ICON_SMALL, IntPtr.Zero);
            SendMessage(hWnd, WM_SETICON, (IntPtr)ICON_BIG, IntPtr.Zero);

            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }
    }
}
