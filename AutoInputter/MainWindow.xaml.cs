using Com.CurtisRutland.WpfHotkeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

namespace AutoInputter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int mouseX, mouseY, clickNum;
        private bool isRunning = false;

        private Hotkey activateHotkey, deactivateHotkey;
        public MainWindow()
        {
            InitializeComponent();
        }

        //Point GetMousePos() => this.PointToScreen(Mouse.GetPosition(this));
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            activateHotkey = new Hotkey(Modifiers.Ctrl , Keys.A, this, registerImmediately: true);
            deactivateHotkey = new Hotkey(Modifiers.Ctrl, Keys.D, this, registerImmediately: true);

            activateHotkey.HotkeyPressed += ActivateClicker; //create event for hotkey to subscribe to
            deactivateHotkey.HotkeyPressed += DeactivateClicker;
        }
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            activateHotkey.Dispose();
            deactivateHotkey.Dispose();
        }
        private void ActivateClicker(object sender, HotkeyEventArgs e)
        {
            if (!this.isRunning)
                AutoClickOnNewThread();
            this.isRunning = true;
        }

        private void DeactivateClicker(object sender, HotkeyEventArgs e)
        {
            this.isRunning = false;
        }
        private void AutoClickOnNewThread()
        {
            Point p = Mouse.GetPosition(Application.Current.MainWindow);
            mouseX = Convert.ToInt32(p.X);
            mouseY = Convert.ToInt32(p.Y);

            Thread t = new Thread(AutoClick);
            t.IsBackground = true;
            t.Start();
        }

        private void AutoClick()
        {
            int timeBetweenClick = 60;

            while (this.isRunning)
            {
                Win32.mouse_event(Win32.MOUSEEVENTF_LEFTDOWN | Win32.MOUSEEVENTF_LEFTUP, (uint)mouseX, (uint)mouseY, 0, 0);
                Thread.Sleep(timeBetweenClick);
            }
        }

        public class Win32
        {
            [DllImport("User32.Dll")]
            public static extern long SetCursorPos(int x, int y);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
            //Mouse actions
            public const int MOUSEEVENTF_LEFTDOWN = 0x02;
            public const int MOUSEEVENTF_LEFTUP = 0x04;
            public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
            public const int MOUSEEVENTF_RIGHTUP = 0x10;
        }
    }
}