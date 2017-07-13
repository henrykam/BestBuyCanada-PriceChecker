using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BestBuyCanadaPriceChecker
{
    

    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const uint MF_BYCOMMAND = 0x00000000;
        const uint MF_GRAYED = 0x00000001;
        const uint MF_ENABLED = 0x00000000;

        const uint SC_CLOSE = 0xF060;

        const int WM_SHOWWINDOW = 0x00000018;
        const int WM_CLOSE = 0x10;


        public LoadingWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            pictureBoxLoading.Image = Resource1.fliptable;//ImageLocation ="Resources/fliptable.gif";
            //Resource1.fliptable.
        }


        private void Progress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ProgressText.Text = "Working hard: " + Progress.Value.ToString() + "%";
        }





    }
}
