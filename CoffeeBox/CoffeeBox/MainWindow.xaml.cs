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

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

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

        public async void StartProc(System.Drawing.Rectangle rect)
        {
            Action action = () =>
            {
                try
                {
                    Console.WriteLine("\nX: {0}, W: {1}\nY: {2}, H: {3}\n", rect.X, rect.Width, rect.Y, rect.Height);
                    string exeName = @"E:\Mina Torrents\WOW Wotlk 3.3.5a\Wow.exe";

                    IntPtr _appWin = IntPtr.Zero;
                    Process _childp = null;

                    var procInfo = new System.Diagnostics.ProcessStartInfo(exeName);

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
                    ShowWindow(_appWin, 5);
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
            Thread th = new Thread(() =>
            {
                foreach (var proc in allProcs.Where(x => !x.HasExited))
                {
                    proc.Kill();
                    proc.WaitForExit();
                }
                allProcs.Clear();

                Dispatcher.Invoke(() => this.Close(),
                System.Windows.Threading.DispatcherPriority.Normal);
            });
            th.Start();
        }

        // TODO
        private void Btn_StartInitialize_Click(object sender, RoutedEventArgs e)
        {
            // WORK HERE
            int total = 3;
            for (int i = 0; i < total; i++)
            {
                Point relP = g1.getRelativePoint(this);
                var rct = new System.Drawing.Rectangle
                {
                    // Make Grid System Work
                    X = (int)relP.X,
                    Y = (int)relP.Y + i * (int)this.g1.Height / total,
                    Width = (int)this.g1.Width,
                    Height = (int)this.g1.Height / total,
                };
                StartProc(rct);
            }
        }
    }
}