using StockPrices.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockPrices
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public void Draw()
        {

        }
        protected override void OnPaint(PaintEventArgs e)
        {

            var bitmap = new Bitmap(100, 100);
            var graphic = Graphics.FromImage(bitmap);
            graphic.
                DrawRectangle(new Pen(new SolidBrush(Color.FromKnownColor(KnownColor.Black)), 1),
                new Rectangle(10, 10, 100, 100));
            graphic.FillRectangle(
                new SolidBrush(Color.FromArgb(128, Color.FromKnownColor(KnownColor.Black))),
                new Rectangle(11, 11, 99, 99));

            // Title (name of the stock)
            //
            graphic.DrawString("asdf", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(Color.Black),
                new RectangleF(14, 14, 70, 12));

            // current price 
            //
            graphic.DrawString("asfasfsadf", new Font("Arial", 10), new SolidBrush(Color.Black),
                new RectangleF(14, 30, 70, 12));
            //g.Clear(Color.White);
            e.Graphics.DrawImage(bitmap, 0, 0);


        }

    }
}
