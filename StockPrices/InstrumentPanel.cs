using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StockPrices.DTO;
using System.Xml;

namespace StockPrices
{
    internal  class InstrumentPanel
    {
        public int Top {  get; set; }
        public int Left { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }



        public  void Draw(Graphics g, StockInstrument stockInstrument)
        {

            var localTop = 10;
            var localLeft = 10;
            var penWidth = 1;
            var titleFontSize = 12;
            var titleHeight = 22;
            var titleWidth = 250;
            var priceFontSize = 10;
            var priceHeight = 16;
            var titleFontName = "Reddit Sans";
            var indicatorWidth = 11;

            var bitmap = new Bitmap(Width,Height);

            var graphic = Graphics.FromImage(bitmap);
            graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            //graphic.
            //    DrawRectangle(new Pen(new SolidBrush(Color.Gray), penWidth), 
            //    new Rectangle(localTop, localLeft, Width-localLeft-penWidth, Height-localTop-penWidth));
            //graphic.FillRectangle(
            //    new SolidBrush(Color.FromArgb(200, Color.FromKnownColor(KnownColor.Black))),
            //    new Rectangle(localLeft + 1, localTop + 1, Width - localLeft-(penWidth) -1, Height-localTop - (penWidth)-1));

            // Title (name of the stock) 
            //
            var currentTop = localTop + 5;
            var textwidth = 
                TextRenderer.MeasureText(stockInstrument.DisplaySymbol, 
                new Font(titleFontName, titleFontSize, FontStyle.Bold));
            var titleText = stockInstrument.DisplaySymbol;
            if (textwidth.Width > titleWidth)
            {

                titleText = titleText.Substring(0, titleText.Length-10) + "...";
            }
            var indicatorTop = localTop + (titleHeight / 2) - (indicatorWidth / 2);
            if (stockInstrument.PerformanceIndicator == Indicator.UP)
            {
                // Draw up
                //
                graphic.FillPolygon(new SolidBrush(Color.LimeGreen), new Point[]
                {
                new Point(localLeft + (indicatorWidth/2),indicatorTop),
                new Point(localLeft,indicatorTop+indicatorWidth),
                new Point(localLeft + indicatorWidth,indicatorTop+indicatorWidth)
                });
            }
            else if (stockInstrument.PerformanceIndicator == Indicator.DOWN)
            {
                // Draw down
                //
                graphic.FillPolygon(new SolidBrush(Color.Red), new Point[]
                {
                new Point(localLeft + (indicatorWidth/2),indicatorTop+indicatorWidth),
                new Point(localLeft,indicatorTop),
                new Point(localLeft + indicatorWidth,indicatorTop)
                });
            }




            //var rightSidePos = localLeft + indicatorWidth + 2 + titleWidth;
            //graphic.DrawRectangle(new Pen(new SolidBrush(Color.Wheat), 1),
            // new Rectangle(
            //    localLeft + indicatorWidth + 2,
            //    localTop, 
            //    titleWidth, 
            //    titleHeight));


            graphic.DrawString(titleText,
                new Font(titleFontName, titleFontSize, FontStyle.Bold),
                new SolidBrush(Color.White),
                new Rectangle(
                    localLeft + indicatorWidth + 2,
                    localTop, 
                    titleWidth, 
                    titleHeight));
           
            
            var nextLeft = localLeft + indicatorWidth + 2 + titleWidth + 2;
            //graphic.DrawRectangle(new Pen(new SolidBrush(Color.Red), 1),
            //    new Rectangle(
            //        nextLeft,
            //        localTop,
            //        Width-nextLeft -4,
            //        titleHeight));
            Color priceColor = stockInstrument.PerformanceIndicator == Indicator.UP ? Color.LimeGreen : Color.Red;

            graphic.DrawString(stockInstrument.CurrentPrice,
                new Font(titleFontName, titleFontSize, FontStyle.Bold),
                new SolidBrush(priceColor),
                 new Rectangle(
                    nextLeft,
                    localTop,
                    Width - nextLeft - 4,
                    titleHeight),
            new StringFormat { Alignment = StringAlignment.Far });
            
            localTop += titleHeight;
            //graphic.DrawRectangle(new Pen(new SolidBrush(Color.Wheat), 1),
            // new Rectangle(
            //    localLeft + indicatorWidth + 2,
            //    localTop,
            //    titleWidth,
            //    titleHeight));


            // D
            //
            //localTop += titleHeight + 1;
            graphic.DrawString(stockInstrument.DisplayName,
                new Font(titleFontName, priceFontSize, FontStyle.Bold),
                new SolidBrush(Color.FromArgb(150, 255, 255, 255)),
                 new Rectangle(
            localLeft ,
                localTop,
                titleWidth,
                titleHeight ),
                 new StringFormat { Alignment = StringAlignment.Near, LineAlignment=StringAlignment.Far });

            graphic.DrawString(stockInstrument.PriceChange,
                new Font(titleFontName, priceFontSize, FontStyle.Bold),
                new SolidBrush(Color.FromArgb(150, 255, 255, 255)),
                 new Rectangle(
                localLeft + titleWidth,
                localTop,
                135,
                titleHeight),
                 new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far });

            //graphic.DrawRectangle(new Pen(Color.White,1),
            //    new Rectangle(
            //    localLeft + titleWidth,
            //    localTop,
            //    135,
            //    titleHeight));



            localTop += 27;
            graphic.DrawLine(
                new Pen(new SolidBrush(Color.FromArgb(128, 128, 128, 128)), penWidth),
                  localLeft, localTop, Width - 4, localTop);

            //g.Clear(Color.White);
            g.DrawImage(bitmap, Left,Top);

        }
    }
}
