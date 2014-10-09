using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfDemo.CellGen
{
    public class MapPage
    {
        public char Letter { get; private set; }
        public int Sector { get; private set; }
        public string Square { get; private set; }

        public MapPage Top { get; private set; }
        public MapPage Right { get; private set; }
        public MapPage Bottom { get; private set; }
        public MapPage Left { get; private set; }

        private int LetterIndex { get; set; }
        private int SectorIndex { get; set; }
        private int SquareIndex { get; set; }

        private static readonly char[] Letters = new char[] { 'N', 'M', 'L' };
        private static readonly int[] Sectors = new int[] { 34, 35, 36, 37 };
        private static readonly string[] Squares = Enumerable.Range(1, 144).Select(squareId =>
        {
            int localSquareId = squareId;
            string ret = squareId.ToString();

            while (localSquareId < 100)
            {
                localSquareId *= 10;
                ret = "0" + ret;
            }

            return ret;
        }).ToArray();

        private static readonly int NumSquaresInLine = 12;

        private static readonly Dictionary<char, Dictionary<int, Dictionary<string, MapPage>>> GlobMap
            = new Dictionary<char, Dictionary<int, Dictionary<string, MapPage>>>();

        static MapPage()
        {
            GenGlobMap();
        }

        private MapPage() { }

        private MapPage(int letterIndex, int sectorIndex, int squreIndex)
        {
            Letter = MapPage.Letters[letterIndex];
            Sector = MapPage.Sectors[sectorIndex];
            Square = MapPage.Squares[squreIndex];

            LetterIndex = letterIndex;
            SectorIndex = sectorIndex;
            SquareIndex = squreIndex;
        }

        private static void GenGlobMap()
        {
            Enumerable.Range(0, MapPage.Letters.Length)
                .SelectMany(letterIndex => Enumerable.Range(0, MapPage.Sectors.Length)
                    .SelectMany(sectorIndex => Enumerable.Range(0, MapPage.Squares.Length)
                        .Select(squreIndex =>
                            new MapPage(letterIndex, sectorIndex, squreIndex))))
                        .ToList().ForEach(map =>
                        {
                            if (!GlobMap.ContainsKey(map.Letter))
                                GlobMap.Add(map.Letter, new Dictionary<int, Dictionary<string, MapPage>>());

                            if (!GlobMap[map.Letter].ContainsKey(map.Sector))
                                GlobMap[map.Letter].Add(map.Sector, new Dictionary<string, MapPage>());

                            if (!GlobMap[map.Letter][map.Sector].ContainsKey(map.Square))
                                GlobMap[map.Letter][map.Sector].Add(map.Square, map);
                        });

            GlobMap.ToList().ForEach(kv1 => kv1.Value.ToList().ForEach(kv2 => kv2.Value.ToList().ForEach(kv3 =>
            {
                MapPage map = kv3.Value;
                map.Top = GetTop(map);
                map.Right = GetRight(map);
                map.Bottom = GetBottom(map);
                map.Left = GetLeft(map);
            })));
        }

        public static MapPage GetMapPage(string mapPageDescriptor)
        {
            Regex regex = new Regex(@"(\w)-(\d+)-(\d+)");

            string localMapPageDescriptor = mapPageDescriptor.ToUpper();

            if (!regex.IsMatch(localMapPageDescriptor))
            {
                return null;
            }

            Match match = regex.Match(localMapPageDescriptor);

            char letter = match.Groups[1].Value.ToCharArray()[0];
            int sector = int.Parse(match.Groups[2].Value);
            string square = match.Groups[3].Value;

            return GetMapPage(letter, sector, square);
        }

        public static MapPage GetMapPage(char letter, int sector, string square)
        {
            return GlobMap.ContainsKey(letter) && GlobMap[letter].ContainsKey(sector) &&
                GlobMap[letter][sector].ContainsKey(square) ? GlobMap[letter][sector][square] : null;
        }

        private static MapPage GetTop(MapPage center)
        {
            int squareIndex = (center.SquareIndex - NumSquaresInLine + Squares.Length) % Squares.Length;

            int sectorIndex = center.SectorIndex;

            int letterIndex = center.SquareIndex - NumSquaresInLine < 0 ?
                (center.LetterIndex - 1 + Letters.Length) % Letters.Length :
                center.LetterIndex;

            char letter = Letters[letterIndex];
            int sector = Sectors[sectorIndex];
            string square = Squares[squareIndex];

            return GetMapPage(letter, sector, square);
        }

        private static MapPage GetRight(MapPage center)
        {
            int squareIndex = (center.SquareIndex + 1) % NumSquaresInLine == 0 ?
                center.SquareIndex - NumSquaresInLine + 1 :
                center.SquareIndex + 1;

            int sectorIndex = (center.SquareIndex + 1) % NumSquaresInLine == 0 ?
                (center.SectorIndex + 1) % Sectors.Length :
                center.SectorIndex;

            int letterIndex = center.LetterIndex;

            char letter = Letters[letterIndex];
            int sector = Sectors[sectorIndex];
            string square = Squares[squareIndex];

            return GetMapPage(letter, sector, square);
        }

        private static MapPage GetBottom(MapPage center)
        {
            int squareIndex = (center.SquareIndex + NumSquaresInLine) % Squares.Length;

            int sectorIndex = center.SectorIndex;

            int letterIndex = center.SquareIndex + NumSquaresInLine >= Squares.Length ?
                (center.LetterIndex + 1) % Letters.Length :
                center.LetterIndex;

            char letter = Letters[letterIndex];
            int sector = Sectors[sectorIndex];
            string square = Squares[squareIndex];

            return GetMapPage(letter, sector, square);
        }

        private static MapPage GetLeft(MapPage center)
        {
            int squareIndex = center.SquareIndex % NumSquaresInLine == 0 ?
                center.SquareIndex + NumSquaresInLine - 1 :
                center.SquareIndex - 1;

            int sectorIndex = center.SquareIndex % NumSquaresInLine == 0 ?
                (center.SectorIndex - 1 + Sectors.Length) % Sectors.Length :
                center.SectorIndex;

            int letterIndex = center.LetterIndex;

            char letter = Letters[letterIndex];
            int sector = Sectors[sectorIndex];
            string square = Squares[squareIndex];

            return GetMapPage(letter, sector, square);
        }
    }
}
