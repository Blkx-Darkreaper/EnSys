using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;

namespace MapMaker
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MapMakerForm());
        }

        public static int TileLength { get; set; }
        public static int TilesetDisplayWidth { get; set; }
        private static string tilesetFilename { get; set; }
        private static Bitmap tilesetImage { get; set; }
        private const string IMAGE_FILTER = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif)|*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif";
        private static string currentSaveFilename { get; set; }
        private const string DEFAULT_FILENAME = "Map{0}.sfm";
        private const string MAP_FILTER = "Map Files(*.sfm)|*.sfm";
        private static string author { get; set; }
        private static DateTime dateCreated { get; set; }
        private static List<Tile> tileset;
        private static Size tilesetDisplaySize { get; set; }
        private static Tile selectedTile { get; set; }
        private static bool selectedValue { get; set; }
        private static int selectedNumber { get; set; }
        /* Non memento pattern
        public static List<GridHistory> allMapGridHistories;
        private static Queue<GridHistory> allUpdatedMapGridHistories;
        */
        // Memento pattern
        public static List<Grid> allMapGrids;
        private static Queue<Grid> allUpdatedMapGrids;
        private static Size mapSize { get; set; }
        public static bool HasMapChanged { get; private set; }
        public static bool HasOverlayChanged { get; private set; }
        public static bool HasTerrainChanged { get; private set; }
        private static List<DrawState> allDrawStates { get; set; }
        private static int currentDrawState { get; set; }
        public static int MaxSectors { get; set; }
        private static Grid selectedGrid { get; set; }
        private static List<Color> colourRoulette { get; set; }
        public enum Overlays
        {
            None, Construction, Drivable, Flyable, Sectors
        }

        public static void Init(int tileLength, int tilesetDisplayWidth)
        {
            TileLength = tileLength;
            TilesetDisplayWidth = tilesetDisplayWidth;
            tilesetFilename = string.Empty;
            tilesetImage = null;
            currentSaveFilename = null;
            author = string.Empty;
            dateCreated = DateTime.Now;
            tileset = new List<Tile>();
            selectedTile = null;
            HasMapChanged = false;
            HasOverlayChanged = false;
            HasTerrainChanged = false;
            /* Non memento pattern
            allMapGridHistories = new List<GridHistory>();
            allUpdatedMapGridHistories = new Queue<GridHistory>();
            */
            // Memento pattern
            allMapGrids = new List<Grid>();
            allUpdatedMapGrids = new Queue<Grid>();
            allDrawStates = new List<DrawState>();
            currentDrawState = -1;
            MaxSectors = 1;
            selectedGrid = null;

            InitColourRoulette(5);
        }

        private static void InitColourRoulette(int totalColours)
        {
            InitColourRoulette(totalColours, 0);
        }

        private static void InitColourRoulette(int totalColours, int seed)
        {
            colourRoulette = new List<Color>(totalColours);

            float saturation = 1f; // 0 - 1
            float brightness = 1f; // 0 - 1

            int alpha = 255;

            for (int i = 0; i < totalColours; i++)
            {
                float hue = i * (360 / (float)totalColours);    // 0 - 360
                hue = Program.Clamp(hue, 0f, 360f);

                Color colour = GetColourFromHsl(alpha, hue, saturation, brightness);
                colourRoulette.Add(colour);
            }
        }

        public static float GetBrightness(Color colour)
        {
            int red = colour.R;
            int green = colour.G;
            int blue = colour.B;

            float brightness = (red * 0.299f + green * 0.587f + blue * 0.114f) / 256f;
            return brightness;
        }

        public static Color GetColourFromHsl(int alpha, float hue, float saturation, float brightness)
        {
            if (saturation == 0f)
            {
                int value = Program.Clamp((int)(brightness * 255), 0, 255);
                return Color.FromArgb(alpha, value, value, value);
            }

            float min, max, huePercent;
            huePercent = hue / 360f;

            max = brightness * (1 + saturation);
            if (brightness >= 0.5)
            {
                max = (brightness + saturation) - (brightness * saturation);
            }

            min = brightness * 2f - max;

            float third = 1 / 3f;
            float redF = RgbChannelFromHue(min, max, huePercent + third);
            float greenF = RgbChannelFromHue(min, max, huePercent);
            float blueF = RgbChannelFromHue(min, max, huePercent - third);

            int red = (int)Math.Round(255 * redF, 0);
            int green = (int)Math.Round(255 * greenF, 0);
            int blue = (int)Math.Round(255 * blueF, 0);

            red = Program.Clamp(red, 0, 255);
            green = Program.Clamp(green, 0, 255);
            blue = Program.Clamp(blue, 0, 255);

            Color colour = Color.FromArgb(alpha, red, green, blue);
            return colour;
        }

        private static float RgbChannelFromHue(float min, float max, float value)
        {
            value = (value + 1) % 1;
            if (value < 0)
            {
                value += 1;
            }

            if (value * 6 < 1)
            {
                return min + (max - min) * 6 * value;
            }
            else if (value * 2 < 1)
            {
                return max;
            }
            else if (value * 3 < 2)
            {
                return min + (max - min) * 6 * (2 / 3f - value);
            }
            else
            {
                return min;
            }
        }

        //public static Color GetColourFromHsl(int alpha, float hue, float saturation, float brightness)
        //{
        //    float redF;
        //    float greenF;
        //    float blueF;

        //    if (brightness <= 0)
        //    {
        //        redF = greenF = blueF = 0;
        //    }
        //    else if (saturation <= 0)
        //    {
        //        redF = greenF = blueF = brightness;
        //    }
        //    else
        //    {
        //        float quotient = hue / 60;
        //        int integer = (int)Math.Floor(quotient);
        //        float remainder = quotient - integer;
        //        float pBrightness = brightness * (1 - saturation);
        //        float qBrightness = brightness * (1 - saturation * remainder);
        //        float tBrightness = brightness * (1 - saturation * (1 - remainder));

        //        switch (integer)
        //        {
        //            // Red is dominant
        //            case 0:
        //                redF = brightness;
        //                greenF = tBrightness;
        //                blueF = pBrightness;
        //                break;

        //            // Green is dominant
        //            case 1:
        //                redF = qBrightness;
        //                greenF = brightness;
        //                blueF = pBrightness;
        //                break;

        //            case 2:
        //                redF = pBrightness;
        //                greenF = brightness;
        //                blueF = tBrightness;
        //                break;

        //            // Blue is dominant
        //            case 3:
        //                redF = pBrightness;
        //                greenF = qBrightness;
        //                blueF = brightness;
        //                break;

        //            case 4:
        //                redF = tBrightness;
        //                greenF = pBrightness;
        //                blueF = brightness;
        //                break;

        //            // Red is dominant
        //            case 5:
        //                redF = brightness;
        //                greenF = pBrightness;
        //                blueF = qBrightness;
        //                break;

        //            // Edge cases in case of rounding errors
        //            case 6:
        //                redF = brightness;
        //                greenF = tBrightness;
        //                blueF = pBrightness;
        //                break;

        //            case -1:
        //                redF = brightness;
        //                greenF = pBrightness;
        //                blueF = qBrightness;
        //                break;

        //            default:
        //                //LFATAL("i Value error in Pixel conversion, Value is %d", i);
        //                redF = greenF = blueF = brightness;    // Black/White
        //                break;
        //        }
        //    }

        //    redF *= 255;
        //    greenF *= 255;
        //    blueF *= 255;

        //    int red = Program.Clamp((int)redF, 0, 255);
        //    int green = Program.Clamp((int)greenF, 0, 255);
        //    int blue = Program.Clamp((int)blueF, 0, 255);

        //    Color colour = Color.FromArgb(alpha, red, green, blue);
        //    return colour;
        //}

        //public static void SetTilesetDisplaySize(Size size)
        //{
        //    tilesetDisplaySize = size;
        //}

        public static NewFileForm CreateNewFile()
        {
            NewFileForm newFileForm = new NewFileForm(TileLength);
            newFileForm.ShowDialog();
            if (newFileForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return null;
            }

            return newFileForm;
        }

        public static void LoadTileset(string filename, int tileLength, int tilesetDisplayWidth)
        {
            Program.TileLength = tileLength;

            Program.LoadTilesetImageFromFile(filename);
            Program.BuildTileset(tileLength, tilesetDisplayWidth);

            //Program.SetTilesetDisplaySize(tilesetDisplaySize);
        }

        public static void BuildTileset(int tileLength, int tilesetDisplayWidth)
        {
            if (tilesetImage == null)
            {
                return;
            }
            if (tileLength == 0)
            {
                return;
            }

            tileset = new List<Tile>();

            int imageWidth = tilesetImage.Width;
            int imageHeight = tilesetImage.Height;

            int tilesWide = imageWidth / (tileLength * 2);
            int tilesHigh = imageHeight / (tileLength * 3);
            int totalTiles = tilesWide * tilesHigh;

            for (int i = 0; i < totalTiles; i++)
            {
                int x = (i % tilesWide) * (2 * tileLength);
                int y = (i / tilesWide) * (3 * tileLength);
                Point corner = new Point(x, y);

                Tile tileToAdd = new Tile(i, corner);
                tileset.Add(tileToAdd);
            }

            // Select first tile
            selectedTile = tileset[0];

            // Set display size
            int columns = tilesetDisplayWidth / tileLength;
            int displayWidth = columns * tileLength;

            int rows = (int)Math.Ceiling(totalTiles / (double)columns);
            int displayHeight = rows * tileLength;

            Size tilesetDisplaySize = new Size(displayWidth, displayHeight);
        }

        public static void BuildMap(Size size)
        {
            BuildMap(size.Width, size.Height);
        }

        public static void BuildMap(int width, int height)
        {
            if (tileset.Count == 0)
            {
                throw new InvalidProgramException("Tileset has not been loaded");
            }

            Tile defaultTile = tileset[0];

            Program.mapSize = new Size(width, height);
            //allMapGridHistories = new List<GridHistory>();
            allMapGrids = new List<Grid>();

            // Set the inital nextState
            Program.InitDrawState();

            for (int y = 0; y < height; y += TileLength)
            {
                for (int x = 0; x < width; x += TileLength)
                {
                    Point corner = new Point(x, y);
                    /* Non memento pattern
                    GridHistory gridToAdd = new GridHistory(gridCorner, defaultTile);

                    allMapGridHistories.Add(gridToAdd);
                    allUpdatedMapGridHistories.Enqueue(gridToAdd);
                    */

                    // Memento pattern
                    Grid gridToAdd = new Grid(corner, defaultTile);
                    allMapGrids.Add(gridToAdd);
                }
            }

            Program.HasTerrainChanged = true;
            Program.HasMapChanged = true;
        }

        private static void InitDrawState()
        {
            Program.currentDrawState = -1;
            Program.allDrawStates.Add(new DrawState());
        }

        public static OpenFileDialog GetImageOpenDialog()
        {
            //string title = "Load tileset";

            return GetOpenDialog(IMAGE_FILTER);
        }

        public static OpenFileDialog GetMapOpenDialog()
        {
            return GetOpenDialog(MAP_FILTER);
        }

        public static OpenFileDialog GetOpenDialog(string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            // Set options
            dialog.Filter = filter;
            dialog.Multiselect = false;

            return dialog;
        }

        public static OpenFileDialog GetOpenDialog(string title, string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            // Set options
            dialog.Filter = filter;
            dialog.Title = title;
            dialog.Multiselect = false;

            return dialog;
        }

        public static string GetFilenameToOpen(OpenFileDialog dialog)
        {
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return null;
            }

            string filename = dialog.FileName;

            if (filename == null)
            {
                MessageBox.Show("Invalid filename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            if (filename.Equals(string.Empty))
            {
                MessageBox.Show("Invalid filename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return filename;
        }

        public static void LoadTilesetImageFromFile(string filename)
        {
            tilesetImage = new Bitmap(filename);
            tilesetFilename = filename;
        }

        public static void LoadMapFile()
        {
            // Get the Filename of the selected file
            OpenFileDialog dialog = Program.GetMapOpenDialog();
            string filename = Program.GetFilenameToOpen(dialog);
            if (filename == null)
            {
                return;
            }

            string json = Program.ReadTextFile(filename);

            StrikeforceMap map = Program.DeserializeMap(json);

            Program.author = map.Author;
            Program.dateCreated = map.DateCreated;
            Program.tilesetFilename = map.TilesetFilename;
            Program.TileLength = map.TileLength;
            Program.mapSize = map.MapSize;

            /* Non memento pattern
            List<Grid> allMapGrids = map.AllMapGrids;
            allMapGridHistories = new List<GridHistory>(allMapGrids.Count);
            foreach (Grid grid in allMapGrids)
            {
                GridHistory history = new GridHistory(grid);
                allMapGridHistories.Add(history);
            }

            allUpdatedMapGridHistories = new Queue<GridHistory>(allMapGridHistories);
            */

            // Memento pattern
            allMapGrids = map.AllMapGrids;

            // Set the initial draw state
            Program.InitDrawState();

            Program.LoadTilesetImageFromFile(tilesetFilename);
            Program.BuildTileset(TileLength, TilesetDisplayWidth);

            Program.currentSaveFilename = filename;
        }

        public static string ReadTextFile(string filename)
        {
            string text = string.Empty;
            using (StreamReader inputFile = new StreamReader(filename))
            {
                text += inputFile.ReadToEnd();
            }

            return text;
        }

        public static void UpdateMapFile()
        {
            if (currentSaveFilename == null)
            {
                Program.SaveMap();
                return;
            }

            SaveMapFile(currentSaveFilename);
        }

        public static SaveFileDialog GetMapSaveDialog()
        {
            string defaultFilename = string.Format(DEFAULT_FILENAME, "001");
            if (currentSaveFilename != null)
            {
                defaultFilename = currentSaveFilename;
            }

            //string title = "Save Map file";

            return GetSaveDialog(defaultFilename, MAP_FILTER);
        }

        private static SaveFileDialog GetSaveDialog(string defaultFilename, string filter)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            // Set options
            dialog.Filter = filter;
            dialog.FileName = defaultFilename;
            dialog.OverwritePrompt = true;

            return dialog;
        }

        private static SaveFileDialog GetSaveDialog(string title, string defaultFilename, string filter)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            // Set options
            dialog.Filter = filter;
            dialog.Title = title;
            dialog.FileName = defaultFilename;
            dialog.OverwritePrompt = true;

            return dialog;
        }

        public static string GetFilenameToSave()
        {
            // Get the Filename of the selected file
            SaveFileDialog dialog = Program.GetMapSaveDialog();
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return null;
            }

            string filename = dialog.FileName;

            if (filename == null)
            {
                MessageBox.Show("Invalid filename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            if (filename.Equals(string.Empty))
            {
                MessageBox.Show("Invalid filename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return filename;
        }

        public static void SaveMap()
        {
            string filename = Program.GetFilenameToSave();
            if (filename == null)
            {
                return;
            }

            SaveMapFile(filename);

            currentSaveFilename = filename;
        }

        private static void SaveMapFile(string filename)
        {
            // Non memento pattern
            //StrikeforceMap map = new StrikeforceMap(author, dateCreated, tilesetFilename, TileLength, mapSize, allMapGridHistories);
            // Memento pattern
            StrikeforceMap map = new StrikeforceMap(author, dateCreated, tilesetFilename, TileLength, Grid.NextSector, mapSize, allMapGrids);
            string json = Program.SerializeMap(map);

            WriteTextFile(filename, json);
        }

        private static void WriteTextFile(string filename, string text)
        {
            using (StreamWriter outputFile = new StreamWriter(filename))
            {
                outputFile.WriteLine(text);
            }
        }

        public static string SerializeMap(StrikeforceMap map)
        {
            string json = JsonConvert.SerializeObject(map);
            return json;
        }

        public static StrikeforceMap DeserializeMap(string json)
        {
            StrikeforceMap map = JsonConvert.DeserializeObject<StrikeforceMap>(json);
            return map;
        }

        //public static Size GetTilesetSize()
        //{
        //    Size size = new Size(0, 0);

        //    if (tilesetImage == null)
        //    {
        //        return size;
        //    }

        //    size = tilesetImage.Size;
        //    return size;
        //}

        public static int GetTilesetCount()
        {
            if (tilesetImage == null)
            {
                return 0;
            }
            if (tileset == null)
            {
                return 0;
            }

            return tileset.Count;
        }

        public static void SetOverlayHasChanged()
        {
            Program.HasOverlayChanged = true;
            Program.HasMapChanged = true;
        }

        public static void SetOverlayIsUpToDate()
        {
            Program.HasOverlayChanged = false;
        }

        public static void SetTerrainHasChanged()
        {
            Program.HasTerrainChanged = true;
            Program.HasMapChanged = true;
        }

        public static void SetTerrainIsUpToDate()
        {
            Program.HasTerrainChanged = false;
        }

        public static void SetMapHasChanged()
        {
            SetTerrainHasChanged();
            SetOverlayHasChanged();
            Program.HasMapChanged = true;
        }

        public static void SetMapIsUpToDate()
        {
            SetOverlayIsUpToDate();
            SetTerrainIsUpToDate();
            Program.HasMapChanged = false;
        }

        public static void SelectTile(Point click)
        {
            int x = click.X;
            int y = click.Y;

            Tile tile = GetTilesetTile(x, y);

            selectedTile = tile;
        }

        /* Non memento pattern
        public static void PenTool(Point location)
        {
            int x = location.X;
            int y = location.Y;

            GridHistory grid = GetMapGrid(x, y);

            PenTool(grid);
        }

        public static void PenTool(GridHistory grid)
        {
            grid.Tile = selectedTile;
            grid.hasChanged = true;

            allUpdatedMapGridHistories.Enqueue(grid);
            Program.HasMapChanged = true;

            Program.AddDrawState(grid);
        }
        */

        // Memento pattern
        public static void PenTool(Grid grid, int selectedOverlay)
        {
            // Save the initial state
            Program.AddToInitialDrawState(grid);

            switch (selectedOverlay)
            {
                case (int)Overlays.Sectors:

                    Program.SetOverlayHasChanged();
                    break;

                case (int)Overlays.Construction:
                    grid.SetConstructable(selectedValue);
                    Program.SetOverlayHasChanged();
                    break;

                case (int)Overlays.Drivable:
                    grid.SetDrivable(selectedValue);
                    Program.SetOverlayHasChanged();
                    break;

                case (int)Overlays.Flyable:
                    grid.SetFlyable(selectedValue);
                    Program.SetOverlayHasChanged();
                    break;

                case (int)Overlays.None:
                default:
                    // Update the grid's tile
                    grid.Tile = selectedTile;

                    allUpdatedMapGrids.Enqueue(grid);
                    Program.SetTerrainHasChanged();
                    break;
            }

            // Save the final state
            Program.AddToFinalDrawState(grid);
        }
        // End memento pattern

        public static void LineTool(Point start, Point end)
        {

        }

        public static void BoxTool(Point start, Point end)
        {

        }

        /* Non memento pattern
        public static void FillTool(Point location)
        {
            int x = location.X;
            int y = location.Y;

            GridHistory grid = GetMapGrid(x, y);
            Tile tiletoChange = grid.Tile;

            if (tiletoChange.Equals(selectedTile))
            {
                return;
            }

            FillTileRecursive(grid, tiletoChange, grid);
        }

        public static void FillTileRecursive(GridHistory grid, Tile tileToChange, GridHistory previousGrid)
        {
            Tile tile = grid.Tile;
            if (!tile.Equals(tileToChange))
            {
                return;
            }

            PenTool(grid);

            int[] adjacentGrids = GetAdjacentGridIndexes(grid);
            foreach (int index in adjacentGrids)
            {
                GridHistory adjacentGrid = allMapGridHistories.ElementAt(index);
                if (adjacentGrid.Equals(previousGrid))
                {
                    continue;
                }

                FillTileRecursive(adjacentGrid, tileToChange, grid);
            }
        }
        */

        // Memento pattern
        public static void FillTool(Grid grid, int selectedOverlay)
        {
            bool valueToChange;
            switch (selectedOverlay)
            {
                case (int)Overlays.Construction:
                    valueToChange = grid.AllowsConstruction;
                    FillConstructionRecursive(grid, valueToChange, grid);
                    break;

                case (int)Overlays.Drivable:
                    valueToChange = grid.AllowsDriving;
                    FillDrivableRecursive(grid, valueToChange, grid);
                    break;

                case (int)Overlays.Flyable:
                    valueToChange = grid.AllowsFlying;
                    FillFlyableRecursive(grid, valueToChange, grid);
                    break;

                case (int)Overlays.None:
                default:
                    Tile tiletoChange = grid.Tile;
                    if (tiletoChange.Equals(selectedTile))
                    {
                        return;
                    }

                    FillTileRecursive(grid, tiletoChange, grid);
                    break;
            }
        }

        public static void FillTileRecursive(Grid grid, Tile tileToChange, Grid previousGrid)
        {
            Tile tile = grid.Tile;
            if (!tile.Equals(tileToChange))
            {
                return;
            }

            Program.PenTool(grid, (int)Overlays.None);

            int[] adjacentGrids = GetAdjacentGridIndexes(grid);
            foreach (int index in adjacentGrids)
            {
                Grid adjacentGrid = allMapGrids.ElementAt(index);
                if (adjacentGrid.Equals(previousGrid))
                {
                    continue;
                }

                FillTileRecursive(adjacentGrid, tileToChange, grid);
            }
        }

        public static void FillConstructionRecursive(Grid grid, bool valueToChange, Grid previousGrid)
        {
            bool constructable = grid.AllowsConstruction;
            if (!constructable.Equals(valueToChange))
            {
                return;
            }

            Program.PenTool(grid, (int)Overlays.Construction);

            int[] adjacentGrids = GetAdjacentGridIndexes(grid);
            foreach (int index in adjacentGrids)
            {
                Grid adjacentGrid = allMapGrids.ElementAt(index);
                if (adjacentGrid.Equals(previousGrid))
                {
                    continue;
                }

                FillConstructionRecursive(adjacentGrid, valueToChange, grid);
            }
        }

        public static void FillDrivableRecursive(Grid grid, bool valueToChange, Grid previousGrid)
        {
            bool drivable = grid.AllowsDriving;
            if (!drivable.Equals(valueToChange))
            {
                return;
            }

            Program.PenTool(grid, (int)Overlays.Drivable);

            int[] adjacentGrids = GetAdjacentGridIndexes(grid);
            foreach (int index in adjacentGrids)
            {
                Grid adjacentGrid = allMapGrids.ElementAt(index);
                if (adjacentGrid.Equals(previousGrid))
                {
                    continue;
                }

                FillDrivableRecursive(adjacentGrid, valueToChange, grid);
            }
        }

        public static void FillFlyableRecursive(Grid grid, bool valueToChange, Grid previousGrid)
        {
            bool flyable = grid.AllowsFlying;
            if (!flyable.Equals(valueToChange))
            {
                return;
            }

            Program.PenTool(grid, (int)Overlays.Flyable);

            int[] adjacentGrids = GetAdjacentGridIndexes(grid);
            foreach (int index in adjacentGrids)
            {
                Grid adjacentGrid = allMapGrids.ElementAt(index);
                if (adjacentGrid.Equals(previousGrid))
                {
                    continue;
                }

                FillFlyableRecursive(adjacentGrid, valueToChange, grid);
            }
        }
        // End memento pattern

        public static int[] GetAdjacentGridIndexes(Grid center)
        {
            int tilesWide = mapSize.Width / TileLength;

            /* Non memento pattern
            int totalTiles = allMapGridHistories.Count;

            List<Grid> allGrids = allMapGridHistories.Cast<Grid>().ToList();
            
            int centerIndex = allGrids.IndexOf(center);
            */

            // Memento pattern
            int totalTiles = allMapGrids.Count;

            int centerIndex = allMapGrids.IndexOf(center);

            List<int> adjacentGrids = new List<int>(4);

            // tile above
            if (centerIndex > tilesWide)
            {
                int aboveIndex = centerIndex - tilesWide;
                adjacentGrids.Add(aboveIndex);
            }

            // tile to the right
            if ((centerIndex % tilesWide) < (tilesWide - 1))
            {
                int rightIndex = centerIndex + 1;
                adjacentGrids.Add(rightIndex);
            }

            // tile below
            if (centerIndex < (totalTiles - tilesWide - 1))
            {
                int belowIndex = centerIndex + tilesWide;
                adjacentGrids.Add(belowIndex);
            }

            // tile to the left
            if (centerIndex % tilesWide != 0)
            {
                int leftIndex = centerIndex - 1;
                adjacentGrids.Add(leftIndex);
            }

            return adjacentGrids.ToArray();
        }

        public static Bitmap DrawTileset(bool addPadding)
        {
            if (tilesetImage == null)
            {
                return null;
            }

            int totalTiles = Program.GetTilesetCount();

            int tilesWide = TilesetDisplayWidth / TileLength;
            int width = tilesWide * TileLength;

            int tilesHigh = (int)Math.Ceiling(totalTiles / (double)tilesWide);
            int height = tilesHigh * TileLength;

            if (addPadding == true)
            {
                // increase TileSize to account for spaces between tiles
                width += tilesWide - 1;
                height += tilesHigh - 1;
            }

            tilesetDisplaySize = new Size(width, height);

            Bitmap displayImage = new Bitmap(width, height);
            Program.DrawAllTilesOntoImage(ref displayImage, tileset, tilesWide, TileLength, addPadding);

            Program.DrawSelectedTile(tilesWide, displayImage);

            return displayImage;
        }

        private static void DrawSelectedTile(int tilesWide, Bitmap displayImage)
        {
            if (selectedTile == null)
            {
                return;
            }

            int index = selectedTile.TilesetIndex;
            int x = (index % tilesWide) * TileLength;
            int y = (index / tilesWide) * TileLength;

            Rectangle bounds = new Rectangle(x, y, TileLength, TileLength);
            Pen pen = new Pen(new SolidBrush(Color.Blue));

            using (Graphics g = Graphics.FromImage(displayImage))
            {
                g.DrawRectangle(pen, bounds);
            }
        }

        public static void InvalidateMap()
        {
            // Non memento pattern
            //allUpdatedMapGridHistories = new Queue<GridHistory>(allMapGridHistories);
            // Memento pattern
            allUpdatedMapGrids = new Queue<Grid>(allMapGrids);
            Program.SetTerrainHasChanged();
        }

        public static Bitmap GetMap(double scale)
        {
            int width = mapSize.Width;
            int height = mapSize.Height;

            int tilesWide = width / TileLength;
            int tilesHigh = height / TileLength;

            width = (int)(width * scale);
            height = (int)(height * scale);

            Bitmap displayImage = new Bitmap(width, height);

            // Non memento pattern
            //Program.DrawAllGridsOntoImage(ref displayImage, allMapGridHistories, tilesWide, TileLength, scale);
            // Memento pattern
            Program.DrawAllGridsOntoImage(ref displayImage, allMapGrids, tilesWide, TileLength, scale);

            Program.SetTerrainIsUpToDate();

            return displayImage;
        }

        public static Bitmap DrawMap(ref Bitmap displayImage, double scale)
        {
            int width = mapSize.Width;
            int scaledWidth = (int)(width * scale);
            if (scaledWidth != displayImage.Width)
            {
                int scaledHeight = (int)(mapSize.Height * scale);
                displayImage = new Bitmap(scaledWidth, scaledHeight);
            }

            int tilesWide = width / TileLength;

            Program.DrawUpdatedGridsOntoImage(ref displayImage, tilesWide, TileLength, scale);

            Program.SetTerrainIsUpToDate();

            return displayImage;
        }

        private static void DrawLabel(Graphics graphics, double scale, Grid grid, string label)
        {
            Point gridCorner = grid.Corner;
            int cornerX = (int)(gridCorner.X * scale);
            int cornerY = (int)(gridCorner.Y * scale);
            int length = (int)(TileLength * scale);

            float emSize = (float)(12f * scale);
            Font font = new Font(FontFamily.GenericSansSerif, emSize, FontStyle.Bold);
            SizeF fontSize = graphics.MeasureString(label, font);

            // Center the label
            int fontX = cornerX + (length - (int)(fontSize.Width * scale)) / 2;
            int fontY = cornerY + (length - (int)(fontSize.Height * scale)) / 2;
            Point fontCorner = new Point(fontX, fontY);

            Brush brush = new SolidBrush(Color.White);

            Rectangle bounds = new Rectangle(fontCorner, new Size(length, length));

            graphics.DrawString(label, font, brush, bounds);
        }

        private static void DrawPassability(Graphics graphics, double scale, Grid grid, bool passable)
        {
            //    string label = "X";
            //    if (passable == true)
            //    {
            //        label = "O";
            //    }

            //    Program.DrawLabel(graphics, scale, grid, label);

            Color colour = Color.FromArgb(0, 255, 0);
            if (passable == false)
            {
                colour = Color.Red;
            }

            Program.DrawCircle(graphics, scale, grid, colour);
        }

        private static void DrawCircle(Graphics graphics, double scale, Grid grid, Color colour)
        {
            Point gridCorner = grid.Corner;
            int length = (int)(TileLength * scale);
            int radius = (int)(TileLength * 0.2);
            int cornerX = (int)(gridCorner.X * scale);
            int cornerY = (int)(gridCorner.Y * scale);

            cornerX += length / 2 - radius;
            cornerY += length / 2 - radius;

            int diameter = radius * 2;
            Rectangle bounds = new Rectangle(cornerX, cornerY, diameter, diameter);

            Brush brush = new SolidBrush(colour);

            graphics.FillEllipse(brush, bounds);
        }

        public static void DrawSectors(ref Bitmap displayImage, double scale)
        {
            using (Graphics graphics = Graphics.FromImage(displayImage))
            {
                Dictionary<int, GraphicsPath> sectors = new Dictionary<int, GraphicsPath>();

                foreach (Grid grid in allMapGrids)
                {
                    int sectorId = grid.Sector;
                    Program.DrawLabel(graphics, scale, grid, sectorId.ToString());

                    GraphicsPath shape;
                    bool sectorStarted = sectors.ContainsKey(sectorId);
                    if (sectorStarted == false)
                    {
                        shape = new GraphicsPath();
                        sectors.Add(sectorId, shape);
                    }

                    shape = sectors[sectorId];
                    int length = (int)(TileLength * scale);
                    Rectangle bounds = new Rectangle(grid.Corner, new Size(length, length));

                    shape.AddRectangle(bounds);
                }

                for (int i = 0; i < sectors.Values.Count; i++)
                {
                    int colourIndex = i % colourRoulette.Count;
                    Color colour = colourRoulette[colourIndex];
                    Pen pen = new Pen(new SolidBrush(colour));

                    GraphicsPath shape = sectors.Values.ElementAt(i);

                    graphics.DrawPath(pen, shape);
                }
            }
        }

        public static void DrawConstructibility(ref Bitmap displayImage, double scale)
        {
            using (Graphics graphics = Graphics.FromImage(displayImage))
            {
                foreach (Grid grid in allMapGrids)
                {
                    bool allowsConstruction = grid.AllowsConstruction;
                    DrawPassability(graphics, scale, grid, allowsConstruction);
                }
            }
        }

        public static void DrawDrivability(ref Bitmap displayImage, double scale)
        {
            using (Graphics graphics = Graphics.FromImage(displayImage))
            {
                foreach (Grid grid in allMapGrids)
                {
                    bool groundPassable = grid.AllowsDriving;
                    DrawPassability(graphics, scale, grid, groundPassable);
                }
            }
        }

        public static void DrawFlyability(ref Bitmap displayImage, double scale)
        {
            using (Graphics graphics = Graphics.FromImage(displayImage))
            {
                foreach (Grid grid in allMapGrids)
                {
                    bool airPassable = grid.AllowsFlying;
                    DrawPassability(graphics, scale, grid, airPassable);
                }
            }
        }

        public static void DrawAllTilesOntoImage(ref Bitmap image, List<Tile> collection, int tilesWide, int tileWidth)
        {
            Program.DrawAllTilesOntoImage(ref image, collection, tilesWide, tileWidth, 1, false);
        }

        public static void DrawAllTilesOntoImage(ref Bitmap image, List<Tile> collection, int tilesWide, int tileWidth, double scale)
        {
            Program.DrawAllTilesOntoImage(ref image, collection, tilesWide, tileWidth, scale, false);
        }

        public static void DrawAllTilesOntoImage(ref Bitmap image, List<Tile> collection, int tilesWide, int tileWidth, bool addPadding)
        {
            Program.DrawAllTilesOntoImage(ref image, collection, tilesWide, tileWidth, 1, addPadding);
        }

        public static void DrawAllTilesOntoImage(ref Bitmap image, List<Tile> collection, int tilesWide, int tileLength, double scale, bool addPadding)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                Tile tile = collection[i];
                int x, y;

                if (addPadding == true)
                {
                    x = i % tilesWide * (int)(tileLength * scale + 1);
                    y = i / tilesWide * (int)(tileLength * scale + 1);
                }
                else
                {
                    x = i % tilesWide * (int)(tileLength * scale);
                    y = i / tilesWide * (int)(tileLength * scale);
                }

                Program.DrawTileOntoImage(ref image, tile, tileLength, x, y, scale);
            }
        }

        /* Non memento pattern
        public static void DrawAllGridsOntoImage(ref Bitmap image, List<GridHistory> collection, int tilesWide, int tileWidth)
        {
            Program.DrawAllGridsOntoImage(ref image, collection, tilesWide, tileWidth, 1, false);
        }

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<GridHistory> collection, int tilesWide, int tileWidth, double scale)
        {
            Program.DrawAllGridsOntoImage(ref image, collection, tilesWide, tileWidth, scale, false);
        }

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<GridHistory> collection, int tilesWide, int tileWidth, bool addPadding)
        {
            Program.DrawAllGridsOntoImage(ref image, collection, tilesWide, tileWidth, 1, addPadding);
        }

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<GridHistory> collection, int tilesWide, int tileLength, double scale, bool addPadding)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                GridHistory grid = collection[i];
                bool needsUpdating = grid.hasChanged;
                if (needsUpdating == false)
                {
                    continue;
                }

                int x, y;

                if (addPadding == true)
                {
                    x = i % tilesWide * (int)(tileLength * scale + 1);
                    y = i / tilesWide * (int)(tileLength * scale + 1);
                }
                else
                {
                    x = i % tilesWide * (int)(tileLength * scale);
                    y = i / tilesWide * (int)(tileLength * scale);
                }

                Tile tile = grid.Tile;

                Program.DrawTileOntoImage(ref image, tile, tileLength, x, y, scale);
                grid.hasChanged = false;
            }
        }

        public static void DrawUpdatedGridsOntoImage(ref Bitmap image, int tilesWide, int tileLength, double scale)
        {
            while (allUpdatedMapGridHistories.Count > 0)
            {
                GridHistory grid = allUpdatedMapGridHistories.Dequeue();
                Point gridCorner = grid.Corner;
                int x = (int)(gridCorner.X * scale);
                int y = (int)(gridCorner.Y * scale);

                Tile tile = grid.Tile;

                Program.DrawTileOntoImage(ref image, tile, tileLength, x, y, scale);
            }
        }
        */

        // Memento pattern
        public static void DrawAllGridsOntoImage(ref Bitmap image, List<Grid> collection, int tilesWide, int tileWidth)
        {
            Program.DrawAllGridsOntoImage(ref image, collection, tilesWide, tileWidth, 1, false);
        }

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<Grid> collection, int tilesWide, int tileWidth, double scale)
        {
            Program.DrawAllGridsOntoImage(ref image, collection, tilesWide, tileWidth, scale, false);
        }

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<Grid> collection, int tilesWide, int tileWidth, bool addPadding)
        {
            Program.DrawAllGridsOntoImage(ref image, collection, tilesWide, tileWidth, 1, addPadding);
        }

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<Grid> collection, int tilesWide, int tileLength, double scale, bool addPadding)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                Grid grid = collection[i];

                int x, y;

                if (addPadding == true)
                {
                    x = i % tilesWide * (int)(tileLength * scale + 1);
                    y = i / tilesWide * (int)(tileLength * scale + 1);
                }
                else
                {
                    x = i % tilesWide * (int)(tileLength * scale);
                    y = i / tilesWide * (int)(tileLength * scale);
                }

                Tile tile = grid.Tile;

                Program.DrawTileOntoImage(ref image, tile, tileLength, x, y, scale);
            }
        }

        public static void DrawUpdatedGridsOntoImage(ref Bitmap image, int tilesWide, int tileLength, double scale)
        {
            while (allUpdatedMapGrids.Count > 0)
            {
                Grid grid = allUpdatedMapGrids.Dequeue();
                Point corner = grid.Corner;
                int x = (int)(corner.X * scale);
                int y = (int)(corner.Y * scale);

                Tile tile = grid.Tile;

                Program.DrawTileOntoImage(ref image, tile, tileLength, x, y, scale);
            }
        }
        // End memento pattern

        private static void DrawTileOntoImage(ref Bitmap image, Tile tile, int tileLength, int x, int y)
        {
            DrawTileOntoImage(ref image, tile, tileLength, x, y, 1);
        }

        private static void DrawTileOntoImage(ref Bitmap image, Tile tile, int tileLength, int x, int y, double scale)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                Image tileImage = Program.GetTileImage(tile);

                //int width = (int)(tileImage.Width * scale);
                //int height = (int)(tileImage.Height * scale);
                int width = (int)(tileLength * scale);
                int height = (int)(tileLength * scale);

                g.DrawImage(tileImage, x, y, width, height);
            }
        }

        private static int GetTileCount(int width, int height, int tileWidth)
        {
            int tilesWide = width / tileWidth;
            int tilesHigh = height / tileWidth;

            int totalTiles = tilesWide * tilesHigh;
            return totalTiles;
        }

        public static Tile GetTilesetTile(int x, int y)
        {
            return GetTile(x, y, tilesetDisplaySize, tileset);
        }

        /* Non memento pattern
        public static GridHistory GetMapGrid(int x, int y)
        {
            return GetGrid(x, y, mapSize, allMapGridHistories);
        }
         
        public static GridHistory GetGrid(int x, int y, Size size, List<GridHistory> collection)
        {
            int width = size.Width;
            int height = size.Height;

            int tilesWide = width / TileLength;

            int column = x / TileLength;
            int row = y / TileLength;

            int index = row * tilesWide + column;
            GridHistory grid = collection[index];

            return grid;
        }
        */

        // Memento pattern
        public static Grid GetMapGrid(int x, int y, double scale)
        {
            return Program.GetGrid(x, y, scale, mapSize, allMapGrids);
        }

        public static Grid GetGrid(int x, int y, double scale, Size size, List<Grid> collection)
        {
            int width = size.Width;
            int height = size.Height;

            int tilesWide = width / TileLength;

            int column = x / (int)(TileLength * scale);
            int row = y / (int)(TileLength * scale);

            int index = row * tilesWide + column;
            Grid grid = collection[index];

            return grid;
        }
        // End memento pattern

        public static Tile GetTile(int x, int y, Size size, List<Tile> collection)
        {
            int width = size.Width;
            int height = size.Height;

            int tilesWide = width / TileLength;

            int column = x / TileLength;
            int row = y / TileLength;

            int index = row * tilesWide + column;
            Tile tile = collection[index];

            return tile;
        }

        public static Bitmap GetTileImage(Tile tile)
        {
            if (tilesetImage == null)
            {
                throw new NullReferenceException("No image loaded");
            }

            Point corner = tile.Corner;
            int x = corner.X;
            int y = corner.Y;

            Rectangle bounds = new Rectangle(x, y, TileLength, TileLength);
            Bitmap tileImage = tilesetImage.Clone(bounds, PixelFormat.Format24bppRgb);
            return tileImage;
        }

        /* Non memento pattern
        public static void AddDrawState(GridHistory grid)
        {
            if (allDrawStates == null)
            {
                allDrawStates = new List<DrawEvent>();
            }

            DrawEvent nextState = allDrawStates.ElementAtOrDefault(currentDrawState);
            if (nextState == null)
            {
                nextState = new DrawEvent(grid);
                allDrawStates.Add(nextState);
            }
            else
            {
                AddToInitialDrawState(grid);
            }
        }
        */

        public static void AddDrawState()
        {
            if (allDrawStates == null)
            {
                allDrawStates = new List<DrawState>();
            }

            currentDrawState++;
            DrawState nextState = allDrawStates.ElementAtOrDefault(currentDrawState);
            if (nextState == null)
            {
                nextState = new DrawState();
                allDrawStates.Add(nextState);
            }
            else
            {
                allDrawStates[currentDrawState] = new DrawState();
            }

            if (allDrawStates.Count <= (currentDrawState + 1))
            {
                return;
            }

            int nextDrawState = currentDrawState + 1;
            int totalToRemove = allDrawStates.Count - nextDrawState;
            allDrawStates.RemoveRange(nextDrawState, totalToRemove);
        }

        /* Non memento pattern
        public static void AddToInitialDrawState(GridHistory grid)
        {
            DrawEvent nextState = allDrawStates[currentDrawState];
            nextState.AddMemento(grid);
        }
        */

        public static void AddToInitialDrawState(Grid grid)
        {
            DrawState state = allDrawStates[currentDrawState];
            state.AddMemento(grid);
        }

        public static void AddToFinalDrawState(Grid grid)
        {
            DrawState state = allDrawStates[currentDrawState];
            state.AddOmen(grid);
        }

        //public static void EndDrawState()
        //{
        //    currentDrawState++;
        //}

        public static bool CheckCanUndo()
        {
            if (currentDrawState == -1)
            {
                return false;
            }
            //int previousDrawState = currentDrawState - 1;
            //if (previousDrawState < 0)
            //{
            //    return false;
            //}

            //DrawState currentState = allDrawStates.ElementAtOrDefault(previousDrawState);
            //if (currentState == null)
            //{
            //    return false;
            //}

            return true;
        }

        /* Non memento pattern
        public static void Undo()
        {
            bool canUndo = CheckCanUndo();
            if (canUndo == false)
            {
                return; // No previous nextState
            }

            int previousDrawState = currentDrawState - 1;
            DrawEvent nextState = allDrawStates[previousDrawState];
            //nextState.Undo();
            LinkedList<GridHistory> omens = nextState.allUpdatedGrids;
            foreach (GridHistory grid in omens)
            {
                grid.PreviousState();
                allUpdatedMapGridHistories.Enqueue(grid);
            }

            Program.HasMapChanged = true;

            currentDrawState--;
            if (currentDrawState < 0)
            {
                currentDrawState = 0;
            }
        }
        */

        // Memento pattern
        public static void Undo()
        {
            bool canUndo = CheckCanUndo();
            if (canUndo == false)
            {
                return; // No previous nextState
            }

            DrawState currentState = allDrawStates[currentDrawState];
            LinkedList<Grid> mementos = currentState.allMementos;
            foreach (Grid previousState in mementos)
            {
                int index = previousState.Id - 1;   // 1 based
                Grid grid = allMapGrids[index];

                grid.MatchCopy(previousState);

                allUpdatedMapGrids.Enqueue(grid);
            }

            Program.SetMapHasChanged();

            currentDrawState--;
            if (currentDrawState < -1)
            {
                currentDrawState = -1;
            }
        }

        public static bool CheckCanRedo()
        {
            int nextDrawState = currentDrawState + 1;
            DrawState drawEvent = allDrawStates.ElementAtOrDefault(nextDrawState);
            if (drawEvent == null)
            {
                return false;
            }

            if (drawEvent.allOmens.Count == 0)
            {
                return false;
            }

            return true;
        }

        /* Non memento pattern
        public static void Redo()
        {
            bool canRedo = CheckCanRedo();
            if (canRedo == false)
            {
                return; // no next nextState
            }

            DrawEvent nextState = allDrawStates[currentDrawState];
            //nextState.Redo();
            LinkedList<GridHistory> omens = nextState.allUpdatedGrids;
            foreach (GridHistory grid in omens)
            {
                grid.NextState();
                allUpdatedMapGridHistories.Enqueue(grid);
            }

            Program.HasMapChanged = true;

            currentDrawState++;
        }
        */

        // Memento pattern
        public static void Redo()
        {
            bool canRedo = CheckCanRedo();
            if (canRedo == false)
            {
                return; // no next nextState
            }

            currentDrawState++;
            int lastDrawState = allDrawStates.Count - 1;
            if (currentDrawState > lastDrawState)
            {
                currentDrawState = lastDrawState;
            }

            DrawState currentState = allDrawStates[currentDrawState];
            LinkedList<Grid> omens = currentState.allOmens;
            foreach (Grid nextState in omens)
            {
                int index = nextState.Id - 1;   // 1 based
                Grid grid = allMapGrids[index];

                grid.MatchCopy(nextState);

                allUpdatedMapGrids.Enqueue(grid);
            }

            Program.SetMapHasChanged();
        }

        public static double GetDistance(Point start, Point end)
        {
            float startX = start.X;
            float startY = start.Y;

            float endX = end.X;
            float endY = end.Y;

            float deltaX = endX - startX;
            float deltaY = endY - startY;

            double distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            return distance;
        }

        public static float Clamp(float value, float min, float max)
        {
            value = ClampToMin(value, min);
            value = ClampToMax(value, max);
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            value = ClampToMin(value, min);
            value = ClampToMax(value, max);
            return value;
        }

        public static float ClampToMax(float value, float max)
        {
            if (value > max)
            {
                value = max;
            }

            return value;
        }

        public static int ClampToMax(int value, int max)
        {
            if (value > max)
            {
                value = max;
            }

            return value;
        }

        public static float ClampToMin(float value, float min)
        {
            if (value < min)
            {
                value = min;
            }

            return value;
        }

        public static int ClampToMin(int value, int min)
        {
            if (value < min)
            {
                value = min;
            }

            return value;
        }

        public static int GetSign(int value)
        {
            if (value == 0)
            {
                return 1;
            }

            int sign = value / Math.Abs(value);
            return sign;
        }

        public static void SelectGrid(Grid grid)
        {
            if (selectedGrid.Equals(grid))
            {
                selectedGrid.CycleSector();
                return;
            }

            selectedGrid = grid;
        }

        public static void SetSelectedValue(bool value)
        {
            Program.selectedValue = value;
        }

        public static void SetSelectedNumber(int number)
        {
            Program.selectedNumber = number;
        }
    }
}