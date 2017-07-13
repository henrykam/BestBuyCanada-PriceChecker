using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestBuyCanadaPriceChecker
{
    public class Product
    {
        public string Id { get; set; }

        public string WebPrice { get; set; }
        public string Title { get; set; }

        //public string Image { get; set; }        
        public string Link { get; set; }

        //public string AdwordsGrouping { get; set; }
        //public string AdwordsLabels { get; set; }
        public string Availability { get; set; }
        //public string Brand { get; set; }
        //public string Condition { get; set; }
        //public string CustomLabel0 { get; set; }
        //public string CustomLabel1 { get; set; }
        //public string CustomLabel2 { get; set; }
        //public string Description { get; set; }
        //public string GoogleProductCategory { get; set; }
        //public string ProductType { get; set; }
        //public string SalePrice { get; set; }
        
        public string Error { get; set; }
    }
}
