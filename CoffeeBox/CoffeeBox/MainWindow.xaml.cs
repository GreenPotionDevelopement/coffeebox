using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Coffee.Extensions;
using System.Threading;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;

namespace CoffeeBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Handles

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr Handle, int x, int y, int w, int h, bool repaint);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        private static readonly int GWL_STYLE = -16;
        private static readonly int WS_VISIBLE = 0x10000000;

        #endregion Handles

        public MainWindow()
        {
            InitializeComponent();
        }

        private List<Process> allProcs = new List<Process>();

        private void mainFrm_Loaded(object sender, RoutedEventArgs e)
        {
        }

        // E:\Mina Torrents\WOW Wotlk 3.3.5a\Wow.exe

        public async void StartProc(System.Drawing.Rectangle rect)
        {
            Action action = () =>
            {
                try
                {
                    Console.WriteLine("\nX: {0}, W: {1}\nY: {2}, H: {3}\n", rect.X, rect.Width, rect.Y, rect.Height);
                    string exeName = @"notepad.exe";

                    IntPtr _appWin = IntPtr.Zero;
                    Process _childp = null;

                    var procInfo = new System.Diagnostics.ProcessStartInfo(exeName)
                    {
                    };
                    procInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(exeName);
                    // Start the process
                    _childp = System.Diagnostics.Process.Start(procInfo);

                    _childp.WaitForInputIdle();

                    Thread.Sleep(300);

                    // Get the main handle
                    _appWin = _childp.MainWindowHandle;

                    // Put it into this form
                    var helper = new WindowInteropHelper(Window.GetWindow(this.g1));
                    SetParent(_appWin, helper.Handle);
                    // Remove border and whatnot
                    SetWindowLong(_appWin, GWL_STYLE, WS_VISIBLE);

                    // Move the window to overlay it on this window
                    MoveWindow(_appWin, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, true);
                    allProcs.Add(_childp);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + "Error");
                }
            };
            await g1.Dispatcher.BeginInvoke(action);
        }

        private void mainFrm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (allProcs.Count == 0) { return; }
            e.Cancel = true;
            KillAllProcs(true);
        }

        public  void KillAllProcs(bool closeWindow = false)
        {
            Thread th = new Thread(() =>
            {
                foreach (var proc in allProcs.Where(x => !x.HasExited))
                {
                    proc.Kill();
                    proc.WaitForExit();
                }
                allProcs.Clear();

                if (closeWindow)
                {
                   Dispatcher.Invoke(() => Close(),
                   System.Windows.Threading.DispatcherPriority.Normal);
                }
            });
            th.Start();
        }

        // TODO
        private void Btn_StartInitialize_Click(object sender, RoutedEventArgs e)
        {
                // WORK HERE
                int total = 2;
                for (int i = 0; i < total; i++)
                {
                    Point relP = g1.getRelativePoint(this);
                    var rct = new System.Drawing.Rectangle
                    {
                        // Make Grid System Work
                        X = (int)relP.X + i * (int)this.g1.ActualWidth / total,
                        Y = (int)relP.Y,
                        Width = (int)this.g1.ActualWidth / total - (int)this.g1.Margin.Left,
                        Height = (int)this.g1.ActualHeight - (int)this.g1.Margin.Top,
                    };
                    StartProc(rct);
                }
        }
        // TODO
        private void mainFrm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine("size changed: X:{0}, Y: {1}", e.NewSize.Width, e.NewSize.Height);
            Console.WriteLine("Tab Control changed: X:{0}, Y: {1}", this.tabControl1.ActualWidth, this.tabControl1.ActualHeight);
            Console.WriteLine("Grid changed: X:{0}, Y: {1}", this.g1.ActualWidth, this.g1.ActualHeight);
            int i = 0;
            var total = allProcs.Where(x => !x.HasExited && x.MainWindowHandle != IntPtr.Zero).Count();
            foreach (var proc in allProcs.Where(x => !x.HasExited && x.MainWindowHandle != IntPtr.Zero))
            {
                Point relP = g1.getRelativePoint(this);
                var rct = new System.Drawing.Rectangle
                {
                    // Make Grid System Work
                    X = (int)relP.X + i * (int)this.g1.ActualWidth / total,
                    Y = (int)relP.Y,
                    Width = (int)this.g1.ActualWidth / total - (int)this.g1.Margin.Left,
                    Height = (int)this.g1.ActualHeight - (int)this.g1.Margin.Top,
                };
                proc.ChangeSize(rct, true);
                i++;
            }
        }
    }
}