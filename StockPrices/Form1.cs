using StockPrices.DTO;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace StockPrices
{
    public partial class Form1 : Form
    {

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);


        private static List<StockInstrument> _instruments = new List<StockInstrument>();

        private int _currentTop = 10;
        private int _controlHeight = 65;
        Graphics _graphics; // = Graphics.FromImage(bitmap);
        Bitmap _bitmap;
        bool _fetched;
        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint, true);
            this.BackColor = Color.FromArgb(1, 0, 0, 0);
            SetLocation();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            IntPtr handle = this.Handle;
            int exStyle = GetWindowLong(handle, GWL_EXSTYLE);
            SetWindowLong(handle, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW );
        }

        private void Setup()
        {
            var symbols = new List<string> { ".dji", ".ixic", "f", "vong", "zbra" };
            ;
            foreach (var symbol in symbols)
            {
                var ixm = new StockInstrument { Symbol = symbol };
                _instruments.Add(ixm);
            }

            var niceNames = new Dictionary<string, string>()
            {
                { "vong" , "Vanguard Russell 1000 Growth Index"},
                { "f", "Ford"},
                { ".ixic", "NASDAQ Composite"},
                { ".dji" , "Dow Jones Industrial Average"},
                { "zbra", "Zebra Technologies"}
            };

            foreach (var ixm in _instruments)
            {
                ixm.DisplayName = niceNames.GetValueOrDefault(ixm.Symbol, ixm.Symbol);
            }

            var niceSymbols = new Dictionary<string, string>()
            {
                { "vong" , "VONG"},
                { "f", "F"},
                { ".ixic", "NASDAQ"},
                { ".dji" , "Dow Jones"},
                { "zbra", "ZBRA"}
            };

            foreach (var ixm in _instruments)
            {
                ixm.DisplaySymbol = niceSymbols.GetValueOrDefault(ixm.Symbol, ixm.Symbol);
            }


            _fetched = false;
        }

        private async Task<bool> FetchData()
        {

            var env = Environment.GetEnvironmentVariable("isProduction") ?? "true";
            var isProd = env == "true";

            var retVal = true;

            retVal = isProd ?
                await Fetch() :
                await MockFetch();

            return retVal;
        }

        Func<Task<bool>> Fetch = async () =>
        {

            var retVal = true;
            var pg = new PriceGetter();
            foreach (var symbol in _instruments)
            {
                var res = await pg.GetPrice(symbol);
            }
            return retVal;
        };

        Func<Task<bool>> MockFetch = () =>
        {

            var retVal = true;
            _instruments.Clear();
            _instruments.Add(
                new StockInstrument
                {
                    Name = "Dow Jones industrial avagerer",
                    ClosePrice = "230.97",
                    CurrentPrice = "230.97",
                    DisplayName = "Dow Jones Industrial Average",
                    Symbol = ".dji",
                    DisplaySymbol = "Dow Jones",
                    PerformanceIndicator = Indicator.UP,
                    PriceChange = "+1.34"

                });

            _instruments.Add(
                           new StockInstrument
                           {
                               Name = "Apple Inc",
                               ClosePrice = "230.97",
                               CurrentPrice = "2310.00",
                               DisplayName = "Apple Inc.",
                               Symbol = "AAPL",
                               DisplaySymbol = "AAPL",
                               PerformanceIndicator = Indicator.DOWN,
                               PriceChange = "-2.34"
                           });
            _instruments.Add(
                          new StockInstrument
                          {
                              Name = "Vong",
                              ClosePrice = "230.97",
                              CurrentPrice = "2310.00",
                              DisplayName = "Apple Inc.",
                              Symbol = "VONG",
                              DisplaySymbol = "AAPL",
                              PerformanceIndicator = Indicator.UNCHANGED,
                              PriceChange = "UNCH"
                          });
            // var pg = new PriceGetter();

            return Task.FromResult(retVal);
        };



        private void DrawPanels()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
            }

            _bitmap = new Bitmap(this.Width, this.Height);
            _graphics = Graphics.FromImage(_bitmap);
            _currentTop = 10;

            foreach (var ins in _instruments)
            {
                var intspane = new InstrumentPanel()
                {
                    Top = _currentTop,
                    Left = 5,
                    Width = 400,
                    Height = _controlHeight
                };

                intspane.Draw(_graphics, ins);
                _currentTop = _currentTop + _controlHeight;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_bitmap != null)
                e.Graphics.DrawImage(_bitmap, 0, 0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Setup();
            GraphicsPath p = RoundedRect(this.ClientRectangle, 10);
            this.Region = new Region(p);


            timer1.Interval = 60000 * 5;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
            Timer1_Tick(null, e);
        }

        private async void Timer1_Tick(object? sender, EventArgs e)
        {
            var res = false;
            // No sat or sun
            //
            var isWeekend = DateTime.Now.DayOfWeek == DayOfWeek.Sunday ||
                DateTime.Now.DayOfWeek == DayOfWeek.Saturday;
            // Not after 4:30 pm or before 9:30 am
            //
            var isTradingHours = (DateTime.Now.Hour > 9 && DateTime.Now.Minute > 30) &&
                (DateTime.Now.Hour < 16 && DateTime.Now.Minute > 30);

            if (!isTradingHours && !isWeekend)
            {
                res = await FetchData();
                DrawPanels();
                Invalidate();
              //  this.Text = $"Stocks - Last refreshed {DateTime.Now}";
            }
            else if (!_fetched)
            {
                res = await FetchData();
                DrawPanels();
                Invalidate();
                _fetched = true;
               // this.Text = $"Stocks - Outside of trading hours";
            }
            else
            {
              //  this.Text = $"Stocks - Outside of trading hours";
            }

        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(25, 255, 255, 255)), this.ClientRectangle);
        }


        // Needed to allow for resizing with no borders
        //
        protected override void WndProc(ref Message m)
        {
            const int RESIZE_HANDLE_SIZE = 10;

            switch (m.Msg)
            {
                case 0x0084/*NCHITTEST*/ :
                    base.WndProc(ref m);

                    if ((int)m.Result == 0x01/*HTCLIENT*/)
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32());
                        Point clientPoint = this.PointToClient(screenPoint);
                        if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)13/*HTTOPLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)12/*HTTOP*/ ;
                            else
                                m.Result = (IntPtr)14/*HTTOPRIGHT*/ ;
                        }
                        else if (clientPoint.Y <= (Size.Height - RESIZE_HANDLE_SIZE))
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)10/*HTLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)2/*HTCAPTION*/ ;
                            else
                                m.Result = (IntPtr)11/*HTRIGHT*/ ;
                        }
                        else
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)16/*HTBOTTOMLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)15/*HTBOTTOM*/ ;
                            else
                                m.Result = (IntPtr)17/*HTBOTTOMRIGHT*/ ;
                        }
                    }
                    return;
            }
            base.WndProc(ref m);
        }


        private bool IsLocationVisible(Point location, Size size)
        {
            Rectangle formRect = new Rectangle(location, size);
            return Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(formRect));
        }
        private void SetLocation()
        {
            // Check location to see if it is offscreen
            //
            var savedLocation = Properties.Settings.Default.Location;
            var savedSize = Properties.Settings.Default.Size;


            // Only restore if values are valid
            if (savedSize.Width > 0 && savedSize.Height > 0)
            {
                this.Size = savedSize;
            }

            if (savedLocation.X >= 0 && savedLocation.Y >= 0)
            {
                // Ensure the saved location is visible on any connected screen
                if (IsLocationVisible(savedLocation, savedSize))
                {
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = savedLocation;
                }
                else
                {
                    // If not visible, center on primary screen
                    this.StartPosition = FormStartPosition.CenterScreen;
                }
            }

            Size = Properties.Settings.Default.Size;

        }
        private void SaveLocation()
        {
            Properties.Settings.Default.Location = Location;
            Properties.Settings.Default.Size = Size;
            Properties.Settings.Default.Save();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLocation();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void FillRoundedRectangle(Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

}
