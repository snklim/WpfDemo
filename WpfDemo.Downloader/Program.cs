using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfDemo.CellGen;

namespace WpfDemo.Downloader
{
    class Program
    {
        class DownloadTarget
        {
            public MapPage BeginMapPage { get; private set; }
            public int NumberToDownload { get; private set; }

            public DownloadTarget(string beginMapPage, int numberToDownload)
            {
                BeginMapPage = MapPage.GetMapPage(beginMapPage);
                NumberToDownload = numberToDownload;
            }
        }

        class Point
        {
            public static int _minX = int.MaxValue;
            public static int _minY = int.MaxValue;

            public static int _maxX = int.MinValue;
            public static int _maxY = int.MinValue;

            private int _x;
            private int _y;

            public int X { get { return _x - _minX; } }
            public int Y { get { return _y - _minY; } }

            public Point(int x, int y)
            {
                _x = x;
                _y = y;

                _minX = Math.Min(x, _minX);
                _minY = Math.Min(y, _minY);

                _maxX = Math.Max(x, _maxX);
                _maxY = Math.Max(y, _maxY);
            }
        }

        static void Main(string[] args)
        {
            Regex regex = new Regex("<area.*href=\"(.*?)\".*>", RegexOptions.Multiline);
            Regex regexMap = new Regex(@"(\w-\d+-\d+)");

            Dictionary<string, MapPage> ukraine = new Dictionary<string, MapPage>();

            using (System.IO.StreamReader streamReader =
                new System.IO.StreamReader(@"C:\work\WpfDemo\WpfDemo.Downloader\bin\Debug\map.html"))
            {
                string htmlPage = streamReader.ReadToEnd();
                if (regex.IsMatch(htmlPage))
                {
                    foreach (Match match in regex.Matches(htmlPage))
                    {
                        string url = match.Groups[1].Value;
                        if (regexMap.IsMatch(url))
                        {
                            Match matchUrl = regexMap.Match(url);
                            string mapPageId = matchUrl.Groups[1].Value.ToUpper();
                            ukraine.Add(mapPageId, MapPage.GetMapPage(mapPageId));
                        }
                    }
                }
            }

            MapPage mapPageBegin = MapPage.GetMapPage("N-34-001");
            MapPage mapPageCurrent = mapPageBegin;

            int numbers = ukraine.Count;

            int x = 0;
            int y = 0;

            List<Point> shape = new List<Point>();

            while (numbers > 0)
            {
                string mapPageId = string.Format("{0}-{1}-{2}", mapPageCurrent.Letter, mapPageCurrent.Sector, mapPageCurrent.Square);

                y++;

                if (ukraine.ContainsKey(mapPageId))
                {
                    numbers--;

                    shape.Add(new Point(x, y));
                }

                mapPageCurrent = mapPageCurrent.Bottom;

                if (mapPageCurrent == mapPageBegin)
                {
                    mapPageCurrent = mapPageCurrent.Right;
                    mapPageBegin = mapPageCurrent;
                    
                    x++;
                    y = 0;
                }

            }

            foreach (var point in shape)
            {
                Console.SetCursorPosition(2*point.X, point.Y);
                Console.WriteLine("xx");
            }

            Console.SetCursorPosition(0, Point._maxY - Point._minY + 1);
        }
    }
}
