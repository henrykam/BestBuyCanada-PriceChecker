using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BestBuyCanadaPriceChecker
{
    /// <summary>
    /// Interaction logic for ErrorModalWindow.xaml
    /// </summary>
    public partial class ErrorModalWindow : Window
    {
        public ErrorModalWindow(string message)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            InitializeComponent();
            Message.Text = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();            
        }
    }
}
