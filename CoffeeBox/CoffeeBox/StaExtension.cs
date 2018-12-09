using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Coffee.Extensions
{
    public static class StaExtension
    {
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

        public static Point getRelativePoint(this System.Windows.Media.Visual ctrl, System.Windows.Media.Visual parent)
        {
            return ctrl.TransformToAncestor(parent).Transform(new Point(0, 0));
        }
    }
}