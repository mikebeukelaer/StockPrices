using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using StockPrices.DTO;

namespace StockPrices
{
    internal class PriceGetter
    {
        string _baseURl = "https://www.cnbc.com/quotes/";

        internal static HttpClient _client;

        public PriceGetter()
        {
           _client = new HttpClient();
        }

        public async Task<bool> GetPrice(StockInstrument ixm)
        {
            var retVal = true;
            var lastPrice = string.Empty;
            var url = $"{_baseURl}{ixm.Symbol}";
                    
            try
            {
                var content = await _client.GetStringAsync(url);
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(content);
            
                var lastStripLastPrice = 
                    htmlDoc.DocumentNode.SelectSingleNode("//span[@class='QuoteStrip-lastPrice']");
                lastPrice = lastStripLastPrice.InnerText;
                ixm.CurrentPrice = lastPrice;

                var htmlname = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='QuoteStrip-name']");
                var name = htmlname.InnerText;
                ixm.Name = name;

                var container = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='QuoteStrip-lastTimeAndPriceContainer']");

                ixm.PerformanceIndicator = Indicator.UNCHANGED;

                var upIndicator = container.SelectSingleNode(".//span[@class='QuoteStrip-changeUp']");
                var downIndicator = container.SelectSingleNode(".//span[@class='QuoteStrip-changeDown']");
                var unchangedIndicator = container.SelectSingleNode(".//span[@class='QuoteStrip-unchanged']");

                if (upIndicator != null)
                {
                    ixm.PerformanceIndicator = Indicator.UP;
                    var inner = upIndicator.SelectSingleNode(".//span");
                    ixm.PriceChange = inner.InnerText;
                }

                if (downIndicator != null) 
                {
                    ixm.PerformanceIndicator = Indicator.DOWN;
                    var inner = downIndicator.SelectSingleNode(".//span");
                    ixm.PriceChange = inner.InnerText;

                }

                if (unchangedIndicator != null) 
                { 
                    ixm.PerformanceIndicator = Indicator.UNCHANGED;
                    var inner = unchangedIndicator.SelectSingleNode(".//span");
                    ixm.PriceChange = inner.InnerText;
                }

            }
            catch (Exception ex) {
                var j = ex;
                retVal = false;
            }

            return retVal;
        }


    }
}
