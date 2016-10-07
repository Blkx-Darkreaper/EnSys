using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Collections.Generic;
using MapMaker;

namespace Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void LoadTileset()
        {
            MapMakerForm app = new MapMakerForm();

            Size mapSize = new Size(96, 96);
            app.MapDisplay.Size = mapSize;

            string filename = @"C:\Users\NicB\Documents\Visual Studio 2013\Projects\MapMaker\tilea2.png";
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
            string tilesetFilename = "C:\\Users\\NicB\\Documents\\Visual Studio 2013\\Projects\\MapMaker\\tilea2.png";
            int tileLength = 32;
            Size mapSize = new Size(96, 96);
            List<Grid> allMapGrids = new List<Grid>();

            Tile grass = new Tile(0, new Point(0, 0));
            Tile cobblestones = new Tile(2, new Point(128, 0));

            for (int i = 0; i < 9; i++)
            {
                int x = (i % 3)* tileLength;
                int y = (i / 3)* tileLength;

                Grid gridToAdd;
                if (i != 4)
                {
                    gridToAdd = new Grid(new Point(x, y), grass);
                }
                else
                {
                    gridToAdd = new Grid(new Point(x, y), cobblestones);
                }

                allMapGrids.Add(gridToAdd);
            }

            int nextSector = Grid.NextSector;

            StrikeforceMap map = new StrikeforceMap(author, dateCreated, tilesetFilename, tileLength, nextSector, mapSize, allMapGrids);

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

            string filename = @"C:\Users\NicB\Documents\Visual Studio 2013\Projects\MapMaker\tilea2.png";
            int TileLength = 32;
            int tilesetDisplayWidth = app.TilesetDisplay.Width;

            Program.LoadTileset(filename, TileLength, tilesetDisplayWidth);

            Program.BuildMap(mapSize);

            Grid twelve = Program.allMapGrids[12];
            int x = twelve.Corner.X;
            int y = twelve.Corner.Y;

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
    }
}
