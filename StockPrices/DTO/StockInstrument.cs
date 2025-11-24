using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace StockPrices.DTO
{

    internal enum Indicator
    {
        UP,
        DOWN,
        UNCHANGED
    }
    internal class StockInstrument
    {
        public string Symbol { get; set; }  
        public string ClosePrice { get; set; }
        public string CurrentPrice { get; set; }
        public string Name { get; set; }

        public string DisplayName { get; set; }
        public string DisplaySymbol { get; set; }
        public Indicator PerformanceIndicator { get; set; }
        public string PriceChange { get; set; }
    }
}
