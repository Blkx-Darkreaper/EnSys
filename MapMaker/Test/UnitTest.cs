using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using MapMaker;

namespace Test
{
    [TestClass]
    public class UnitTest
    {
        public void LoadTileset(int width = 96, int height = 96)
        {
            MapMakerForm app = new MapMakerForm();

            Size mapSize = new Size(width, height);
            app.MapDisplay.Size = mapSize;

            string filename = @"\tilea2.png";
            int TileLength = 32;
            int tilesetDisplayWidth = app.TilesetDisplay.Width;

            Program.LoadTileset(filename, TileLength, tilesetDisplayWidth);

            Program.BuildMap(mapSize);

            app.UpdateDisplay();
        }

        [TestMethod]
        public void Serialization()
        {
            string author = string.Empty;
            DateTime dateCreated = DateTime.Now;
            string tilesetFilename = @"\tilea2.png";
            int tileLength = 32;
            Size mapSize = new Size(96, 96);
            List<Grid> allMapGrids = new List<Grid>();
            List<Checkpoint> allCheckpoints = new List<Checkpoint>();
            List<Spawnpoint> allSpawnpoints = new List<Spawnpoint>();

            Tile grass = new Tile(0, 0, 0);
            Tile cobblestones = new Tile(2, 128, 0);

            for (int i = 0; i < 9; i++)
            {
                int x = (i % 3) * tileLength;
                int y = (i / 3) * tileLength;

                Grid gridToAdd;
                if (i != 4)
                {
                    gridToAdd = new Grid(x, y, grass);
                }
                else
                {
                    gridToAdd = new Grid(x, y, cobblestones);
                }

                allMapGrids.Add(gridToAdd);
            }

            int nextSector = Grid.NextSectorId;
            int nextZone = Grid.NextZoneId;

            StrikeforceMap map = new StrikeforceMap(author, dateCreated, tilesetFilename, tileLength, nextSector, nextZone, mapSize, allMapGrids, allCheckpoints);

            string json = Program.SerializeMap(map);

            StrikeforceMap mapCopy = Program.DeserializeMap(json);
        }

        [TestMethod]
        public void CircularBuffer()
        {
            int index = -1;
            int lastIndex = -1;
            int firstIndex = -1;
            int max = 5;

            List<string> buffer = new List<string>(max);

            for (int i = 0; i < max; i++)
            {
                buffer.Add(i.ToString());
                index = (index + 1) % max;
            }

            firstIndex = 0;
            lastIndex = index;

            string current;

            current = buffer[index];
            Assert.IsTrue(current.Equals("4"));

            // Action
            index = (index + 1) % max;
            lastIndex = index;

            if (index == firstIndex)
            {
                firstIndex++;
                firstIndex %= max;
            }

            buffer[index] = "5";

            current = buffer[index];
            Assert.IsTrue(current.Equals("5"));

            // Undo
            if (index != firstIndex)
            {
                index = (index - 1) % max;
                if (index < 0)
                {
                    index += max;
                }
            }

            current = buffer[index];
            Assert.IsTrue(current.Equals("4"));

            // Redo
            if (index != lastIndex)
            {
                index = (index + 1) % max;
            }

            current = buffer[index];
            Assert.IsTrue(current.Equals("5"));

            // Undo
            if (index != firstIndex)
            {
                index = (index - 1) % max;
                if (index < 0)
                {
                    index += max;
                }
            }

            current = buffer[index];
            Assert.IsTrue(current.Equals("4"));

            // Undo
            if (index != firstIndex)
            {
                index = (index - 1) % max;
                if (index < 0)
                {
                    index += max;
                }
            }

            current = buffer[index];
            Assert.IsTrue(current.Equals("3"));

            // Action
            index = (index + 1) % max;
            lastIndex = index;

            if (index == firstIndex)
            {
                firstIndex++;
                firstIndex %= max;
            }

            buffer[index] = "6";

            current = buffer[index];
            Assert.IsTrue(current.Equals("6"));
        }

        [TestMethod]
        public void GetAdjacentGrids()
        {
            MapMakerForm app = new MapMakerForm();

            Size mapSize = new Size(160, 192);
            app.MapDisplay.Size = mapSize;

            string filename = @"\tilea2.png";
            int TileLength = 32;
            int tilesetDisplayWidth = app.TilesetDisplay.Width;

            Program.LoadTileset(filename, TileLength, tilesetDisplayWidth);

            Program.BuildMap(mapSize);

            Grid twelve = Program.AllMapGrids[12];
            int x = twelve.Location.X;
            int y = twelve.Location.Y;

            int[] adjacentIndexes = Program.GetAdjacentGridIndexes(twelve);

            int above = adjacentIndexes[0];
            Assert.IsTrue(above == 7);

            int right = adjacentIndexes[1];
            Assert.IsTrue(right == 13);

            int below = adjacentIndexes[2];
            Assert.IsTrue(below == 17);

            int left = adjacentIndexes[3];
            Assert.IsTrue(left == 11);
        }

        [TestMethod]
        public void GetColourFromHsv()
        {
            int alpha, red, green, blue;
            float hue, saturation, brightness;

            Color cyan = Color.Cyan;
            alpha = cyan.A;
            red = cyan.R;
            green = cyan.G;
            blue = cyan.B;

            hue = cyan.GetHue();
            saturation = cyan.GetSaturation();
            brightness = cyan.GetBrightness();
            //brightness = Program.GetBrightness(cyan);

            Color cyanCopy = Program.GetColourFromHsl(alpha, hue, saturation, brightness);
            Assert.IsTrue(cyanCopy.R == red);
            Assert.IsTrue(cyanCopy.G == green);
            Assert.IsTrue(cyanCopy.B == blue);

            Color honeyDew = Color.Honeydew;
            alpha = honeyDew.A;
            red = honeyDew.R;
            green = honeyDew.G;
            blue = honeyDew.B;

            hue = honeyDew.GetHue();
            saturation = honeyDew.GetSaturation();
            brightness = Program.GetBrightness(honeyDew);

            Color honeyDewCopy = Program.GetColourFromHsl(alpha, hue, saturation, brightness);
            Assert.IsTrue(honeyDewCopy.R == red);
            Assert.IsTrue(honeyDewCopy.G == green);
            Assert.IsTrue(honeyDewCopy.B == blue);
        }

        [TestMethod]
        public void GetNumberFromChar()
        {
            char a = 'a';
            char capA = 'A';

            int numberA = Program.CharToNumber(a);

            int numberCapA = Program.CharToNumber(capA);

            char copyA = Program.NumberToChar(numberA);
            Assert.IsTrue(Char.ToUpper(a).Equals(copyA));

            char copyCapA = Program.NumberToChar(numberCapA);
            Assert.IsTrue(capA.Equals(copyCapA));
        }

        [TestMethod]
        public void ColourRoulette()
        {
            int totalColours = 5;
            Program.InitColourRoulette(totalColours);

            for (int i = 0; i < totalColours; i++)
            {
                int colourIndex = i % Program.ColourRoulette.Count;
                Color colour = Program.ColourRoulette[colourIndex];
            }
        }

        //[TestMethod]
        //public void GetSectorShape()
        //{
        //    Sector zone = new Sector(0);

        //    Grid grid;
        //    int length = 32;

        //    grid = new Grid(new Point(0, 0));
        //    zone.AddGrid(grid, length);

        //    grid = new Grid(new Point(32, 0));
        //    zone.AddGrid(grid, length);

        //    grid = new Grid(new Point(0, 32));
        //    zone.AddGrid(grid, length);

        //    grid = new Grid(new Point(64, 32));
        //    zone.AddGrid(grid, length);

        //    grid = new Grid(new Point(64, 0));
        //    zone.AddGrid(grid, length);

        //    GraphicsPath bounds = zone.GetShape(1);
        //    for (int i = 0; i < bounds.PathPoints.Length; i++)
        //    {
        //        PointF point = bounds.PathPoints[i];
        //        int type = (int)bounds.PathTypes[i];
        //    }
        //}

        [TestMethod]
        public void GetGridsInArea()
        {
            LoadTileset(640, 800);

            // 4 deltaX 2
            Point start = new Point(40, 40);
            Point end = new Point(136, 80);

            double scale = 1.0;

            List<Grid> selectedGrids = Program.GetGridsInArea(start, end, scale);
            Assert.IsTrue(selectedGrids.Count == 8);

            Grid first = selectedGrids[0];
            Assert.IsTrue(first.Location.X == 32);
            Assert.IsTrue(first.Location.Y == 32);

            Grid last = selectedGrids[selectedGrids.Count - 1];
            Assert.IsTrue(last.Location.X == 160);
            Assert.IsTrue(last.Location.Y == 64);
        }

        [TestMethod]
        public void InitSectors()
        {
            LoadTileset(128, 128);

            double scale = 1.0;
            Point start, end;

            start = new Point(0, 0);
            end = new Point(64, 64);
            List<Grid> quadrant1 = Program.GetGridsInArea(start, end, scale);
            foreach (Grid grid in quadrant1)
            {
                grid.SectorId = 1;
            }

            start = new Point(64, 0);
            end = new Point(128, 64);
            List<Grid> quadrant2 = Program.GetGridsInArea(start, end, scale);
            foreach (Grid grid in quadrant2)
            {
                grid.SectorId = 2;
            }

            start = new Point(0, 64);
            end = new Point(128, 128);
            List<Grid> quadrant0 = Program.GetGridsInArea(start, end, scale);
            foreach (Grid grid in quadrant0)
            {
                grid.SectorId = 0;
            }

            Program.InitSectors();

            Rectangle bounds;
            int x, y, width, height;

            //Sector sector0 = Program.AllSectors[0];
            //bounds = sector0.Area;
            //x = bounds.Location.X;
            //Assert.IsTrue(x == 0);

            //y = bounds.Location.Y;
            //Assert.IsTrue(y == 64);

            //width = bounds.Width;
            //Assert.IsTrue(width == (4 * 32));

            //height = bounds.Height;
            //Assert.IsTrue(height == (2 * 32));

            Sector sector1 = Program.AllSectors[1];
            bounds = sector1.PixelArea;
            x = bounds.Location.X;
            Assert.IsTrue(x == 0);

            y = bounds.Location.Y;
            Assert.IsTrue(y == 0);

            width = bounds.Width;
            Assert.IsTrue(width == (2 * 32));

            height = bounds.Height;
            Assert.IsTrue(height == (2 * 32));

            Sector sector2 = Program.AllSectors[2];
            bounds = sector2.PixelArea;
            x = bounds.Location.X;
            Assert.IsTrue(x == 64);

            y = bounds.Location.Y;
            Assert.IsTrue(y == 0);

            width = bounds.Width;
            Assert.IsTrue(width == (2 * 32));

            height = bounds.Height;
            Assert.IsTrue(height == (2 * 32));
        }

        [TestMethod]
        public void GetDifference()
        {
            Rectangle rect1 = new Rectangle(10, 10, 25, 20);
            Rectangle rect2;
            Rectangle[] diff;

            // Move North
            rect2 = new Rectangle(10, 8, 25, 20);
            diff = Program.Difference(rect1, rect2);
        }
    }
}