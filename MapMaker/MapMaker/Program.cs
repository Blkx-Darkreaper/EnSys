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
        public static double MapScale { get; set; }
        public static int TilesetDisplayWidth { get; set; }
        private static string tilesetFilename { get; set; }
        private static Bitmap tilesetImage { get; set; }
        private const string IMAGE_FILTER = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif)|*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif";
        private static string currentSaveFilename { get; set; }
        private const string DEFAULT_FILENAME = "Map{0}.json";
        private const string MAP_FILTER = "Map Files(*.json)|*.json";
        public static string Author { get; set; }
        private static DateTime dateCreated { get; set; }
        private static List<Tile> tileset;
        private static Size tilesetDisplaySize { get; set; }
        public static Color SelectionColour { get; set; }
        public static Tile SelectedTile { get; private set; }
        private static bool selectedValue { get; set; }
        private static int selectedNumber { get; set; }
        public static List<Grid> AllMapGrids;
        private static Queue<Grid> allUpdatedMapGrids;
        private static Size mapSize { get; set; }
        public static bool HasDisplayChanged { get; private set; }
        public static bool HasOverlayChanged { get; private set; }
        public static bool HasTerrainChanged { get; private set; }
        public static bool HasUnsavedChanges { get; private set; }
        public static Dictionary<int, Checkpoint> AllCheckpoints { get; set; }
        public static Dictionary<int, Sector> AllSectors { get; set; }
        public static int MaxSectors { get { return AllSectors.Count + 1; } }
        private static List<DrawState> allDrawStates { get; set; }
        private static int currentDrawState { get; set; }
        public static List<Color> ColourRoulette { get; private set; }
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
            Author = string.Empty;
            dateCreated = DateTime.Now;
            tileset = new List<Tile>();
            SelectionColour = Color.Blue;
            SelectedTile = null;
            AllMapGrids = new List<Grid>();
            allUpdatedMapGrids = new Queue<Grid>();
            HasDisplayChanged = false;
            HasOverlayChanged = false;
            HasTerrainChanged = false;
            HasUnsavedChanges = false;
            AllCheckpoints = new Dictionary<int, Checkpoint>();
            AllSectors = new Dictionary<int, Sector>();
            allDrawStates = new List<DrawState>();
            currentDrawState = -1;

            InitColourRoulette(5);
        }

        public static void InitColourRoulette(int totalColours)
        {
            InitColourRoulette(totalColours, 0);
        }

        public static void InitColourRoulette(int totalColours, int seed)
        {
            ColourRoulette = new List<Color>(totalColours);

            float saturation = 1f; // 0 - 1
            float brightness = 0.5f; // 0 - 1

            int alpha = 255;

            for (int i = 0; i < totalColours; i++)
            {
                float hue = (i * (360 / (float)totalColours) + seed) % 360;    // 0 - 360
                hue = Program.Clamp(hue, 0f, 360f);

                Color colour = GetColourFromHsl(alpha, hue, saturation, brightness);
                ColourRoulette.Add(colour);
            }
        }

        public static void InitSectors()
        {
            foreach (Grid grid in AllMapGrids)
            {
                int sectorId = grid.SectorId;
                if (sectorId == 0)
                {
                    continue;
                }

                Sector sector;
                bool sectorExists = AllSectors.ContainsKey(sectorId);
                if (sectorExists == false)
                {
                    AllSectors.Add(sectorId, new Sector(sectorId, TileLength));
                }

                sector = AllSectors[sectorId];
                // Resize sector to accomodate grid
                sector.AddGrid(grid, TileLength);
            }

            if (ColourRoulette.Count >= MaxSectors)
            {
                return;
            }

            InitColourRoulette(MaxSectors);
        }

        public static void LoadCheckpoints(List<Checkpoint> checkpoints)
        {
            if (checkpoints == null)
            {
                return;
            }

            foreach (Checkpoint toAdd in checkpoints)
            {
                int y = toAdd.Location.Y;
                AllCheckpoints.Add(y, toAdd);
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

        public static NewFileForm CreateNewFile()
        {
            NewFileForm newFileForm = new NewFileForm();
            newFileForm.ShowDialog();
            if (newFileForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return null;
            }

            return newFileForm;
        }

        public static DisplayForm GetDisplayOptions(Size windowSize)
        {
            DisplayForm displayForm = new DisplayForm(windowSize);
            displayForm.ShowDialog();
            if (displayForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return null;
            }

            return displayForm;
        }

        public static MapPropertiesForm GetMapProperties()
        {
            MapPropertiesForm mapPropertiesForm = new MapPropertiesForm(mapSize, tilesetFilename, TileLength, Author, dateCreated.ToString());
            mapPropertiesForm.ShowDialog();
            if (mapPropertiesForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return null;
            }

            return mapPropertiesForm;
        }

        public static void LoadTileset(string filename, int tileLength, int tilesetDisplayWidth)
        {
            Program.TileLength = tileLength;

            Program.LoadTilesetImageFromFile(filename);
            Program.BuildTileset(tileLength, tilesetDisplayWidth);
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
            SelectedTile = tileset[0];

            // Set display nodeSize
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
            AllMapGrids = new List<Grid>();

            // Set the inital nextState
            Program.InitDrawState();

            for (int y = 0; y < height; y += TileLength)
            {
                for (int x = 0; x < width; x += TileLength)
                {
                    Point corner = new Point(x, y);

                    Grid gridToAdd = new Grid(corner, defaultTile);
                    AllMapGrids.Add(gridToAdd);
                }
            }

            AllMapGrids.Sort();

            Program.MapHasChanged();
        }

        public static void ResizeMap(int width, int height)
        {
            Tile defaultTile = tileset[0];

            int oldWidth = Program.mapSize.Width;
            int oldHeight = Program.mapSize.Height;

            int deltaWidth = width - oldWidth;
            int deltaHeight = height - oldHeight;

            Program.mapSize = new Size(width, height);

            // Set the inital nextState
            Program.InitDrawState();

            int maxWidth = Math.Max(width, oldWidth);
            int maxHeight = Math.Max(height, oldHeight);

            for (int y = 0; y < maxHeight; y += TileLength)
            {
                for (int x = 0; x < maxWidth; x += TileLength)
                {
                    Point corner = new Point(x, y);
                    Grid dummy = new Grid(corner, defaultTile);

                    bool gridAlreadyAdded = AllMapGrids.Contains(dummy);

                    Grid grid;
                    if (gridAlreadyAdded == true)
                    {
                        grid = AllMapGrids.Find(g => g.Equals(dummy));
                    }
                    else
                    {
                        grid = dummy;
                    }

                    if (x >= width)
                    {
                        AllMapGrids.Remove(grid);
                        continue;
                    }

                    if (y >= height)
                    {
                        AllMapGrids.Remove(grid);
                        continue;
                    }

                    if (gridAlreadyAdded == true)
                    {
                        continue;
                    }

                    AllMapGrids.Add(grid);
                }
            }

            AllMapGrids.Sort();

            Program.MapHasChanged();
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
            string startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string relativePath = filename.Replace(startupPath, "");
            string fullPath = startupPath + relativePath;

            Program.tilesetImage = new Bitmap(fullPath);
            Program.tilesetFilename = relativePath;
        }

        public static void LoadMapFile(string filename)
        {
            string json = Program.ReadTextFile(filename);

            StrikeforceMap map = Program.DeserializeMap(json);

            Program.Author = map.Author;
            Program.dateCreated = map.DateCreated;
            Program.tilesetFilename = map.TilesetFilename;
            Program.TileLength = map.TileLength;
            Program.mapSize = map.MapSize;

            AllMapGrids = map.AllMapGrids;
            AllMapGrids.Sort();

            // Set the initial draw margin
            Program.InitDrawState();

            // Load Checkpoints
            Program.LoadCheckpoints(map.AllCheckpoints);

            // Create Sectors
            Program.InitSectors();

            Program.LoadTilesetImageFromFile(tilesetFilename);
            Program.BuildTileset(TileLength, TilesetDisplayWidth);

            Program.currentSaveFilename = filename;
            Program.HasUnsavedChanges = false;
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

            Program.SaveMapFile(currentSaveFilename);
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

            Program.SaveMapFile(filename);

            currentSaveFilename = filename;

            HasUnsavedChanges = false;
        }

        private static void SaveMapFile(string filename)
        {
            StrikeforceMap map = new StrikeforceMap(Author, dateCreated, tilesetFilename, TileLength, Grid.NextSector, mapSize, AllMapGrids, AllCheckpoints.Values.ToList());
            string json = Program.SerializeMap(map);

            WriteTextFile(filename, json);

            HasUnsavedChanges = false;
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
        //    Size nodeSize = new Size(0, 0);

        //    if (tilesetImage == null)
        //    {
        //        return nodeSize;
        //    }

        //    nodeSize = tilesetImage.Size;
        //    return nodeSize;
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

        public static void OverlayHasChanged()
        {
            Program.HasOverlayChanged = true;
            Program.HasDisplayChanged = true;
            //Program.HasUnsavedChanges = true;
        }

        public static void OverlayIsUpToDate()
        {
            Program.HasOverlayChanged = false;
        }

        public static void TerrainHasChanged()
        {
            Program.HasTerrainChanged = true;
            Program.HasDisplayChanged = true;
            Program.HasUnsavedChanges = true;
        }

        public static void TerrainIsUpToDate()
        {
            Program.HasTerrainChanged = false;
        }

        public static void MapHasChanged()
        {
            TerrainHasChanged();
            OverlayHasChanged();
            DisplayHasChanged();
            Program.HasUnsavedChanges = true;
        }

        public static void DisplayIsUpToDate()
        {
            OverlayIsUpToDate();
            TerrainIsUpToDate();
            Program.HasDisplayChanged = false;
        }

        public static void DisplayHasChanged()
        {
            Program.HasDisplayChanged = true;
        }

        public static void SelectTile(Point cursor)
        {
            int x = cursor.X;
            int y = cursor.Y;

            Tile tile = GetTilesetTile(x, y);

            SelectedTile = tile;
        }

        private static void SetGridProperties(Grid grid, int selectedOverlay)
        {
            if (grid == null)
            {
                return;
            }

            // Save the initial state
            Program.AddToInitialDrawState(grid);

            switch (selectedOverlay)
            {
                case (int)Overlays.Construction:
                    grid.SetConstructable(selectedValue);
                    Program.OverlayHasChanged();
                    break;

                case (int)Overlays.Drivable:
                    grid.SetDrivable(selectedValue);
                    Program.OverlayHasChanged();
                    break;

                case (int)Overlays.Flyable:
                    grid.SetFlyable(selectedValue);
                    Program.OverlayHasChanged();
                    break;

                case (int)Overlays.None:
                default:
                    // Update the grid's tile
                    grid.Tile = SelectedTile;

                    allUpdatedMapGrids.Enqueue(grid);
                    Program.TerrainHasChanged();
                    break;
            }

            // Save the final state
            Program.AddToFinalDrawState(grid);
        }

        public static void PenTool(Grid grid, int selectedOverlay)
        {
            if (selectedOverlay == (int)Overlays.Sectors)
            {
                return;
            }

            SetGridProperties(grid, selectedOverlay);
        }

        public static void LineTool(Point start, Point end, int selectedOverlay, double scale)
        {
            if (selectedOverlay == (int)Overlays.Sectors)
            {
                return;
            }

            List<Grid> selectedGrids = GetGridsInArea(start, end, scale);

            foreach (Grid grid in selectedGrids)
            {
                int scaledX = (int)Math.Round(grid.Corner.X * scale, 0);
                int scaledY = (int)Math.Round(grid.Corner.Y * scale, 0);
                int scaledLength = (int)Math.Round(TileLength * scale, 0);

                Rectangle bounds = new Rectangle(scaledX, scaledY, scaledLength, scaledLength);
                bool intersects = LineIntersectsRect(start, end, bounds);
                if (intersects == false)
                {
                    continue;
                }

                SetGridProperties(grid, selectedOverlay);
            }
        }

        public static void RectangleTool(Point start, Point end, int selectedOverlay, double scale)
        {
            if (selectedOverlay == (int)Overlays.Sectors)
            {
                return;
            }

            List<Grid> selectedGrids = GetGridsInArea(start, end, scale);

            foreach (Grid grid in selectedGrids)
            {
                SetGridProperties(grid, selectedOverlay);
            }
        }

        public static void FillTool(Grid grid, int selectedOverlay)
        {
            bool valueToChange;
            switch (selectedOverlay)
            {
                case (int)Overlays.Sectors:
                    return; // Don't add undo margin

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
                    if (tiletoChange.Equals(SelectedTile))
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
                Grid adjacentGrid = AllMapGrids.ElementAt(index);
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
                Grid adjacentGrid = AllMapGrids.ElementAt(index);
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
                Grid adjacentGrid = AllMapGrids.ElementAt(index);
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
                Grid adjacentGrid = AllMapGrids.ElementAt(index);
                if (adjacentGrid.Equals(previousGrid))
                {
                    continue;
                }

                FillFlyableRecursive(adjacentGrid, valueToChange, grid);
            }
        }

        public static void HandleSectorOverlayMouseEnter(MouseEventArgs e, bool editSectors, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editSectors == true)
            {
                // Sectors
                foreach (Sector sector in AllSectors.Values)
                {
                    bool mouseWithinBounds = sector.Area.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    sector.OnMouseEnter(e);
                    Cursor.Current = sector.Cursor;
                    OverlayHasChanged();
                }
            }
            else
            {
                // Checkpoints
                foreach (Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    bool mouseWithinBounds = checkpoint.Area.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    checkpoint.OnMouseEnter(e);
                    Cursor.Current = checkpoint.Cursor;
                    OverlayHasChanged();
                }
            }
        }

        public static void HandleSectorOverlayMouseLeave(MouseEventArgs e, bool editSectors, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editSectors == true)
            {
                // Sectors
                foreach (Sector sector in AllSectors.Values)
                {
                    sector.OnMouseLeave(e);
                    OverlayHasChanged();
                }
            }
            else
            {
                // Checkpoints
                foreach (Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    checkpoint.OnMouseLeave(e);
                    OverlayHasChanged();
                }
            }

            Cursor.Current = Cursors.Default;
        }

        public static void HandleSectorOverlayMouseMove(MouseEventArgs e, bool editSectors, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editSectors == true)
            {
                // Sectors
                foreach (Sector sector in AllSectors.Values)
                {
                    sector.OnMouseMove(e);

                    bool mouseWithinBounds = sector.Area.Contains(scaledCursor);
                    if (mouseWithinBounds == true)
                    {
                        sector.OnMouseEnter(e);
                        Cursor.Current = sector.Cursor;
                        OverlayHasChanged();
                        continue;
                    }

                    sector.OnMouseLeave(e);
                    OverlayHasChanged();
                }
            }
            else
            {
                // Checkpoints
                foreach (Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    checkpoint.OnMouseMove(e);

                    bool mouseWithinBounds = checkpoint.Area.Contains(scaledCursor);
                    if (mouseWithinBounds == true)
                    {
                        checkpoint.OnMouseEnter(e);
                        Cursor.Current = checkpoint.Cursor;
                        OverlayHasChanged();
                        continue;
                    }

                    checkpoint.OnMouseLeave(e);
                    OverlayHasChanged();
                }
            }
        }

        public static void HandleSectorOverlayMouseDown(MouseEventArgs e, bool editSectors, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editSectors == true)
            {
                // Sectors
                foreach (Sector sector in AllSectors.Values)
                {
                    bool mouseWithinBounds = sector.Area.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    sector.OnMouseDown(e);
                    Cursor.Current = sector.Cursor;
                }
            }
            else
            {
                // Checkpoints
                foreach (Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    bool mouseWithinBounds = checkpoint.Area.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    checkpoint.OnMouseDown(e);
                    Cursor.Current = checkpoint.Cursor;
                }
            }
        }

        private static Point GetScaledCursor(Point cursor, double scale)
        {
            int scaledX = (int)Math.Round(cursor.X / scale, 0);
            int scaledY = (int)Math.Round(cursor.Y / scale, 0);
            Point scaledCursor = new Point(scaledX, scaledY);
            return scaledCursor;
        }

        public static void HandleSectorOverlayMouseUp(MouseEventArgs e, bool editSectors, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            int scaledX = (int)Math.Round(cursor.X / scale, 0);
            int scaledY = (int)Math.Round(cursor.Y / scale, 0);
            Point scaledCursor = new Point(scaledX, scaledY);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledX, scaledY, e.Delta);

            if (editSectors == true)
            {
                // Sectors
                foreach (Sector sector in AllSectors.Values)
                {
                    bool selected = sector.HasMouseFocus;
                    if (selected == false)
                    {
                        continue;
                    }

                    sector.OnMouseUp(e);
                    Cursor.Current = sector.Cursor;

                    Rectangle[] removed = sector.GetRemoved();
                    foreach (Rectangle area in removed)
                    {
                        UpdateGridSectors(area, 0);
                    }

                    int sectorId = sector.SectorId;
                    Rectangle[] added = sector.GetAdded();
                    foreach (Rectangle area in added)
                    {
                        UpdateGridSectors(area, sectorId);
                    }
                }
            }
            else
            {
                // Checkpoints
                List<Checkpoint> checkpoints = new List<Checkpoint>(AllCheckpoints.Values);
                foreach (Checkpoint checkpoint in checkpoints)
                {
                    bool selected = checkpoint.HasMouseFocus;
                    if (selected == false)
                    {
                        continue;
                    }

                    checkpoint.OnMouseUp(e);
                    Cursor.Current = checkpoint.Cursor;
                }
            }
        }

        public static void UpdateGridSectors(Rectangle updatedArea, int updatedSectorId)
        {
            Point start = updatedArea.Location;
            int startX = start.X;
            int startY = start.Y;
            int width = updatedArea.Width;
            int height = updatedArea.Height;

            int endX = startX + width;
            int endY = startY + height;

            Point end = new Point(endX, endY);

            List<Grid> gridsToUpdate = GetGridsInArea(start, end, 1);
            foreach (Grid grid in gridsToUpdate)
            {
                grid.SectorId = updatedSectorId;
            }
        }

        public static int[] GetAdjacentGridIndexes(Grid center)
        {
            int tilesWide = mapSize.Width / TileLength;

            int totalTiles = AllMapGrids.Count;

            int centerIndex = AllMapGrids.IndexOf(center);

            List<int> adjacentGrids = new List<int>(4);

            // tile above
            if (centerIndex > tilesWide)
            {
                int aboveIndex = centerIndex - tilesWide;
                adjacentGrids.Add(aboveIndex);
            }

            // tile to the east
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

            // tile to the west
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
            if (SelectedTile == null)
            {
                return;
            }

            int index = SelectedTile.TilesetIndex;
            int x = (index % tilesWide) * TileLength;
            int y = (index / tilesWide) * TileLength;

            Rectangle bounds = new Rectangle(x, y, TileLength, TileLength);

            using (Graphics g = Graphics.FromImage(displayImage))
            {
                Pen pen = new Pen(Program.SelectionColour);
                g.DrawRectangle(pen, bounds);
            }
        }

        public static void InvalidateMap()
        {
            allUpdatedMapGrids = new Queue<Grid>(AllMapGrids);
            Program.TerrainHasChanged();
        }

        public static Bitmap GetMapImage(double scale)
        {
            int width = mapSize.Width;
            int height = mapSize.Height;

            int tilesWide = width / TileLength;
            int tilesHigh = height / TileLength;

            width = (int)(width * scale);
            height = (int)(height * scale);

            Bitmap displayImage = new Bitmap(width, height);

            Program.DrawAllGridsOntoImage(ref displayImage, AllMapGrids, tilesWide, TileLength, scale);

            Program.TerrainIsUpToDate();

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

            Program.TerrainIsUpToDate();

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
                // Draw oldSector outlines
                if (MaxSectors > ColourRoulette.Count)
                {
                    InitColourRoulette(MaxSectors * 2);
                }

                for (int i = 0; i < AllSectors.Values.Count; i++)
                {
                    if (AllSectors.Count == 0)
                    {
                        break;
                    }

                    int colourIndex = i % ColourRoulette.Count;
                    Color colour = ColourRoulette[colourIndex];
                    Pen pen = new Pen(colour, 2);

                    Sector currentSector = AllSectors.Values.ElementAt(i);
                    if (currentSector.IsMouseOver == true)
                    {
                        pen = new Pen(colour, 4);
                    }

                    pen.Alignment = PenAlignment.Inset;

                    Rectangle bounds = currentSector.GetScaledBounds(scale);

                    graphics.DrawRectangle(pen, bounds);

                    Font font = new Font(FontFamily.GenericSansSerif, 12f);

                    int scaledX = (int)Math.Round(currentSector.Location.X * scale, 0);
                    int scaledY = (int)Math.Round(currentSector.Location.Y * scale, 0);
                    Point scaledLocation = new Point(scaledX, scaledY);
                    graphics.DrawString(currentSector.SectorId.ToString(), font, Brushes.White, scaledLocation);
                }

                // Draw checkpoints
                foreach (Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    checkpoint.Draw(graphics, scale);
                }
            }
        }

        public static void DrawConstructibility(ref Bitmap displayImage, double scale)
        {
            using (Graphics graphics = Graphics.FromImage(displayImage))
            {
                foreach (Grid grid in AllMapGrids)
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
                foreach (Grid grid in AllMapGrids)
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
                foreach (Grid grid in AllMapGrids)
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

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<Grid> collection, int tilesWide, int tileLength)
        {
            Program.DrawAllGridsOntoImage(ref image, collection, tilesWide, tileLength, 1, false);
        }

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<Grid> collection, int tilesWide, int tileLength, double scale)
        {
            Program.DrawAllGridsOntoImage(ref image, collection, tilesWide, tileLength, scale, false);
        }

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<Grid> collection, int tilesWide, int tileLength, bool addPadding)
        {
            Program.DrawAllGridsOntoImage(ref image, collection, tilesWide, tileLength, 1, addPadding);
        }

        public static void DrawAllGridsOntoImage(ref Bitmap image, List<Grid> collection, int tilesWide, int tileLength, double scale, bool addPadding)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                Grid grid = collection[i];

                int x, y;

                //if (addPadding == true)
                //{
                //    x = i % tilesWide * (int)(TileLength * miniMapScale + 1);
                //    y = i / tilesWide * (int)(TileLength * miniMapScale + 1);
                //}
                //else
                //{
                //    x = i % tilesWide * (int)(TileLength * miniMapScale);
                //    y = i / tilesWide * (int)(TileLength * miniMapScale);
                //}

                if (addPadding == true)
                {
                    int column = grid.Corner.X / tileLength;
                    int row = grid.Corner.Y / tileLength;

                    x = (int)Math.Round((grid.Corner.X + column) * scale, 0);
                    y = (int)Math.Round((grid.Corner.Y + row) * scale, 0);
                }
                else
                {
                    x = (int)Math.Round(grid.Corner.X * scale, 0);
                    y = (int)Math.Round(grid.Corner.Y * scale, 0);
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

        public static void DrawTileOntoImage(ref Bitmap image, Tile tile, int tileLength, int x, int y)
        {
            DrawTileOntoImage(ref image, tile, tileLength, x, y, 1);
        }

        public static void DrawTileOntoImage(ref Bitmap image, Tile tile, int tileLength, int x, int y, double scale)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                Image tileImage = Program.GetTileImage(tile);

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

        public static Grid GetMapGrid(int x, int y, double scale)
        {
            return Program.GetGrid(x, y, scale, mapSize, AllMapGrids);
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

        public static List<Grid> GetGridsInArea(Point start, Point end, double scale)
        {
            // Get the rectangular area
            int cornerX = Math.Min(start.X, end.X);
            int cornerY = Math.Min(start.Y, end.Y);

            int width = Math.Abs(end.X - start.X);
            int height = Math.Abs(end.Y - start.Y);

            if (cornerX < 0)
            {
                width += cornerX;
                cornerX = 0;
            }
            if (cornerY < 0)
            {
                height += cornerY;
                cornerY = 0;
            }

            int normalCornerX = cornerX;
            int normalCornerY = cornerY;
            int normalWidth = width;
            int normalHeight = height;

            // Adjust rectangle to full miniMapScale
            if (scale != 1)
            {
                normalCornerX = (int)Math.Round(cornerX / scale, 0);
                normalCornerY = (int)Math.Round(cornerY / scale, 0);
                normalWidth = (int)Math.Round(width / scale, 0);
                normalHeight = (int)Math.Round(height / scale, 0);
            }

            // Snap to grid
            int remainder = normalCornerX % TileLength;
            normalCornerX -= remainder;
            normalWidth += remainder;

            remainder = normalCornerY % TileLength;
            normalCornerY -= remainder;
            normalHeight += remainder;

            remainder = normalWidth % TileLength;
            int difference = (TileLength - remainder) % TileLength;
            normalWidth += difference;

            remainder = normalHeight % TileLength;
            difference = (TileLength - remainder) % TileLength;
            normalHeight += difference;

            // Get all affected Grids
            List<Grid> selectedGrids = new List<Grid>();

            normalWidth += normalCornerX;
            normalHeight += normalCornerY;

            for (int y = normalCornerY; y < normalHeight; y += TileLength)
            {
                for (int x = normalCornerX; x < normalWidth; x += TileLength)
                {
                    Grid dummy = new Grid(new Point(x, y));
                    Grid grid = AllMapGrids.Find(g => g.Equals(dummy));
                    selectedGrids.Add(grid);
                }
            }

            return selectedGrids;
        }

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

        public static void AddToInitialDrawState(Grid grid)
        {
            if (currentDrawState == -1)
            {
                return;
            }

            DrawState state = allDrawStates.ElementAtOrDefault(currentDrawState);
            if (state == null)
            {
                state = new DrawState();
            }

            state.AddMemento(grid);
        }

        public static void AddToFinalDrawState(Grid grid)
        {
            if (currentDrawState == -1)
            {
                return;
            }

            DrawState state = allDrawStates.ElementAtOrDefault(currentDrawState);
            if (state == null)
            {
                state = new DrawState();
            }

            state.AddOmen(grid);
        }

        public static bool CheckCanUndo()
        {
            if (currentDrawState == -1)
            {
                return false;
            }

            return true;
        }

        public static void CancelDrawing()
        {
            bool canUndo = CheckCanUndo();
            if (canUndo == false)
            {
                return; // No previous nextState
            }

            RevertState();

            allDrawStates[currentDrawState] = new DrawState();
            Program.DisplayHasChanged();

            currentDrawState--;
            if (currentDrawState < -1)
            {
                currentDrawState = -1;
            }
        }

        public static void RevertState()
        {
            DrawState currentState = allDrawStates.ElementAtOrDefault(currentDrawState);
            if (currentState == null)
            {
                return;
            }
            if (currentState.IsEmpty == true)
            {
                return;
            }

            List<Grid> mementos = currentState.AllMementos;
            foreach (Grid previousState in mementos)
            {
                Grid grid = AllMapGrids.Find(g => g.Equals(previousState));

                grid.MatchCopy(previousState);

                allUpdatedMapGrids.Enqueue(grid);
            }
        }

        public static void Undo()
        {
            bool canUndo = CheckCanUndo();
            if (canUndo == false)
            {
                return; // No previous nextState
            }

            RevertState();
            Program.MapHasChanged();

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
            if (drawEvent.IsEmpty == true)
            {
                return false;
            }
            if (drawEvent.AllOmens.Count == 0)
            {
                return false;
            }

            return true;
        }

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
            List<Grid> omens = currentState.AllOmens;
            foreach (Grid nextState in omens)
            {
                Grid grid = AllMapGrids.Find(g => g.Equals(nextState));

                grid.MatchCopy(nextState);

                allUpdatedMapGrids.Enqueue(grid);
            }

            Program.MapHasChanged();
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

        public static void SetSelectedValue(bool value)
        {
            Program.selectedValue = value;
        }

        public static int CharToNumber(char letter)
        {
            int number = Char.ToUpper(letter) - 'A';
            return number;
        }

        public static char NumberToChar(int number)
        {
            char letter = (char)(number + 65);
            return letter;
        }

        public static void AddSector(double hScrollPercent, double vScrollPercent, Size displaySize)
        {
            int mapWidth = mapSize.Width;
            int mapHeight = mapSize.Height;

            int scrollBarLength = 25;
            int displayWidth = displaySize.Width;
            int displayHeight = displaySize.Height;

            int viewWidth = displayWidth - scrollBarLength;
            int viewHeight = displayHeight - scrollBarLength;

            int maxX = mapWidth - viewWidth;
            int maxY = mapHeight - viewHeight;

            int x = (int)Math.Round(hScrollPercent * maxX + viewWidth / 2, 0);
            int y = (int)Math.Round(vScrollPercent * maxY + viewHeight / 2, 0);

            // Snap to grid
            int remainder;
            remainder = x % TileLength;
            if (remainder >= (TileLength / 2))
            {
                x += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                x -= remainder;
            }

            remainder = y % TileLength;
            if (remainder >= (TileLength / 2))
            {
                y += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                y -= remainder;
            }

            int nextSectorId = MaxSectors + 1;
            int width = TileLength * 3;
            int height = TileLength * 3;
            Sector toAdd = new Sector(nextSectorId, TileLength, x, y, width, height);
            Rectangle areaToAdd = toAdd.Area;

            foreach (Sector sector in AllSectors.Values)
            {
                bool intersects = sector.Area.IntersectsWith(areaToAdd);
                if (intersects == false)
                {
                    continue;
                }

                return;
            }

            AllSectors.Add(nextSectorId, toAdd);
            UpdateGridSectors(areaToAdd, nextSectorId);

            Program.OverlayHasChanged();
        }

        public static void AddCheckPoint(double vScrollPercent, Size displaySize)
        {
            int mapWidth = mapSize.Width;
            int mapHeight = mapSize.Height;

            int scrollBarHeight = 25;
            int displayHeight = displaySize.Height;
            int viewHeight = displayHeight - scrollBarHeight;
            int maxY = mapHeight - viewHeight;

            int y = (int)Math.Round(vScrollPercent * maxY + viewHeight / 2, 0);

            // Snap to grid
            int remainder = y % TileLength;
            if (remainder >= (TileLength / 2))
            {
                y += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                y -= remainder;
            }

            bool alreadyExists = AllCheckpoints.ContainsKey(y);
            if (alreadyExists == true)
            {
                return;
            }

            Checkpoint checkpoint = new Checkpoint(y, mapWidth, TileLength);
            AllCheckpoints.Add(y, checkpoint);

            Program.OverlayHasChanged();
        }

        public static void MoveCheckpoint(Checkpoint checkpointToMove)
        {
            int oldY = checkpointToMove.Key;
            AllCheckpoints.Remove(oldY);

            int updatedY = checkpointToMove.Location.Y;
            AllCheckpoints.Add(updatedY, checkpointToMove);
        }

        public static Rectangle[] Difference(Rectangle rect1, Rectangle rect2)
        {
            int a = Math.Min(rect1.X, rect2.X);
            int b = Math.Max(rect1.X, rect2.X);
            int c = Math.Min(rect1.X + rect1.Width, rect2.X + rect2.Width);
            int d = Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width);

            int e = Math.Min(rect1.Y, rect2.Y);
            int f = Math.Max(rect1.Y, rect2.Y);
            int g = Math.Min(rect1.Y + rect1.Height, rect2.Y + rect2.Height);
            int h = Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height);

            Rectangle[] result = new Rectangle[6];

            // we'll always have rectangles 1, 3, 4 and 6
            result[0] = new Rectangle(b, e, c - b, f - e);
            result[1] = new Rectangle(a, f, b - a, g - f);
            result[2] = new Rectangle(c, d, f, g);
            result[3] = new Rectangle(b, g, c - b, h - g);

            // decide which corners
            bool corner1 = rect1.X == a && rect1.Y == e;
            bool corner2 = rect2.X == a && rect2.Y == e;

            if (corner1 || corner2)
            { // corners 0 and 7
                result[4] = new Rectangle(a, e, b - a, f - e);
                result[5] = new Rectangle(c, g, d - c, h - g);
            }
            else
            { // corners 2 and 5
                result[4] = new Rectangle(c, e, d - c, f - e);
                result[5] = new Rectangle(a, g, b - a, h - g);
            }

            return result;
        }

        public static bool LineIntersectsRect(Point start, Point end, Rectangle bounds)
        {
            bool check = bounds.Contains(start);
            if (check == true)
            {
                return true;
            }

            check = bounds.Contains(end);
            if (check == true)
            {
                return true;
            }

            check = LineIntersectsLine(start, end, new Point(bounds.X, bounds.Y), new Point(bounds.X + bounds.Width, bounds.Y));
            if (check == true)
            {
                return true;
            }

            check = LineIntersectsLine(start, end, new Point(bounds.X + bounds.Width, bounds.Y), new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height));
            if (check == true)
            {
                return true;
            }

            check = LineIntersectsLine(start, end, new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height), new Point(bounds.X, bounds.Y + bounds.Height));
            if (check == true)
            {
                return true;
            }

            check = LineIntersectsLine(start, end, new Point(bounds.X, bounds.Y + bounds.Height), new Point(bounds.X, bounds.Y));
            if (check == true)
            {
                return true;
            }

            return false;
        }

        private static bool LineIntersectsLine(Point lineAStart, Point lineAEnd, Point lineBStart, Point lineBEnd)
        {
            float q = (lineAStart.Y - lineBStart.Y) * (lineBEnd.X - lineBStart.X) - (lineAStart.X - lineBStart.X) * (lineBEnd.Y - lineBStart.Y);
            float d = (lineAEnd.X - lineAStart.X) * (lineBEnd.Y - lineBStart.Y) - (lineAEnd.Y - lineAStart.Y) * (lineBEnd.X - lineBStart.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (lineAStart.Y - lineBStart.Y) * (lineAEnd.X - lineAStart.X) - (lineAStart.X - lineBStart.X) * (lineAEnd.Y - lineAStart.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }
    }
}