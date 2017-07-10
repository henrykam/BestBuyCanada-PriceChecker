using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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

        public void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress.Value = e.ProgressPercentage;
            ProgressText.Text = "Working hard: " + Progress.Value.ToString() +"%";
            //Progress.Dispatcher.Invoke(() => Progress.Value = e.ProgressPercentage, DispatcherPriority.Background);
        }

        private void Progress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == 100)
            {
                Close();
            }
        }


    }
}
