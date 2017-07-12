using AngleSharp.Parser.Html;
using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BestBuyCanadaPriceChecker
{
    //
    public delegate void OnLoadProductsCompleted(object sender, ProgressChangedEventArgs e);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        
        private static string _baseUrl = @"https://quickresource.eyereturn.com/bestbuy/products/";
        private LoadingWindow _loadingWindow;
        private ProductsWindow _productsWindow;

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

        }        

        public async Task<Product> Lookup(string id)
        {
            
            var product = new Product();
            product.Id = id;

            if (int.TryParse(id, out int i) && id.Length == 8)
            {
                string url = _baseUrl + id.Substring(0, 5) + @"/" + id + ".json";

                string crawlUrl = @"http://www.bestbuy.ca/en-CA/Search/SearchResults.aspx?&query=" + id;

                try
                {
                    
                    RestClient client = new RestClient(url);
                    var res = await client.ExecuteGetTaskAsync(new RestRequest() { OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; } }).ConfigureAwait(false);
                    product = JsonConvert.DeserializeObject<Product>(res.Content, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() } });
                    
                    RestClient crawlClient = new RestClient(crawlUrl);
                    var page = await crawlClient.ExecuteGetTaskAsync(new RestRequest() { OnBeforeDeserialization = resp => { resp.ContentType = "text/html"; } }).ConfigureAwait(false);

                    var parser = new HtmlParser();
                    var doc = parser.Parse(page.Content);

                    var webPrice = doc.All.FirstOrDefault(m => m.ClassName == "amount" && m.TagName == "SPAN");
                    product.WebPrice = webPrice.TextContent;
                    return product;
                }
                catch (Exception)
                {
                    product.Error = "Not found";
                }
            }
            else
            {
                product.Error = "Invalid id";
            }
            return product;
        }



        [STAThread]
        private async void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            _loadingWindow = new LoadingWindow();
            _loadingWindow.Show();
            this.Hide();


            IProgress<int> progress = new Progress<int>(percent=>_loadingWindow.Progress.Value = percent);

            string s = WebcodeTextBox.Text.Trim();            
            string[] values = s.Split(',', ';', ' ');

            var res = await OnLoadProducts(values.ToList(), progress);

            _productsWindow = new ProductsWindow();
            _productsWindow.Populate(res);
            _productsWindow.Show();
            _loadingWindow.Hide();
            this.Show();            
        }

        /*

        private void Finish(object sender, RunWorkerCompletedEventArgs e)
        {
            _loadingWindow.Hide();
            this.Show();

            _productsWindow = new Products();
            _productsWindow.Populate(e.Result as List<Product>);
            _productsWindow.Show();
        }
        */

        private async Task<List<Product>> OnLoadProducts(List<string> webCodes, IProgress<int> progress)
        {
            // Remove spaces

            HashSet<string> hashSet = new HashSet<string>();

            if (webCodes.Any())
            {
                Product[] products = new Product[webCodes.Count];
                int doneCount = 0;

                var tasks = new Dictionary<Task<Product>, int>();

                for(int i=0;i<products.Length;i++)
                {
                    tasks.Add(Lookup(webCodes[i]), i);
                }

                // Report progress
                do
                {
                    var completedTask = await Task.WhenAny(tasks.Keys);
                    tasks.TryGetValue(completedTask, out int i);
                    products[i] = completedTask.Result;
                    progress.Report((int)(++doneCount * 100.0 / (double)products.Length));
                    tasks.Remove(completedTask);
                }
                while (!Task.WhenAll(tasks.Keys).IsCompleted);

                return products.ToList();
            }
            else
            {
                return null;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void WebcodeTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                EnterButton_Click(sender, e);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            WebcodeTextBox.Focus();
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "CSV files (*.csv)|*.csv";
            //dialog.FileOk += CsvFileSelected;
            dialog.ShowDialog();
            var stream = dialog.OpenFile();
            StreamReader reader = new StreamReader(stream);
            var webcodes = ReadWebcodesFromCsv(reader);

            _loadingWindow = new LoadingWindow();
            _loadingWindow.Show();
            this.Hide();

            IProgress<int> progress = new Progress<int>(percent => _loadingWindow.Progress.Value = percent);

            string s = WebcodeTextBox.Text.Trim();
            string[] values = s.Split(',', ';', ' ');

            var res = await OnLoadProducts(webcodes, progress);

            _productsWindow = new ProductsWindow();
            _productsWindow.Populate(res);
            _productsWindow.Show();
            _loadingWindow.Hide();
            this.Show();           
        }

        private void CsvFileSelected(object sender, CancelEventArgs e)
        {
            _loadingWindow = new LoadingWindow();
            _loadingWindow.Show();
            this.Hide();
                        
            _productsWindow = new ProductsWindow();
            
            //_productsWindow.Populate(res);
            _productsWindow.Show();
            _loadingWindow.Hide();
            this.Show();
        }

        private List<string> ReadWebcodesFromCsv(StreamReader stream)
        {
            var webcodes = new List<string>();

            //CsvReader reader = new CsvReader(stream, new CsvHelper.Configuration.CsvConfiguration { HasHeaderRecord = false });
            var parser = new CsvParser(stream);
            while (true)
            {
                string[] row = parser.Read();
                if (row == null)
                {
                    break;
                }

                webcodes.Add(row[0]);
            }
            //var webcodes = .GetRecords<string>();


            return webcodes;
        }

    }
}
