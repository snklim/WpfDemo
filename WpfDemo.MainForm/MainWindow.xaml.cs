using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfDemo.CellGen;

namespace WpfDemo.MainForm
{
    public class MapCell : INotifyPropertyChanged
    {
        public int CellIndexX { get; private set; }
        public int CellIndexY { get; private set; }
        public int CellSizeX { get { return 50; } }
        public int CellSizeY { get { return 50; } }
        public int CellX { get { return 50 * CellIndexX; } }
        public int CellY { get { return 50 * CellIndexY; } }

        public int CellImgX { get; private set; }
        public int CellImgY { get; private set; }

        private static int NumCellX { get; set; }
        private static int NumCellY { get; set; }

        private List<string> ChangedProperties { get; set; }

        private MapPage CurrMapPage { get; set; }

        private static readonly int TotalNumCellX;
        private static readonly int TotalNumCellY;

        private static readonly string RootDir;

        private static readonly Dictionary<string, Dictionary<string, string>> Neighbors;

        static MapCell()
        {
            TotalNumCellX = 42;
            TotalNumCellY = 44;

            RootDir = System.IO.Directory.GetCurrentDirectory();
        }

        public string CellImgPath
        {
            get
            {
                string filePath = string.Format(@"{0}\map\{1}-{2}-{3}\{1}-{2}-{3}_{4}_{5}.jpg",
                    RootDir, CurrMapPage.Letter, CurrMapPage.Sector, CurrMapPage.Square, CellImgX, CellImgY);
                
                if (!System.IO.File.Exists(filePath))
                    return RootDir + @"\CellNotFound.jpg";

                return filePath;
            }
        }

        public MapCell(int cellIndexX, int cellIndexY)
        {
            CellImgX = CellIndexX = cellIndexX;
            CellImgY = CellIndexY = cellIndexY;

            NumCellX = Math.Max(NumCellX, cellIndexX + 1);
            NumCellY = Math.Max(NumCellY, cellIndexY + 1);

            ChangedProperties = new List<string>();

            CurrMapPage = MapPage.GetMapPage('M', 36, "061");
        }

        public void ChangeCellIndex(int totalDeltaCellIndexX, int totalDeltaCellIndexY)
        {
            int newCellIndexX = CellIndexX;
            int newCellImgX = CellImgX;

            int newCellIndexY = CellIndexY;
            int newCellImgY = CellImgY;

            if (totalDeltaCellIndexX != 0)
            {
                int deltaCellIndexX = totalDeltaCellIndexX > 0 ? 1 : -1;

                newCellIndexX = (CellIndexX + deltaCellIndexX + NumCellX) % NumCellX;

                if (deltaCellIndexX < 0 && CellIndexX == 0 && CellIndexX < newCellIndexX)
                {
                    if (newCellImgX + NumCellX >= TotalNumCellX)
                    {
                        CurrMapPage = CurrMapPage.Right;
                        //if (Neighbors[CurrMapPage].ContainsKey("R"))
                        //{
                        //    CurrMapPage = Neighbors[CurrMapPage]["R"];
                        //}
                        //else if (Neighbors[CurrMapPage].ContainsKey("L"))
                        //{
                        //    CurrMapPage = Neighbors[CurrMapPage]["L"];
                        //}
                    }
                    newCellImgX = (newCellImgX + NumCellX + TotalNumCellX) % TotalNumCellX;
                }

                if (deltaCellIndexX > 0 && CellIndexX + 1 == NumCellX && CellIndexX > newCellIndexX)
                {
                    if (newCellImgX - NumCellX < 0)
                    {
                        CurrMapPage = CurrMapPage.Left;
                        //if (Neighbors[CurrMapPage].ContainsKey("L"))
                        //{
                        //    CurrMapPage = Neighbors[CurrMapPage]["L"];
                        //}
                        //else if (Neighbors[CurrMapPage].ContainsKey("R"))
                        //{
                        //    CurrMapPage = Neighbors[CurrMapPage]["R"];
                        //}
                    }
                    newCellImgX = (newCellImgX - NumCellX + TotalNumCellX) % TotalNumCellX;
                }
            }

            if (totalDeltaCellIndexY != 0)
            {
                int deltaCellIndexY = totalDeltaCellIndexY > 0 ? 1 : -1;

                newCellIndexY = (CellIndexY + deltaCellIndexY + NumCellY) % NumCellY;

                if (deltaCellIndexY < 0 && CellIndexY == 0 && CellIndexY < newCellIndexY)
                {
                    if (newCellImgY + NumCellY >= TotalNumCellY)
                    {
                        CurrMapPage = CurrMapPage.Bottom;
                        //if (Neighbors[CurrMapPage].ContainsKey("B"))
                        //{
                        //    CurrMapPage = Neighbors[CurrMapPage]["B"];
                        //}
                        //else if (Neighbors[CurrMapPage].ContainsKey("T"))
                        //{
                        //    CurrMapPage = Neighbors[CurrMapPage]["T"];
                        //}
                    }
                    newCellImgY = (newCellImgY + NumCellY + TotalNumCellY) % TotalNumCellY;
                }

                if (deltaCellIndexY > 0 && CellIndexY + 1 == NumCellY && CellIndexY > newCellIndexY)
                {
                    if (newCellImgY - NumCellY < 0)
                    {
                        CurrMapPage = CurrMapPage.Top;
                        //if (Neighbors[CurrMapPage].ContainsKey("T"))
                        //{
                        //    CurrMapPage = Neighbors[CurrMapPage]["T"];
                        //}
                        //else if (Neighbors[CurrMapPage].ContainsKey("B"))
                        //{
                        //    CurrMapPage = Neighbors[CurrMapPage]["B"];
                        //}
                    }
                    newCellImgY = (newCellImgY - NumCellY + TotalNumCellY) % TotalNumCellY;
                }
            }
            
            if (CellIndexX != newCellIndexX)
            {
                CellIndexX = newCellIndexX;
                ChangedProperties.Add("CellX");
            }

            if (CellIndexY != newCellIndexY)
            {
                CellIndexY = newCellIndexY;
                ChangedProperties.Add("CellY");
            }

            if (CellImgX != newCellImgX || CellImgY != newCellImgY)
            {
                CellImgX = newCellImgX;
                CellImgY = newCellImgY;
                ChangedProperties.Add("CellImgPath");
            }

            NotifyPropertyChanged();
        }

        private void NotifyPropertyChanged()
        {
            if (PropertyChanged != null)
            {
                foreach (string propName in ChangedProperties)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
                }
            }

            ChangedProperties.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<MapCell> Cells { get; private set; }

        public DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof(double), typeof(Window));
        public double CenterX { get { return (double)GetValue(CenterXProperty); } set { SetValue(CenterXProperty, value); } }

        public DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof(double), typeof(Window));
        public double CenterY { get { return (double)GetValue(CenterYProperty); } set { SetValue(CenterYProperty, value); } }

        public ObservableCollection<object> Logs { get; private set; }

        private void CreateMap()
        {
            Cells.Clear();

            int numCellX = (int)Math.Ceiling(ActualWidth / 50) + 2;
            int numCellY = (int)Math.Ceiling(ActualHeight / 50) + 2;

            Enumerable.Range(0, numCellY).SelectMany(y =>
                Enumerable.Range(0, numCellX).Select(x => new MapCell(x, y)))
                .ToList()
                .ForEach(cell => Cells.Add(cell));
        }

        public MainWindow()
        {
            InitializeComponent();

            Cells = new ObservableCollection<MapCell>();

            Map.MouseDown += MainWindow_MouseDown;
            Map.MouseUp += MainWindow_MouseUp;
            Map.MouseMove += MainWindow_MouseMove;

            this.SizeChanged += MainWindow_SizeChanged;
            /*
            new System.Threading.Thread(() =>
            {
                int centerX = 97;
                int centerY = 143;

                int sizeX = 2150;
                int sizeY = 2200;

                int numPixelX = 50;
                int numPixelY = 50;

                int numCellX = (int) Math.Ceiling((double) sizeX/numPixelX);
                int numCellY = (int) Math.Ceiling((double) sizeY/numPixelY);

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"m-(\d+)-(\d+)");

                foreach (string fileSource in System.IO.Directory.GetFiles(@"C:\Users\Admin\Dropbox\map\"))
                {
                    if (!regex.IsMatch(fileSource))
                    {
                        continue;
                    }
                    System.Text.RegularExpressions.Match match = regex.Match(fileSource);

                    string dirTarget = System.IO.Directory.GetCurrentDirectory();

                    string part1 = match.Groups[1].Value;
                    string part2 = match.Groups[2].Value;

                    string fileTargetTemplate = @"{0}\map\m-{1}-{2}_{3}_{4}.jpg";

                    System.Drawing.Bitmap bSource = new System.Drawing.Bitmap(fileSource);

                    for (int j = 0; j < numCellY; j++)
                    {
                        for (int i = 0; i < numCellX; i++)
                        {
                            System.Drawing.Bitmap bTarget = new System.Drawing.Bitmap(numPixelX, numPixelY);

                            for (int y = 0; y < numPixelY; y++)
                            {
                                for (int x = 0; x < numPixelX; x++)
                                {
                                    System.Drawing.Color color =
                                        bSource.GetPixel(numPixelX * i + x + centerX, numPixelY * j + y + centerY);
                                    bTarget.SetPixel(x, y, color);
                                }
                            }

                            bTarget.Save(string.Format(fileTargetTemplate, dirTarget,
                                part1, part2, i, j));
                        }
                    }
                }

                MessageBox.Show("Created");

            }).Start();
            */
        }

        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CreateMap();
        }

        bool mouseDown;
        Point deltaDownPoint;

        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point mouseCurrPoint = e.GetPosition(Map);

                double deltaX = deltaDownPoint.X - mouseCurrPoint.X;
                double deltaY = deltaDownPoint.Y - mouseCurrPoint.Y;

                CenterX = deltaX % 50;
                CenterY = deltaY % 50;

                int deltaCellIndexX = (int)deltaX / 50;
                int deltaCellIndexY = (int)deltaY / 50;

                if (Math.Abs(deltaCellIndexX) >= 1 || Math.Abs(deltaCellIndexY) >= 1)
                {
                    foreach (MapCell cell in Cells)
                    {
                        cell.ChangeCellIndex(deltaCellIndexX, deltaCellIndexY);
                    }

                    deltaDownPoint = mouseCurrPoint;

                    deltaDownPoint.X += CenterX;
                    deltaDownPoint.Y += CenterY;
                }
            }
        }

        void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;

            Map.ReleaseMouseCapture();
        }

        void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            deltaDownPoint = e.GetPosition(Map);

            deltaDownPoint.X += CenterX;
            deltaDownPoint.Y += CenterY;

            Map.CaptureMouse();
        }
    }
}
