using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private Products _productsWindow;

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

        }


        public Product Lookup(string id)
        {
            //https://quickresource.eyereturn.com/bestbuy/products/10406/10406783.json
            var product = new Product();
            product.Id = id;

            if (int.TryParse(id, out int i) && id.Length == 8)
            {
                string url = _baseUrl + id.Substring(0, 5) + @"/"+id+".json";

                try
                {
                    RestClient client = new RestClient(url);
                    var res = client.Get(new RestRequest() { OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; } });
                    product = JsonConvert.DeserializeObject<Product>(res.Content, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() } });

                    //crawl
                    var crawlUrl = product.Link;
                    RestClient crawlClient = new RestClient(crawlUrl);
                    var page = crawlClient.Get(new RestRequest() { OnBeforeDeserialization = resp => { resp.ContentType = "text/html"; } });

                    var parser = new HtmlParser();
                    var doc = parser.Parse(page.Content);

                    var webPrice = doc.All.Where(m => m.ClassName == "amount").FirstOrDefault();
                    product.WebPrice = webPrice.TextContent;
                    return product;
                }
                catch(Exception)
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
        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            _loadingWindow = new LoadingWindow();
            _loadingWindow.Show();
            this.Hide();

            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += OnLoadProducts;       
            bgWorker.ProgressChanged += _loadingWindow.worker_ProgressChanged;
            bgWorker.RunWorkerCompleted += Finish;
            bgWorker.WorkerReportsProgress = true;
            bgWorker.RunWorkerAsync(WebcodeTextBox.Text);
        }

        private void Finish(object sender, RunWorkerCompletedEventArgs e)
        {
            _loadingWindow.Hide();
            this.Show();

            _productsWindow = new Products();
            _productsWindow.Populate(e.Result as List<Product>);
            _productsWindow.Show();
        }

        private void OnLoadProducts(object sender, DoWorkEventArgs e)
        {
            var s = (e.Argument as string);

            // Remove spaces
            s = s.Trim();
            string[] values = s.Split(',', ';', ' ');

            HashSet<string> hashSet = new HashSet<string>();

            if (values.Any())
            {
                Product[] products = new Product[values.Length];

                int doneCount = 0;
                Parallel.For(0, values.Length, index => {

                    var webcodeEntry = values[index];

                    lock (hashSet)
                    {
                        if (!hashSet.Add(webcodeEntry))
                        {
                            // handle duplicate
                            products[index] = new Product() { Id = webcodeEntry, Error = "Duplicate" };
                            return;
                        }
                    }

                    try
                    {
                        var product = Lookup(webcodeEntry);
                        products[index] = product;
                    }
                    catch (Exception ex)
                    {
                        products[index] = new Product() { Id = webcodeEntry, Error = "Unknown error: " + ex.Message };                        
                    }
                    
                    int percentageProgress = (int)(++doneCount * 100.0 / (double)values.Length);
                    (sender as BackgroundWorker).ReportProgress(percentageProgress);

                });

                e.Result = products.ToList();
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WebcodeTextBox_KeyDown(object sender, KeyEventArgs e)
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

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
