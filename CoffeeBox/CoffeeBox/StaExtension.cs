using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Coffee.Extensions
{
    public static class StaExtension
    {

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr Handle, int x, int y, int w, int h, bool repaint);

        public static IntPtr getHandle(this System.Windows.Media.Visual ctrl)
        {
            IntPtr handle = IntPtr.Zero;
            HwndSource hwndSource = PresentationSource.FromVisual(ctrl) as HwndSource;

            if (hwndSource != null)
            {
                handle = hwndSource.Handle;
            }
            return handle;
        }

        public static System.Windows.Point getRelativePoint(this System.Windows.Media.Visual ctrl, System.Windows.Media.Visual parent)
        {
            return ctrl.TransformToAncestor(parent).Transform(new System.Windows.Point(0, 0));
        }

        public static void ChangeSize(this Process proc, Rectangle rect, bool repaint)
        {
            if (proc.MainWindowHandle  == IntPtr.Zero) { return; }
            MoveWindow(proc.MainWindowHandle, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, repaint);
        }
    }
}