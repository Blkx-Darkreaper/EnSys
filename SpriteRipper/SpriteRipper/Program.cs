using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace SpriteRipper
{
    public static class Program
    {
        //public static Bitmap loadedImage { get; set; }
        //public static Bitmap CurrentSubImage { get; set; }
        //public static int TotalSubImages { get; set; }
        //public static Size CurrentSubImageSize { get; set; }
        public static ImageCollection Images { get; set; }
        //private static List<Tile> allSubImageTiles { get; set; }
        //private static LinkedList<Tile> allSubImageTiles { get; set; }
        private static SortedSet<Tile> allSubImageTiles { get; set; }
        private static ConcurrentBag<Tile> tileDepository { get; set; }
        public static bool DisplayReady { get; private set; }
        private static TileSorting allSortedTiles { get; set; }
        public static bool TilesetReady { get; private set; }
        public static Color BackgroundColour { get; set; }
        private static int ACCURACY = 2;
        public static int TasksComplete { get; private set; }
        public static int TotalTasks { get; private set; }
        public static int Duplicates { get; private set; }
        public static long SortTime { get; private set; }
        private const string IMAGE_FILTER = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif)|*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif";
        private const int MAX_THREADS = 4;
        private const int MAX_CONCURRENT_TILES = 225;
        private const int MAX_PIXELS = 57600;

        static Program()
        {
            allSubImageTiles = null;
            tileDepository = null;
            allSortedTiles = null;
            Images = null;

            DisplayReady = false;
            TilesetReady = false;

            // default colour
            BackgroundColour = Color.LightSeaGreen;

            TasksComplete = 0;
            TotalTasks = 0;
            Duplicates = 0;
            SortTime = 0;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Gui());
        }

        public static int GetTileCount()
        {
            if (allSubImageTiles == null)
            {
                return 0;
            }

            return allSubImageTiles.Count;
        }

        public static int GetSortedTileCount()
        {
            if (allSortedTiles == null)
            {
                return 0;
            }

            return allSortedTiles.TileCount;
        }

        public static int GetGroupCount()
        {
            if (allSortedTiles == null)
            {
                return 0;
            }

            return allSortedTiles.GroupCount;
        }

        public static int GetCountOfGroup(int groupIndex)
        {
            return allSortedTiles.allTileGroups[groupIndex].Count;
        }

        public static OpenFileDialog GetFilenameToOpen()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            // Set filter options
            dialog.Filter = IMAGE_FILTER;

            dialog.Multiselect = false;

            return dialog;
        }

        public static SaveFileDialog GetFilenameToSave()
        {
            SaveFileDialog dialog = new SaveFileDialog();

            // Set filter options
            dialog.Filter = IMAGE_FILTER;

            return dialog;
        }

        public static ImageFormat GetImageFormat(string filename)
        {
            filename = filename.ToLower();
            int start = filename.IndexOf(".") + 1;
            int length = filename.Length - start;
            string extension = filename.Substring(start, length);

            ImageFormat format;
            switch (extension)
            {
                case "bmp":
                    format = ImageFormat.Bmp;
                    break;

                case "gif":
                    format = ImageFormat.Gif;
                    break;

                case "jpg":
                case "jpeg":
                    format = ImageFormat.Jpeg;
                    break;

                case "png":
                    format = ImageFormat.Png;
                    break;

                case "tif":
                    format = ImageFormat.Tiff;
                    break;

                default:
                    throw new Exception("Invalid file format");
            }

            return format;
        }

        public static Bitmap LoadImage(string path)
        {
            Bitmap imageToLoad = new Bitmap(path);
            return imageToLoad;
        }

        public static void LoadImage(string path, int tileSize, int offsetX, int offsetY)
        {
            Bitmap image = LoadCroppedImage(path, tileSize, offsetX, offsetY);
            int width = image.Width;
            int height = image.Height;

            Images = new ImageCollection(path, tileSize, width, height, offsetX, offsetY);

            allSortedTiles = new TileSorting();

            Images.SetSubImageByRefFromImage(ref image, 0);

            //loadedImage = canvas;
        }

        public static Bitmap LoadCroppedImage(string filename, int tileSize, int offsetX, int offsetY)
        {
            Bitmap image = new Bitmap(filename);

            int width = image.Width;
            int height = image.Height;

            int croppedWidth = tileSize * ((width - offsetX) / tileSize);
            int croppedHeight = tileSize * ((height - offsetY) / tileSize);

            if (croppedWidth != width || croppedHeight != height)
            {
                Rectangle rect = new Rectangle(offsetX, offsetY, croppedWidth, croppedHeight);
                //Bitmap croppedImage = canvas.Clone(rect, PixelFormat.Format24bppRgb);
                Bitmap croppedImage = new Bitmap(croppedWidth, croppedHeight, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(croppedImage))
                {
                    graphics.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);
                }

                image = croppedImage;
            }

            return image;
        }

        public static void LoadSubImage(int bitsPerColour, int tileSize)
        {
            LoadSubImage(bitsPerColour, tileSize, 0);
        }

        public static void LoadSubImage(int bitsPerColour, int tileSize, int subImageIndex)
        {
            if (Images == null)
            {
                throw new NullReferenceException("No image loaded");
            }

            //allSubImageTiles = new List<Tile>();
            //allSubImageTiles = new LinkedList<Tile>();
            allSubImageTiles = new SortedSet<Tile>();
            //tileDepository = new ConcurrentBag<Tile>();   //threaded

            Images.LoadSubImage(subImageIndex);

            LoadAllTilesBySubImage(bitsPerColour, tileSize, subImageIndex);
        }

        private static void LoadAllTilesBySubImage(int bitsPerColour, int tileSize, int subImageIndex)
        {
            if (Images == null)
            {
                throw new NullReferenceException("No image loaded");
            }

            DisplayReady = false;

            int totalSubImageTiles = Images.GetSubImageTileCount(subImageIndex);
            for (int i = 0; i < totalSubImageTiles; i++)
            {
                int tileIndex = Images.GetTileIndex(subImageIndex, i);

                Tile tileToAdd;
                try
                {
                    //tileToAdd = GetTile(bitsPerColour, tileSize, tileIndex);
                    tileToAdd = GetTileFromSubImage(bitsPerColour, tileSize, tileIndex, subImageIndex, i);
                }
                catch (OutOfMemoryException ex)
                {
                    MessageBox.Show(string.Format("Error loading tiles. {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                allSubImageTiles.Add(tileToAdd);

                TasksComplete++;
            }

            DisplayReady = true;
        }

        //public static void GetSubImage(int BitsPerColour, int TileSize, int subImageIndex)
        //{
        //    if (loadedImage == null)
        //    {
        //        throw new NullReferenceException("No canvas loaded");
        //    }

        //    DisplayReady = false;
        //    int imageWidth = loadedImage.Width;
        //    int imageHeight = loadedImage.Height;
        //    //int totalTiles = Program.GetTileCount(imageWidth, imageHeight, TileSize);

        //    //int subImageWidth = imageWidth;
        //    //int subImageHeight = imageHeight;
        //    //int totalPixels = imageWidth * imageHeight;

        //    //while (totalPixels > MAX_PIXELS)
        //    ////while (totalTiles > MAX_CONCURRENT_TILES)
        //    //{
        //    //    int halfSubWidth = subImageWidth / 2;
        //    //    int halfSubHeight = subImageHeight / 2;

        //    //    if (halfSubWidth % TileSize != 0)
        //    //    {
        //    //        if (halfSubHeight % TileSize != 0)
        //    //        {
        //    //            break;
        //    //        }
        //    //    }

        //    //    if (halfSubWidth % TileSize == 0)
        //    //    {
        //    //        subImageWidth = halfSubWidth;
        //    //    }
        //    //    if (halfSubHeight % TileSize == 0)
        //    //    {
        //    //        subImageHeight = halfSubHeight;
        //    //    }

        //    //    totalPixels = subImageWidth * subImageHeight;
        //    //    //totalTiles = Program.GetTileCount(subImageWidth, subImageHeight, TileSize);
        //    //}

        //    if (CurrentSubImageSize == null)
        //    {
        //        CurrentSubImageSize = Program.GetSubImageSize(imageWidth, imageHeight, TileSize);
        //    }

        //    int subImageWidth = CurrentSubImageSize.Width;
        //    int subImageHeight = CurrentSubImageSize.Height;

        //    int subImagesWide = imageWidth / subImageWidth;
        //    TotalSubImages = subImagesWide * (imageHeight / subImageHeight);

        //    int x = (subImageIndex % subImagesWide) * subImageWidth;
        //    int y = (subImageIndex / subImagesWide) * subImageHeight;
        //    Rectangle rect = new Rectangle(x, y, subImageWidth, subImageHeight);
        //    Bitmap subImage = loadedImage.Clone(rect, PixelFormat.Format24bppRgb);
        //    CurrentSubImage = subImage;
        //    loadedImage = null; //testing

        //    //allSubImageTiles = new List<Tile>();
        //    //allSubImageTiles = new LinkedList<Tile>();
        //    allSubImageTiles = new SortedSet<Tile>();
        //    //tileDepository = new ConcurrentBag<Tile>();   //threaded

        //    LoadTilesByRef(ref subImage, BitsPerColour, TileSize);

        //    DisplayReady = true;
        //}

        //public static Size GetSubImageSize(int imageWidth, int imageHeight, int tileSize)
        //{
        //    int totalPixels = imageWidth * imageHeight;
        //    if (totalPixels <= MAX_PIXELS)
        //    {
        //        return new Size(imageWidth, imageHeight);
        //    }

        //    int widthDivisor = 1;
        //    int heightDivisor = 1;

        //    int tilesWide = imageWidth / tileSize;
        //    int tilesHigh = imageHeight / tileSize;

        //    widthDivisor = GetNextDivisor(widthDivisor, tilesWide);
        //    heightDivisor = GetNextDivisor(heightDivisor, tilesHigh);

        //    int subImageWidth = imageWidth / widthDivisor;
        //    int subImageHeight = imageHeight / heightDivisor;

        //    totalPixels = subImageWidth * subImageHeight;
        //    while (totalPixels > MAX_PIXELS)
        //    {
        //        if (widthDivisor < heightDivisor)
        //        {
        //            int nextWidthDivisor = GetNextDivisor(widthDivisor, tilesWide);
        //            if (nextWidthDivisor != -1)
        //            {
        //                widthDivisor = nextWidthDivisor;
        //            }
        //        }
        //        else if (widthDivisor > heightDivisor)
        //        {
        //            int nextHeightDivisor = GetNextDivisor(heightDivisor, tilesHigh);
        //            if (nextHeightDivisor != -1)
        //            {
        //                heightDivisor = nextHeightDivisor;
        //            }
        //        }
        //        else
        //        {
        //            int nextWidthDivisor = GetNextDivisor(widthDivisor, tilesWide);
        //            if (nextWidthDivisor != -1)
        //            {
        //                widthDivisor = nextWidthDivisor;
        //            }

        //            int nextHeightDivisor = GetNextDivisor(heightDivisor, tilesHigh);
        //            if (nextHeightDivisor != -1)
        //            {
        //                heightDivisor = nextHeightDivisor;
        //            }
        //        }

        //        subImageWidth = imageWidth / widthDivisor;
        //        subImageHeight = imageHeight / heightDivisor;

        //        totalPixels = subImageWidth * subImageHeight;
        //    }

        //    return new Size(subImageWidth, subImageHeight);
        //}

        private static int GetNextDivisor(int divisor, int divided)
        {
            int denominator = divisor + 1;
            while (divided % denominator != 0)
            {
                denominator++;

                if (denominator >= divided)
                {
                    return -1;
                    //throw new ArgumentOutOfRangeException("No greater divisor");
                }
            }

            return denominator;
        }

        public static void ResetAll()
        {
            //allSubImageTiles = new List<Tile>();
            //allSubImageTiles = new LinkedList<Tile>();
            allSubImageTiles = new SortedSet<Tile>();
            DisplayReady = false;

            Images = null;

            ResetTileset();
        }

        public static void ResetTileset()
        {
            allSortedTiles = new TileSorting();
            TilesetReady = false;
        }

        public static Bitmap GetGroupedTileset(PixelFormat format, int tileSize, int zoom, bool addPadding)
        {
            if (allSortedTiles == null)
            {
                return null;
            }

            int maxTilesWide = allSortedTiles.GetMaxGroupSize();
            int totalGroups = allSortedTiles.Count;

            int width = maxTilesWide * tileSize * zoom;
            if (addPadding == true)
            {
                width += maxTilesWide - 1;
            }
            int height = totalGroups * tileSize * zoom;
            if (addPadding == true)
            {
                height += 5 * (totalGroups - 1);
            }

            Bitmap tileset = new Bitmap(width, height, format);

            // Fill with background colour
            SetBackgroundColour(width, height, tileset);

            // Draw tileToCompare groups in tileset
            for (int i = 0; i < allSortedTiles.Count; i++)
            {
                TileGroup group = allSortedTiles[i];
                int masterIndex = group.MasterIndex;

                // Get Tile
                //Tile Master = allSubImageTiles[MasterIndex];
                Bitmap masterTileImage = Images.GetTileImage(masterIndex);

                // draw tile onto tileset
                int x = 0;
                int y;
                int padding = 0;
                if (addPadding == true)
                {
                    padding = 5;
                }

                y = i * (tileSize * zoom + padding);
                //DrawTileOntoImage(ref tileset, Master, x, y, zoom);
                DrawImageOntoCanvas(ref tileset, ref masterTileImage, x, y, zoom);

                List<int> similarTiles = group.GetSortedTiles();
                // draw similar tiles
                for (int j = 0; j < similarTiles.Count; j++)
                {
                    padding = 0;
                    if (addPadding == true)
                    {
                        padding = 1;
                    }

                    x = tileSize * zoom + padding + j * (tileSize * zoom + padding);

                    int tileIndex = similarTiles[j];
                    //Tile tileToDraw = allSubImageTiles[tileIndex];
                    //Tile tileToDraw = allSubImageTiles.ElementAt(tileIndex);
                    //DrawTileOntoImage(ref tileset, tileToDraw, x, y, zoom);
                    Bitmap tileImage = Images.GetTileImage(tileIndex);
                    DrawImageOntoCanvas(ref tileset, ref tileImage, x, y, zoom);
                }
            }

            return tileset;
        }

        public static Bitmap GetTileset(PixelFormat format, int tileSize, int tilesWide, int zoom, bool addPadding)
        {
            int totalTiles = allSortedTiles.TileCount;

            int width = tilesWide * tileSize * zoom;
            if (addPadding == true)
            {
                width += tilesWide - 1;
            }

            int rows = (int)Math.Ceiling((double)totalTiles / (double)tilesWide);
            int height = rows * tileSize * zoom;
            if (addPadding == true)
            {
                height += rows - 1;
            }

            Bitmap tileset = new Bitmap(width, height, format);

            // Fill with background colour
            SetBackgroundColour(width, height, tileset);

            // Draw tileToCompare groups in sequence
            int tileNumber = 0;
            for (int i = 0; i < allSortedTiles.Count; i++)
            {
                // draw tileToDraw
                TileGroup group = allSortedTiles[i];
                int masterIndex = group.MasterIndex;
                //Tile Master = allSubImageTiles[MasterIndex];
                //Tile Master = allSubImageTiles.ElementAt(MasterIndex);
                //DrawNextTileOntoImage(tileset, Master, tileSize, tilesWide, zoom, addPadding, ref tileNumber);
                Bitmap masterTileImage = Images.GetTileImage(masterIndex);
                DrawNextTileImageOntoCanvas(ref tileset, ref masterTileImage, tilesWide, zoom, addPadding, ref tileNumber);

                List<int> similarTiles = group.GetSortedTiles();
                // draw similar tiles
                for (int j = 0; j < similarTiles.Count; j++)
                {
                    int tileIndex = similarTiles[j];
                    //Tile tileToDraw = allSubImageTiles[tileIndex];
                    //Tile tileToDraw = allSubImageTiles.ElementAt(tileIndex);
                    //DrawNextTileOntoImage(tileset, tileToDraw, tileSize, tilesWide, zoom, addPadding, ref tileNumber);
                    Bitmap tileImage = Images.GetTileImage(tileIndex);
                    DrawNextTileImageOntoCanvas(ref tileset, ref tileImage, tilesWide, zoom, addPadding, ref tileNumber);
                }
            }

            return tileset;
        }

        private static void SetBackgroundColour(int width, int height, Bitmap tileset)
        {
            Graphics g = Graphics.FromImage(tileset);
            SolidBrush brush = new SolidBrush(BackgroundColour);
            g.FillRectangle(brush, 0, 0, width, height);
        }

        private static void DrawNextTileOntoImage(Bitmap tileset, Tile tileToDraw, int tileSize, int tilesWide, int zoom, bool addPadding, ref int count)
        {
            int x = (count % tilesWide) * tileSize * zoom;
            int y = (count / tilesWide) * tileSize * zoom;

            if (addPadding == true)
            {
                x += (count % tilesWide);
                y += (count / tilesWide);
            }

            DrawTileOntoImage(ref tileset, tileToDraw, x, y, zoom);
            count++;
        }

        private static void DrawNextTileImageOntoCanvas(ref Bitmap canvas, ref Bitmap nextTileImage, int tilesWide, int zoom, bool addPadding, ref int count)
        {
            int tileSize = Images.TileSize;

            int x = (count % tilesWide) * tileSize * zoom;
            int y = (count / tilesWide) * tileSize * zoom;

            if (addPadding == true)
            {
                x += (count % tilesWide);
                y += (count / tilesWide);
            }

            DrawImageOntoCanvas(ref canvas, ref nextTileImage, x, y, zoom);
            count++;
        }

        //private static Bitmap DrawGroupedTileset(PixelFormat format, int TileSize, int imageWidth, int imageHeight, int zoom)
        //{
        //    if (allSortedTiles == null)
        //    {
        //        return null;
        //    }

        //    Bitmap tileset = new Bitmap(imageWidth, imageHeight, format);

        //    // Draw tileToCompare groups in tileset
        //    for (int i = 0; i < allSortedTiles.Count; i++)
        //    {
        //        TileGroup group = allSortedTiles[i];
        //        int MasterIndex = group.MasterIndex;
        //        Tile tileToDraw = allSubImageTiles[MasterIndex];
        //        // draw tileToDraw
        //        int cornerX = 0;
        //        int cornerY = 1 + i * (TileSize * zoom + 5);
        //        DrawTileOntoImage(tileset, tileToDraw, cornerX, cornerY, zoom);

        //        List<int> similarTiles = group.GetSortedTiles();
        //        // draw similar tiles
        //        for (int j = 0; j < similarTiles.Count; j++)
        //        {
        //            cornerX = TileSize * zoom + 1 + j * (TileSize * zoom + 1);
        //            int tileIndex = similarTiles[j];
        //            Tile tileToDraw = allSubImageTiles[tileIndex];
        //            DrawTileOntoImage(tileset, tileToDraw, cornerX, cornerY, zoom);
        //        }
        //    }

        //    return tileset;
        //}

        public static void DrawAllTilesOntoImage(ref Bitmap image, int tilesWide, int tileSize, int zoom, bool addPadding)
        {
            for (int i = 0; i < allSubImageTiles.Count; i++)
            {
                //Tile tileToCompare = allSubImageTiles[i];
                Tile tile = allSubImageTiles.ElementAt(i);
                int x, y;

                if (addPadding == true)
                {
                    x = i % tilesWide * (tileSize * zoom + 1);
                    y = i / tilesWide * (tileSize * zoom + 1);
                }
                else
                {
                    x = i % tilesWide * (tileSize * zoom);
                    y = i / tilesWide * (tileSize * zoom);
                }

                DrawTileOntoImage(ref image, tile, x, y, zoom);
            }
        }

        private static void DrawTileOntoImage(ref Bitmap image, Tile tile, int x, int y)
        {
            DrawTileOntoImage(ref image, tile, x, y, 1);
        }

        private static void DrawTileOntoImage(ref Bitmap image, Tile tile, int x, int y, int zoom)
        {
                //Image image = tile.GetTileImage();
                int tileIndex = tile.Index;
                Bitmap tileImage = Program.GetTileImage(tileIndex);

                DrawImageOntoCanvas(ref image, ref tileImage, x, y, zoom);
        }

        private static void DrawImageOntoCanvas(ref Bitmap canvas, ref Bitmap image, int x, int y)
        {
            DrawImageOntoCanvas(ref canvas, ref image, x, y, 1);
        }

        private static void DrawImageOntoCanvas(ref Bitmap canvas, ref Bitmap image, int x, int y, int zoom)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                int width = image.Width * zoom;
                int height = image.Height * zoom;

                g.DrawImage(image, x, y, width, height);
            }
        }

        public static void SortTiles(float patternThreshold, float colourThreshold)
        {
            TilesetReady = false;
            TasksComplete = 0;
            TotalTasks = allSubImageTiles.Count;
            Duplicates = 0;
            SortTime = 0;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            if (allSortedTiles == null)
            {
                allSortedTiles = new TileSorting();
            }

            //while(allSubImageTiles.Count > 0)
            for (int i = 0; i < allSubImageTiles.Count; i++)
            {
                //Tile tileToCompare = allSubImageTiles[i];
                Tile tile = allSubImageTiles.ElementAt(i);
                //int index = tile.Index;
                //Tile tileToCompare = allSubImageTiles.First.Value;
                //allSubImageTiles.RemoveFirst();

                if (i == 0)
                {
                    //allSortedTiles.AddGroup(index);
                    allSortedTiles.AddGroup(tile);
                    TasksComplete++;
                    continue;
                }

                //TileGroup group = FindMatchingGroup(tile, patternThreshold, colourThreshold);
                TileGroupMatchResults results = FindMatchingGroupWithResults(tile, patternThreshold, colourThreshold);
                TileGroup group = results.group;
                if (group == null)
                {
                    // Add to new group
                    //allSortedTiles.AddGroup(index);
                    allSortedTiles.AddGroup(tile);
                    TasksComplete++;
                    continue;
                }

                // Convert pattern match to hashcode
                //int MasterIndex = group.MasterIndex;
                ////Tile Master = allSubImageTiles[MasterIndex];
                //Tile Master = allSubImageTiles.ElementAt(MasterIndex);
                //Tuple<float, float> results = Master.GetMatches(tile);
                //float patternMatch = results.Item1;
                //float colourMatch = results.Item2;

                float patternMatch = results.patternMatch;
                float colourMatch = results.colourMatch;

                bool areIdentical = Tile.IdenticalTo(patternMatch, colourMatch);
                if (areIdentical == true)
                {
                    Duplicates++;
                    TasksComplete++;
                    continue;
                }

                // If tilecount is saved for each group, could improve performance by reducing the hashcode TileSize since tilecount gets smaller with time
                int key = Tile.GetHashcode(ACCURACY, patternMatch, colourMatch, TotalTasks);
                AddSimilarTileToGroup(tile, group, key);
                TasksComplete++;
            }

            timer.Stop();
            SortTime = timer.ElapsedMilliseconds;

            TilesetReady = true;
        }

        private static void AddSimilarTileToGroup(Tile tile, TileGroup group, int key)
        {
            // Check if key already exists
            bool hasKey = group.ContainsKey(key);
            while (hasKey == true)
            {
                int otherTileIndex = group[key];
                //Tile otherTile = allSubImageTiles[otherTileIndex];
                Tile otherTile = allSubImageTiles.ElementAt(otherTileIndex);
                Tuple<float, float> results = otherTile.GetMatches(tile);
                float patternMatch = results.Item1;
                float colourMatch = results.Item2;

                bool areIdentical = Tile.IdenticalTo(patternMatch, colourMatch);
                if (areIdentical == true)
                {
                    Duplicates++;
                    return;
                }

                key++;
                hasKey = group.ContainsKey(key);
            }

            int tileIndex = tile.Index;
            group.AddSimilar(key, tileIndex);
        }

        private class TileGroupMatchResults
        {
            public TileGroup group { get; private set; }
            public float patternMatch { get; private set; }
            public float colourMatch { get; private set; }

            public TileGroupMatchResults()
            {
                group = null;
                patternMatch = 0f;
                colourMatch = 0f;
            }

            public TileGroupMatchResults(TileGroup group, float patternMatch, float colourMatch)
            {
                this.group = group;
                this.patternMatch = patternMatch;
                this.colourMatch = colourMatch;
            }
        }

        private static TileGroupMatchResults FindMatchingGroupWithResults(Tile tileToCompare, float patternThreshold, float colourThreshold)
        {
            foreach (TileGroup group in allSortedTiles)
            {
                //int masterIndex = group.MasterIndex;
                //Tile Master = allSubImageTiles[MasterIndex];
                //Tile master = allSubImageTiles.ElementAt(masterIndex);
                Tile master = group.Master;
                Tuple<float, float> results = tileToCompare.GetMatches(master);
                float patternMatch = results.Item1;
                float colourMatch = results.Item2;

                bool areSimilar = Tile.SimilarTo(patternMatch, patternThreshold);
                if (areSimilar == false)
                {
                    continue;
                }

                return new TileGroupMatchResults(group, patternMatch, colourMatch);
            }

            return new TileGroupMatchResults();
        }

        private static TileGroup FindMatchingGroup(Tile tileToCompare, float patternThreshold, float colourThreshold)
        {
            foreach (TileGroup group in allSortedTiles)
            {
                int masterIndex = group.MasterIndex;
                //Tile Master = allSubImageTiles[MasterIndex];
                Tile master = allSubImageTiles.ElementAt(masterIndex);
                bool areSimilar = master.SimilarTo(tileToCompare, patternThreshold, colourThreshold);
                if (areSimilar == false)
                {
                    continue;
                }

                return group;
            }

            return null;
        }

        public static void LoadAllTiles(Bitmap image, int bitsPerColour, int tileSize)
        {
            LoadAllTiles(image, bitsPerColour, tileSize, 0, 0);
        }

        public static void LoadAllTiles(Bitmap image, int bitsPerColour, int tileSize, int offsetX, int offsetY)
        {
            DisplayReady = false;
            int width = image.Width - offsetX;
            int height = image.Height - offsetY;
            int totalTiles = Program.GetTileCount(width, height, tileSize);

            TasksComplete = 0;
            TotalTasks = totalTiles;

            int subWidth = width;
            int subHeight = height;

            while (totalTiles > MAX_CONCURRENT_TILES)
            {
                subWidth /= 2;
                subHeight /= 2;

                totalTiles = Program.GetTileCount(subWidth, subHeight, tileSize);
            }

            int subImagesWide = width / subWidth;
            int totalSubImages = subImagesWide * (height / subHeight);

            //allSubImageTiles = new List<Tile>();
            //allSubImageTiles = new LinkedList<Tile>();
            allSubImageTiles = new SortedSet<Tile>();
            //tileDepository = new ConcurrentBag<Tile>();   //threaded

            for (int i = 0; i < totalSubImages; i++)
            {
                int x = (i % subImagesWide) * subWidth;
                int y = (i / subImagesWide) * subHeight;
                Rectangle rect = new Rectangle(x, y, subWidth, subHeight);
                //Bitmap subImage = canvas.Clone(rect, PixelFormat.Format24bppRgb);
                Bitmap subImage = new Bitmap(subWidth, subHeight, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(subImage))
                {
                    graphics.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);
                }

                LoadTiles(subImage, bitsPerColour, tileSize, offsetX, offsetY);
            }

            //allSubImageTiles = new SortedSet<Tile>(tileDepository); //threaded
            DisplayReady = true;
        }

        //public static void LoadAllTilesByRef(ref Bitmap canvas, int BitsPerColour, int TileSize)
        //{
        //    LoadAllTilesByRef(ref canvas, BitsPerColour, TileSize, 0, 0);
        //}

        //public static void LoadAllTilesByRef(ref Bitmap canvas, int BitsPerColour, int TileSize, int offsetX, int offsetY)
        //{
        //    DisplayReady = false;
        //    int imageWidth = canvas.Width - offsetX;
        //    int imageHeight = canvas.Height - offsetY;
        //    int totalTiles = Program.GetTileCount(imageWidth, imageHeight, TileSize);

        //    TasksComplete = 0;
        //    TotalTasks = totalTiles;

        //    int subImageWidth = imageWidth;
        //    int subImageHeight = imageHeight;

        //    while (totalTiles > MAX_CONCURRENT_TILES)
        //    {
        //        subImageWidth /= 2;
        //        subImageHeight /= 2;

        //        totalTiles = Program.GetTileCount(subImageWidth, subImageHeight, TileSize);
        //    }

        //    int subImagesWide = imageWidth / subImageWidth;
        //    int TotalSubImages = subImagesWide * (imageHeight / subImageHeight);

        //    LoadTilesByRef(ref canvas, BitsPerColour, TileSize);
        //    //for (int i = 0; i < TotalSubImages; i++)
        //    //{
        //    //    int x = (i % subImagesWide) * subImageWidth;
        //    //    int y = (i / subImagesWide) * subImageHeight;
        //    //    Rectangle rect = new Rectangle(x, y, subImageWidth, subImageHeight);
        //    //    Bitmap croppedImage = loadedImage.Clone(rect, PixelFormat.Format24bppRgb);

        //    //    int subImageOffsetX;
        //    //    int subImageOffsetY;

        //    //    LoadTiles(croppedImage, BitsPerColour, TileSize, subImageOffsetX, subImageOffsetY); // Sub loadedImage messes with corner location
        //    //}

        //    //allSubImageTiles = new SortedSet<Tile>(tileDepository); //threaded
        //    DisplayReady = true;
        //}

        private static void LoadTiles(Bitmap image, int bitsPerColour, int tileSize, int offsetX, int offsetY)
        {
            int boundsX = image.Width - tileSize * (int)Math.Ceiling((double)offsetX / tileSize);
            int boundsY = image.Height - tileSize * (int)Math.Ceiling((double)offsetY / tileSize);

            for (int y = offsetY; y < boundsY; y += tileSize)
            {
                for (int x = offsetX; x < boundsX; x += tileSize)
                {
                    Tile tileToAdd;
                    try
                    {
                        //tileToAdd = GetTile(loadedImage, BitsPerColour, cornerX, cornerY, TileSize);
                        tileToAdd = GetTile(image, bitsPerColour, x, y, tileSize, TasksComplete);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        MessageBox.Show(string.Format("Error loading tiles. {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    allSubImageTiles.Add(tileToAdd);
                    //allSubImageTiles.AddLast(tileToAdd);

                    TasksComplete++;
                }
            }
        }

        //private static void LoadTilesByRef(ref Bitmap canvas, int BitsPerColour, int TileSize)
        //{
        //    int imageWidth = canvas.Width;
        //    int imageHeight = canvas.Height;

        //    for (int y = 0; y < imageHeight; y += TileSize)
        //    {
        //        for (int x = 0; x < imageWidth; x += TileSize)
        //        {
        //            Tile tileToAdd;
        //            try
        //            {
        //                //tileToAdd = GetTile(loadedImage, BitsPerColour, cornerX, cornerY, TileSize);
        //                tileToAdd = GetTile(canvas, BitsPerColour, x, y, TileSize, TasksComplete);
        //            }
        //            catch (OutOfMemoryException ex)
        //            {
        //                MessageBox.Show(string.Format("Error loading tiles. {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }

        //            allSubImageTiles.Add(tileToAdd);
        //            //allSubImageTiles.AddLast(tileToAdd);

        //            TasksComplete++;
        //        }
        //    }
        //}

        public static void LoadTilesThreaded(Bitmap image, int bitsPerColour, int tileSize, int offsetX, int offsetY)
        {
            int rowCount = GetRowCount(image.Height, tileSize, offsetY);
            int threads = 0;
            //ManualResetEvent[] completedRows = new ManualResetEvent[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                if (threads == MAX_THREADS)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                // Assign a row to a thread
                //completedRows[i] = new ManualResetEvent(false);
                //ThreadInfo info = new ThreadInfo(loadedImage, BitsPerColour, TileSize, offsetX, offsetY, i, completedRows[i]);
                //ThreadPool.QueueUserWorkItem(LoadTileRow, info);
                Thread thread = new Thread(() => LoadTileRow(image, bitsPerColour, tileSize, offsetX, offsetY, i));
                thread.Start();
                thread.Join();
                threads++;
            }

            // Wait until all rows have been processed
            //WaitHandle.WaitAll(completedRows);
        }

        private class ThreadInfo
        {
            public Bitmap image { get; set; }
            public int bitsPerColour { get; set; }
            public int tileSize { get; set; }
            public int offsetX { get; set; }
            public int offsetY { get; set; }
            public int rowNumber { get; set; }
            public ManualResetEvent completionStatus { get; set; }

            public ThreadInfo(Bitmap image, int bitsPerColour, int tileSize, int offsetX, int offsetY, int rowNumber, ManualResetEvent completionStatus)
            {
                this.image = image;
                this.bitsPerColour = bitsPerColour;
                this.tileSize = tileSize;
                this.offsetX = offsetX;
                this.offsetY = offsetY;
                this.rowNumber = rowNumber;
                this.completionStatus = completionStatus;
            }
        }

        private static void LoadTileRow(Bitmap image, int bitsPerColour, int tileSize, int offsetX, int offsetY, int rowNumber)
        {
            //private static void LoadTileRow(object parameters)
            //{
            //ThreadInfo info = parameters as ThreadInfo;

            //Bitmap loadedImage = info.loadedImage;
            //int BitsPerColour = info.BitsPerColour;
            //int TileSize = info.TileSize;
            //int offsetX = info.offsetX;
            //int offsetY = info.offsetY;
            //int rowNumber = info.rowNumber;
            //ManualResetEvent resetEvent = info.completionStatus;

            int tilesWide = GetColumnsCount(image.Width, tileSize, offsetX);
            int y = tileSize * rowNumber + offsetY;

            for (int i = 0; i < tilesWide; i++)
            {
                int x = i * tileSize + offsetX;
                int tileNumber = tilesWide * rowNumber + i;

                Tile tileToAdd;
                try
                {
                    //tileToAdd = GetTile(loadedImage, BitsPerColour, cornerX, cornerY, TileSize);
                    tileToAdd = GetTile(image, bitsPerColour, x, y, tileSize, tileNumber);
                }
                catch (OutOfMemoryException ex)
                {
                    MessageBox.Show(string.Format("Error loading tiles. {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tileDepository.Add(tileToAdd);
            }

            //resetEvent.Set();
            TasksComplete += tilesWide;
        }

        public static int GetTileCount(int width, int height, int tileSize)
        {
            int tilesWide = GetColumnsCount(width, tileSize);
            int tilesHigh = GetRowCount(height, tileSize);

            int totalTiles = tilesWide * tilesHigh;
            return totalTiles;
        }

        private static int GetRowCount(int height, int tileSize, int offset)
        {
            int tilesHigh = (int)Math.Floor((double)(height - offset) / (double)tileSize);
            return tilesHigh;
        }

        private static int GetRowCount(int height, int tileSize)
        {
            return GetRowCount(height, tileSize, 0);
        }

        private static int GetColumnsCount(int width, int tileSize, int offset)
        {
            int tilesWide = (int)Math.Floor((double)(width - offset) / (double)tileSize);
            return tilesWide;
        }

        private static int GetColumnsCount(int width, int tileSize)
        {
            return GetColumnsCount(width, tileSize, 0);
        }

        //private static Tile GetTile(Bitmap canvas, int BitsPerColour, int x, int y, int length)
        //{
        //    Bitmap subImage = GetSubImage(canvas, x, y, length);
        //    Tile tileToCompare = new Tile(subImage, BitsPerColour, x, y, length);
        //    return tileToCompare;
        //}

        private static Tile GetTile(Bitmap image, int bitsPerColour, int x, int y, int size, int index)
        {
            Bitmap subImage = GetSubImage(ref image, x, y, size);
            Tile tile = new Tile(subImage, bitsPerColour, size, index);
            return tile;
        }

        //private static Tile GetTile(int bitsPerColour, int tileSize, int tileIndex)
        //{
        //    Bitmap image = Images.GetTileImage(tileIndex);
        //    Tile tile = new Tile(image, bitsPerColour, tileSize, tileIndex);
        //    return tile;
        //}

        public static Bitmap GetTileImage(int tileIndex)
        {
            if (Images == null)
            {
                throw new NullReferenceException("No image loaded");
            }

            Bitmap tileImage = Images.GetTileImage(tileIndex);
            return tileImage;
        }

        private static Tile GetTileFromSubImage(int bitsPerColour, int tileSize, int tileIndex, int subImageIndex, int subImageTileIndex)
        {
            Bitmap tileImage = Images.GetTileImage(subImageIndex, subImageTileIndex);
            Tile tile = new Tile(tileImage, bitsPerColour, tileSize, tileIndex);
            return tile;
        }

        private static Bitmap GetSubImage(ref Bitmap image, int x, int y, int length)
        {
            Rectangle rect = new Rectangle(x, y, length, length);
            //Bitmap subImage = canvas.Clone(rect, canvas.PixelFormat);
            Bitmap subImage = new Bitmap(length, length, PixelFormat.Format24bppRgb);
            using (Graphics graphics = Graphics.FromImage(subImage))
            {
                graphics.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);
            }

            return subImage;
        }

        public static Bitmap GetTileImage(int x, int y, int size)
        {
            Bitmap currentSubImage = Images.CurrentSubImage;
            Bitmap tileImage = GetSubImage(ref currentSubImage, x, y, size);
            return tileImage;
        }
    }
}