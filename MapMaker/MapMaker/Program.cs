using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MapMaker
{
    public enum Overlay
    {
        None, Construction, Drivable, Flyable, Sectors, Zones
    }

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
        public const int DefaultTileLength = 32;
        public static double MapScale { get; set; }
        public static int TilesetDisplayWidth { get; set; }
        private static string tilesetFilename { get; set; }
        private static Bitmap tilesetImage { get; set; }
        private const string IMAGE_FILTER = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif)|*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif";
        private static string currentSaveFilename { get; set; }
        private static string currentSaveName { get; set; }
        private const string DEFAULT_FILENAME = "Map{0}";
        private const string JSON_EXTENSION = "json";
        private const string MAP_FILTER = "Map Files(*.json)|*.json";
        public static string Author { get; set; }
        private static DateTime dateCreated { get; set; }
        private static List<Tile> tileset;
        private static Size tilesetDisplaySize { get; set; }
        public static Color SelectionColour { get; set; }
        public static Tile SelectedTile { get; private set; }
        public static Region SelectedRegion { get; set; }
        private static bool selectedValue { get; set; }
        private static int selectedNumber { get; set; }
        public static List<Grid> AllMapGrids;
        private static Queue<Grid> allUpdatedMapGrids;
        public static Size MapSize { get; private set; }
        public static bool HasDisplayChanged { get; private set; }
        public static bool HasOverlayChanged { get; private set; }
        public static bool HasTerrainChanged { get; private set; }
        public static bool HasUnsavedChanges { get; private set; }
        public static Dictionary<int, Checkpoint> AllCheckpoints { get; set; }
        public static List<Spawnpoint> AllSpawnpoints { get; set; }
        public static Dictionary<int, Zone> AllZones { get; set; }  // 1 based
        public static Dictionary<int, Sector> AllSectors { get; set; }  // 1 based
        private static List<DrawState> allDrawStates { get; set; }
        private static int currentDrawState { get; set; }
        public static List<Color> ColourRoulette { get; private set; }

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
            SelectedRegion = null;
            AllMapGrids = new List<Grid>();
            allUpdatedMapGrids = new Queue<Grid>();
            HasDisplayChanged = false;
            HasOverlayChanged = false;
            HasTerrainChanged = false;
            HasUnsavedChanges = false;
            AllCheckpoints = new Dictionary<int, Checkpoint>();
            AllSpawnpoints = new List<Spawnpoint>();
            AllSectors = new Dictionary<int, Sector>();
            AllZones = new Dictionary<int, Zone>();
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

        public static void LoadSector(Sector sectorToLoad, Zone parentZone)
        {
            int sectorId = sectorToLoad.Id;

            bool sectorExists = AllSectors.ContainsKey(sectorId);
            if (sectorExists == false)
            {
                AllSectors.Add(sectorId, sectorToLoad);
            }

            sectorToLoad.SetParentZone(parentZone);

            Spawnpoint spawnToLoad = sectorToLoad.Spawn;
            spawnToLoad.SetParentSector(sectorToLoad);
            Program.AllSpawnpoints.Add(spawnToLoad);

            int sectors = AllSectors.Count;
            if (ColourRoulette.Count >= sectors)
            {
                return;
            }

            InitColourRoulette(sectors * 2);
        }

        public static void LoadZonesAndSectors(List<Zone> allZonesToLoad)
        {
            int maxWidth = MapSize.Width;

            foreach (Zone zoneToLoad in allZonesToLoad)
            {
                int zoneId = zoneToLoad.Id;

                bool zoneExists = AllZones.ContainsKey(zoneId);
                if (zoneExists == false)
                {
                    AllZones.Add(zoneId, zoneToLoad);
                }

                zoneToLoad.UpdateWidth(maxWidth);   // Ensure each zone is same width as level

                foreach(Sector sector in zoneToLoad.AllSectors)
                {
                    LoadSector(sector, zoneToLoad);
                }
            }

            int sectors = AllSectors.Count;
            if (ColourRoulette.Count >= sectors)
            {
                return;
            }

            InitColourRoulette(sectors * 2);
        }

        public static void LoadCheckpoints(List<Checkpoint> checkpoints)
        {
            if (checkpoints == null)
            {
                return;
            }

            foreach (Checkpoint toAdd in checkpoints)
            {
                toAdd.SetBorders();

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
            MapPropertiesForm mapPropertiesForm = new MapPropertiesForm(MapSize, tilesetFilename, TileLength, Author, dateCreated.ToString());
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

            //int tilesWide = imageWidth / (tileLength * 2);    // RPG tileset tiles are 2x3 to allow for tiling
            //int tilesHigh = imageHeight / (tileLength * 3);
            int tilesWide = imageWidth / tileLength;
            int tilesHigh = imageHeight / tileLength;

            int totalTiles = tilesWide * tilesHigh;

            for (int i = 0; i < totalTiles; i++)
            {
                //int x = (i % tilesWide) * (2 * tileLength);
                //int y = (i / tilesWide) * (3 * tileLength);
                int x = (i % tilesWide);
                int y = (i / tilesWide);

                Tile tileToAdd = new Tile(i, x, y);
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

        public static void BuildMap(Size mapSize)
        {
            BuildMap(mapSize.Width, mapSize.Height);
        }

        public static void BuildMap(int width, int height)
        {
            if (tileset.Count == 0)
            {
                throw new InvalidProgramException("Tileset has not been loaded");
            }

            Tile defaultTile = tileset[0];

            Program.MapSize = new Size(width, height);
            AllMapGrids = new List<Grid>();

            // Set the inital nextState
            Program.InitDrawState();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Grid gridToAdd = new Grid(x, y, defaultTile);
                    AllMapGrids.Add(gridToAdd);
                }
            }

            // Sort by x, y
            //AllMapGrids.Sort();

            // Sort by Id
            AllMapGrids.Sort((a, b) => a.Id.CompareTo(b.Id));

            Program.MapHasChanged();
        }

        public static void ResizeMap(int width, int height)
        {
            Tile defaultTile = tileset[0];

            int oldWidth = MapSize.Width;
            int oldHeight = MapSize.Height;

            int deltaWidth = width - oldWidth;
            int deltaHeight = height - oldHeight;

            if(deltaWidth == 0)
            {
                if(deltaHeight == 0)
                {
                    return;
                }
            }

            Program.MapSize = new Size(width, height);

            // Set the inital nextState
            Program.InitDrawState();

            int maxWidth = Math.Max(width, oldWidth);
            int maxHeight = Math.Max(height, oldHeight);

            for (int y = 0; y < maxHeight; y++)
            {
                for (int x = 0; x < maxWidth; x++)
                {
                    Grid dummy = new Grid(x, y, defaultTile);

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

            // If map is wider, expand zones and checkpoints
            if(maxWidth > oldWidth)
            {
                foreach(Zone zone in AllZones.Values)
                {
                    zone.UpdateWidth(maxWidth);
                }

                foreach(Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    checkpoint.UpdateWidth(maxWidth);
                }
            }

            // Sort by x, y
            //AllMapGrids.Sort();

            // Sort by Id
            AllMapGrids.Sort((a, b) => a.Id.CompareTo(b.Id));

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
            //Program.TileLength = map.TileLength;  // Tilelength has to be set before loading map
            Program.MapSize = map.MapSize;

            Program.AllMapGrids = map.AllMapGrids;

            // Sort by x, y
            //AllMapGrids.Sort();

            // Sort by Id
            AllMapGrids.Sort((a, b) => a.Id.CompareTo(b.Id));

            // Set the initial draw margin
            Program.InitDrawState();

            // Load Checkpoints
            Program.LoadCheckpoints(map.AllCheckpoints);

            // Create Zones and Sectors
            Program.LoadZonesAndSectors(map.AllZones);

            Program.LoadTilesetImageFromFile(tilesetFilename);
            Program.BuildTileset(TileLength, TilesetDisplayWidth);

            Program.currentSaveFilename = filename;
            Program.currentSaveName = GetMapName(filename);

            Program.HasUnsavedChanges = false;
        }

        private static string GetMapName(string filename)
        {
            int index = filename.LastIndexOf('\\');
            string fullMapName = filename.Substring(index + 1);

            index = fullMapName.LastIndexOf('.');

            string mapName = fullMapName.Substring(0, index);
            return mapName;
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
            string defaultFilename = string.Format("{0}.{1}", string.Format(DEFAULT_FILENAME, "001"), JSON_EXTENSION);
            if (currentSaveFilename != null)
            {
                defaultFilename = currentSaveFilename;
            }

            //string title = "Save Map file";

            return GetSaveDialog(defaultFilename, MAP_FILTER);
        }

        public static SaveFileDialog GetImageSaveDialog()
        {
            string pngExtension = "png";
            string defaultFilename = string.Format("{0}.{1}", string.Format(DEFAULT_FILENAME, "001"), pngExtension);
            if (currentSaveName != null)
            {
                defaultFilename = string.Format("{0}.{1}", currentSaveName, pngExtension);
            }

            return GetSaveDialog(defaultFilename, IMAGE_FILTER);
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

        public static string GetMapFilenameToSave()
        {
            // Get the Filename of the selected file
            SaveFileDialog dialog = Program.GetMapSaveDialog();
            return GetFilenameToSave(dialog);
        }

        public static string GetImageFilenameToSave()
        {
            // Get the Filename of the selected file
            SaveFileDialog dialog = Program.GetImageSaveDialog();
            return GetFilenameToSave(dialog);
        }

        private static string GetFilenameToSave(SaveFileDialog dialog)
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

        public static void SaveMap()
        {
            string filename = Program.GetMapFilenameToSave();
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
            int nextSectorId = AllSectors.Count + 1;
            int nextZoneId = AllZones.Count + 1;

            StrikeforceMap map = new StrikeforceMap(Author, dateCreated, tilesetFilename, TileLength, nextSectorId, nextZoneId,
                MapSize, AllMapGrids, AllZones.Values.ToList(), AllCheckpoints.Values.ToList());
            string json = Program.SerializeMap(map);

            WriteTextFile(filename, json);

            HasUnsavedChanges = false;
        }

        public static void SaveImageFile()
        {
            string filename = Program.GetImageFilenameToSave();
            if (filename == null)
            {
                return;
            }

            Bitmap image = Program.GetMapImage(1);
            image.Save(filename);
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
            // Get tile length first
            int tileLength = DeserializeTileLength(json);
            Program.TileLength = tileLength;

            StrikeforceMap map = JsonConvert.DeserializeObject<StrikeforceMap>(json);
            return map;
        }

        public static int DeserializeTileLength(string json)
        {
            string pattern = "\"TileLength\"" + @": (\d+),";
            Regex regex = new Regex(pattern);
            Match firstMatch = regex.Match(json);
            if (firstMatch == null)
            {
                return DefaultTileLength;
            }

            for (int groupIndex = 1; groupIndex < firstMatch.Groups.Count; groupIndex++)
            {
                Group group = firstMatch.Groups[groupIndex];

                for (int captureIndex = 0; captureIndex < group.Captures.Count; captureIndex++)
                {
                    string tileLengthString = group.Captures[captureIndex].Value;

                    int tileLength;
                    if (Int32.TryParse(tileLengthString, out tileLength) == false)
                    {
                        continue;
                    }

                    return tileLength;
                }
            }

            return DefaultTileLength;
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

        private static void SetGridProperties(Grid grid, Overlay selectedOverlay)
        {
            if (grid == null)
            {
                return;
            }

            // Save the initial state
            Program.AddToInitialDrawState(grid);

            switch (selectedOverlay)
            {
                case Overlay.Construction:
                    grid.SetConstructable(selectedValue);
                    Program.OverlayHasChanged();
                    break;

                case Overlay.Drivable:
                    grid.SetDrivable(selectedValue);
                    Program.OverlayHasChanged();
                    break;

                case Overlay.Flyable:
                    grid.SetFlyable(selectedValue);
                    Program.OverlayHasChanged();
                    break;

                case Overlay.None:
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

        public static void PenTool(Grid grid, Overlay selectedOverlay)
        {
            if (selectedOverlay == Overlay.Sectors)
            {
                return;
            }

            SetGridProperties(grid, selectedOverlay);
        }

        public static void LineTool(Point startPixel, Point endPixel, Overlay selectedOverlay, double scale)
        {
            if (selectedOverlay == Overlay.Sectors)
            {
                return;
            }

            List<Grid> selectedGrids = GetGridsInArea(startPixel, endPixel, scale);

            foreach (Grid grid in selectedGrids)
            {
                int scaledPixelX = (int)Math.Round(grid.Location.X * TileLength * scale, 0);
                int scaledPixelY = (int)Math.Round(grid.Location.Y * TileLength * scale, 0);
                int scaledPixelLength = (int)Math.Round(TileLength * scale, 0);

                Rectangle bounds = new Rectangle(scaledPixelX, scaledPixelY, scaledPixelLength, scaledPixelLength);
                bool intersects = LineIntersectsRect(startPixel, endPixel, bounds);
                if (intersects == false)
                {
                    continue;
                }

                SetGridProperties(grid, selectedOverlay);
            }
        }

        public static void RectangleTool(Point start, Point end, Overlay selectedOverlay, double scale)
        {
            if (selectedOverlay == Overlay.Sectors)
            {
                return;
            }

            List<Grid> selectedGrids = GetGridsInArea(start, end, scale);

            foreach (Grid grid in selectedGrids)
            {
                SetGridProperties(grid, selectedOverlay);
            }
        }

        public static void FillTool(Grid grid, Overlay selectedOverlay)
        {
            bool valueToChange;
            switch (selectedOverlay)
            {
                case Overlay.Sectors:
                    return; // Don't add undo margin

                case Overlay.Construction:
                    valueToChange = grid.AllowsConstruction;
                    FillConstructionRecursive(grid, valueToChange, grid);
                    break;

                case Overlay.Drivable:
                    valueToChange = grid.AllowsDriving;
                    FillDrivableRecursive(grid, valueToChange, grid);
                    break;

                case Overlay.Flyable:
                    valueToChange = grid.AllowsFlying;
                    FillFlyableRecursive(grid, valueToChange, grid);
                    break;

                case Overlay.None:
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

            Program.PenTool(grid, (int)Overlay.None);

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

            Program.PenTool(grid, Overlay.Construction);

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

            Program.PenTool(grid, Overlay.Drivable);

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

            Program.PenTool(grid, Overlay.Flyable);

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

        private static Point GetScaledCursor(Point cursor, double scale)
        {
            int scaledX = (int)Math.Round(cursor.X / scale, 0);
            int scaledY = (int)Math.Round(cursor.Y / scale, 0);
            Point scaledCursor = new Point(scaledX, scaledY);
            return scaledCursor;
        }

        private static void SetSelectedRegion(Region region)
        {
            if (SelectedRegion == region)
            {
                SelectedRegion = null;
                return;
            }

            SelectedRegion = region;
        }

        public static void HandleZoneOverlayMouseClick(MouseEventArgs e, bool editZones, double scale)
        {
            if (e.Button == MouseButtons.Right)
            {
                Program.SelectedRegion = null;
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editZones == true)
            {
                // Zones
                foreach (Zone zone in AllZones.Values)
                {
                    bool mouseWithinCenterBounds = zone.IsInCenter(scaledCursor);
                    if (mouseWithinCenterBounds == false)
                    {
                        continue;
                    }

                    zone.OnMouseClick(e);
                    SetSelectedRegion(zone);
                    OverlayHasChanged();
                }
            }
            else
            {
                // Checkpoints
                foreach (Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    bool mouseWithinBounds = checkpoint.PixelArea.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    checkpoint.OnMouseClick(e);
                    SetSelectedRegion(checkpoint);
                    OverlayHasChanged();
                }
            }
        }

        public static void HandleSectorOverlayMouseClick(MouseEventArgs e, bool editSectors, double scale)
        {
            if (e.Button == MouseButtons.Right)
            {
                Program.SelectedRegion = null;
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editSectors == true)
            {
                // Sectors
                foreach (Sector sector in AllSectors.Values)
                {
                    bool mouseWithinCenterBounds = sector.IsInCenter(scaledCursor);
                    if (mouseWithinCenterBounds == false)
                    {
                        continue;
                    }

                    sector.OnMouseClick(e);
                    SetSelectedRegion(sector);
                    OverlayHasChanged();
                }
            }
            else
            {
                // Spawnpoints
                foreach (Spawnpoint spawnpoint in AllSpawnpoints)
                {
                    bool mouseWithinBounds = spawnpoint.PixelArea.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    spawnpoint.OnMouseClick(e);
                    SetSelectedRegion(spawnpoint);
                    OverlayHasChanged();
                }
            }
        }

        public static void HandleSectorOverlayMouseDoubleClick(MouseEventArgs e, bool editSectors, double scale)
        {
            if (editSectors == true)
            {
                return;
            }

            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            foreach (Spawnpoint spawnpoint in AllSpawnpoints)
            {
                bool mouseWithinBounds = spawnpoint.PixelArea.Contains(scaledCursor);
                if (mouseWithinBounds == false)
                {
                    continue;
                }

                spawnpoint.OnDoubleClick(e);
            }
        }

        public static void HandleZoneOverlayMouseEnter(MouseEventArgs e, bool editZones, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editZones == true)
            {
                // Zones
                foreach (Zone zone in AllZones.Values)
                {
                    bool mouseWithinBounds = zone.PixelArea.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    zone.OnMouseEnter(e);
                    Cursor.Current = zone.Cursor;
                    OverlayHasChanged();
                }
            }
            else
            {
                // Checkpoints
                foreach (Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    bool mouseWithinBounds = checkpoint.PixelArea.Contains(scaledCursor);
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
                    bool mouseWithinBounds = sector.PixelArea.Contains(scaledCursor);
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
                // Spawnpoints
                foreach (Spawnpoint spawnpoint in AllSpawnpoints)
                {
                    bool mouseWithinBounds = spawnpoint.PixelArea.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    spawnpoint.OnMouseEnter(e);
                    Cursor.Current = spawnpoint.Cursor;
                    OverlayHasChanged();
                }
            }
        }

        public static void HandleZoneOverlayMouseLeave(MouseEventArgs e, bool editZones, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editZones == true)
            {
                // Sectors
                foreach (Zone zone in AllZones.Values)
                {
                    zone.OnMouseLeave(e);
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
                // Spawnpoints
                foreach (Spawnpoint spawnpoint in AllSpawnpoints)
                {
                    spawnpoint.OnMouseLeave(e);
                    OverlayHasChanged();
                }
            }

            Cursor.Current = Cursors.Default;
        }

        public static void HandleZoneOverlayMouseMove(MouseEventArgs e, bool editZones, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editZones == true)
            {
                // Zones
                foreach (Zone zone in AllZones.Values)
                {
                    zone.OnMouseMove(e);

                    bool mouseWithinBounds = zone.PixelArea.Contains(scaledCursor);
                    if (mouseWithinBounds == true)
                    {
                        zone.OnMouseEnter(e);
                        Cursor.Current = zone.Cursor;
                        OverlayHasChanged();
                        continue;
                    }

                    zone.OnMouseLeave(e);
                    OverlayHasChanged();
                }
            }
            else
            {
                // Checkpoints
                foreach (Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    checkpoint.OnMouseMove(e);

                    bool mouseWithinBounds = checkpoint.PixelArea.Contains(scaledCursor);
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

                    bool mouseWithinBounds = sector.PixelArea.Contains(scaledCursor);
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
                // Spawnpoints
                foreach (Spawnpoint spawnpoint in AllSpawnpoints)
                {
                    spawnpoint.OnMouseMove(e);

                    bool mouseWithinBounds = spawnpoint.PixelArea.Contains(scaledCursor);
                    if (mouseWithinBounds == true)
                    {
                        spawnpoint.OnMouseEnter(e);
                        Cursor.Current = spawnpoint.Cursor;
                        OverlayHasChanged();
                        continue;
                    }

                    spawnpoint.OnMouseLeave(e);
                    OverlayHasChanged();
                }
            }
        }

        public static void HandleZoneOverlayMouseDown(MouseEventArgs e, bool editZones, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            Point scaledCursor = GetScaledCursor(cursor, scale);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledCursor.X, scaledCursor.Y, e.Delta);

            if (editZones == true)
            {
                // Zones
                foreach (Zone zone in AllZones.Values)
                {
                    bool mouseWithinBounds = zone.PixelArea.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    zone.OnMouseDown(e);
                    Cursor.Current = zone.Cursor;
                }
            }
            else
            {
                // Checkpoints
                foreach (Checkpoint checkpoint in AllCheckpoints.Values)
                {
                    bool mouseWithinBounds = checkpoint.PixelArea.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    checkpoint.OnMouseDown(e);
                    Cursor.Current = checkpoint.Cursor;
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
                    bool mouseWithinBounds = sector.PixelArea.Contains(scaledCursor);
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
                // Spawnpoints
                foreach (Spawnpoint spawnpoint in AllSpawnpoints)
                {
                    bool mouseWithinBounds = spawnpoint.PixelArea.Contains(scaledCursor);
                    if (mouseWithinBounds == false)
                    {
                        continue;
                    }

                    spawnpoint.OnMouseDown(e);
                    Cursor.Current = spawnpoint.Cursor;
                }
            }
        }

        public static void HandleZoneOverlayMouseUp(MouseEventArgs e, bool editZones, double scale)
        {
            Point cursor = e.Location;

            // Adjust cursor to miniMapScale
            int scaledX = (int)Math.Round(cursor.X / scale, 0);
            int scaledY = (int)Math.Round(cursor.Y / scale, 0);
            Point scaledCursor = new Point(scaledX, scaledY);
            e = new MouseEventArgs(e.Button, e.Clicks, scaledX, scaledY, e.Delta);

            if (editZones == true)
            {
                // Zones
                foreach (Zone zone in AllZones.Values)
                {
                    bool selected = zone.HasMouseFocus;
                    if (selected == false)
                    {
                        continue;
                    }

                    zone.OnMouseUp(e);
                    Cursor.Current = zone.Cursor;
                }
            }
            else
            {
                // Checkpoints
                List<Checkpoint> allCheckpoints = AllCheckpoints.Values.ToList();   // Need seperate list to avoid iteration exceptions
                foreach (Checkpoint checkpoint in allCheckpoints)
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
                }
            }
            else
            {
                // Spawnpoints
                foreach (Spawnpoint spawnpoint in AllSpawnpoints)
                {
                    bool selected = spawnpoint.HasMouseFocus;
                    if (selected == false)
                    {
                        continue;
                    }

                    spawnpoint.OnMouseUp(e);
                    Cursor.Current = spawnpoint.Cursor;
                }
            }
        }

        public static void DeleteSelectedRegion()
        {
            if (SelectedRegion == null)
            {
                return;
            }

            SelectedRegion.Delete();
            SelectedRegion = null;
        }

        public static int[] GetAdjacentGridIndexes(Grid center)
        {
            int tilesWide = MapSize.Width / TileLength;

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
            int width = tilesWide * TileLength; // this is to ensure no tiles are cut off

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
            int tilesWide = MapSize.Width;
            int tilesHigh = MapSize.Height;

            int pixelWidth = tilesWide * TileLength;
            int pixelHeight = tilesHigh * TileLength;

            pixelWidth = (int)(pixelWidth * scale);
            pixelHeight = (int)(pixelHeight * scale);

            Bitmap displayImage = new Bitmap(pixelWidth, pixelHeight);

            Program.DrawAllGridsOntoImage(ref displayImage, AllMapGrids, tilesWide, TileLength, scale);

            Program.TerrainIsUpToDate();

            return displayImage;
        }

        public static Bitmap DrawMap(ref Bitmap displayImage, double scale)
        {
            int pixelWidth = MapSize.Width * TileLength; // Convert to pixels
            int scaledPixelWidth = (int)(pixelWidth * scale);
            if (scaledPixelWidth != displayImage.Width)
            {
                int scaledPixelHeight = (int)(MapSize.Height * scale * TileLength);  // Convert to pixels
                displayImage = new Bitmap(scaledPixelWidth, scaledPixelHeight);
            }

            int tilesWide = MapSize.Width;

            Program.DrawUpdatedGridsOntoImage(ref displayImage, tilesWide, TileLength, scale);

            Program.TerrainIsUpToDate();

            return displayImage;
        }

        private static void DrawLabel(Graphics graphics, double scale, Grid grid, string label)
        {
            Point gridCorner = grid.Location;
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
            Point gridCorner = grid.Location;
            int length = (int)(TileLength * scale);
            int radius = (int)(TileLength * 0.2);
            int cornerPixelX = (int)(gridCorner.X * TileLength * scale);
            int cornerPixelY = (int)(gridCorner.Y * TileLength * scale);

            cornerPixelX += length / 2 - radius;
            cornerPixelY += length / 2 - radius;

            int diameter = radius * 2;
            Rectangle bounds = new Rectangle(cornerPixelX, cornerPixelY, diameter, diameter);

            Brush brush = new SolidBrush(colour);

            graphics.FillEllipse(brush, bounds);
        }

        public static void DrawSectorView(ref Bitmap displayImage, double scale)
        {
            using (Graphics graphics = Graphics.FromImage(displayImage))
            {
                // Draw oldSector outlines
                int sectors = AllSectors.Count;
                if (sectors > ColourRoulette.Count)
                {
                    InitColourRoulette(sectors * 2);
                }

                Color colour;
                for (int i = 0; i < AllSectors.Values.Count; i++)
                {
                    if (AllSectors.Count == 0)
                    {
                        return;
                    }

                    int colourIndex = i % ColourRoulette.Count;
                    colour = ColourRoulette[colourIndex];

                    Sector currentSector = AllSectors.Values.ElementAt(i);
                    currentSector.Draw(graphics, scale, colour);

                    // Draw Spawnpoints
                    Spawnpoint spawn = currentSector.Spawn;
                    spawn.Draw(graphics, scale);
                }

                // Draw Zones around Sectors
                colour = Color.Black;
                for (int i = 0; i < AllZones.Values.Count; i++)
                {
                    Pen pen = new Pen(colour, 2);
                    pen.Alignment = PenAlignment.Inset;

                    Zone currentZone = AllZones.Values.ElementAt(i);
                    Rectangle bounds = currentZone.GetScaledBounds(scale);
                    graphics.DrawRectangle(pen, bounds);

                }
            }
        }

        public static void DrawZoneView(ref Bitmap displayImage, double scale)
        {
            using (Graphics graphics = Graphics.FromImage(displayImage))
            {
                // Draw oldSector outlines
                int zones = AllZones.Count;
                if (zones > ColourRoulette.Count)
                {
                    InitColourRoulette(zones * 2);
                }

                for (int i = 0; i < AllZones.Values.Count; i++)
                {
                    if (AllZones.Count == 0)
                    {
                        break;
                    }

                    int colourIndex = i % ColourRoulette.Count;
                    Color colour = ColourRoulette[colourIndex];

                    Zone currentZone = AllZones.Values.ElementAt(i);
                    currentZone.Draw(graphics, scale, colour);
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
                int pixelX, pixelY;   // pixels

                if (addPadding == true)
                {
                    pixelX = i % tilesWide * (int)(tileLength * scale + 1);
                    pixelY = i / tilesWide * (int)(tileLength * scale + 1);
                }
                else
                {
                    pixelX = i % tilesWide * (int)(tileLength * scale);
                    pixelY = i / tilesWide * (int)(tileLength * scale);
                }

                Program.DrawTileOntoImage(ref image, tile, tileLength, pixelX, pixelY, scale);
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

                int pixelX = grid.Location.X * TileLength;
                int pixelY = grid.Location.Y * TileLength;

                //if (addPadding == true)
                //{
                //    deltaX = i % tilesWide * (int)(TileLength * miniMapScale + 1);
                //    deltaY = i / tilesWide * (int)(TileLength * miniMapScale + 1);
                //}
                //else
                //{
                //    deltaX = i % tilesWide * (int)(TileLength * miniMapScale);
                //    deltaY = i / tilesWide * (int)(TileLength * miniMapScale);
                //}

                int scaledPixelX, scaledPixelY;
                if (addPadding == true)
                {
                    int column = grid.Location.X;
                    int row = grid.Location.Y;

                    scaledPixelX = (int)Math.Round((pixelX + column) * scale, 0);   // convert to pixels
                    scaledPixelY = (int)Math.Round((pixelY + row) * scale, 0);   // convert to pixels
                }
                else
                {
                    scaledPixelX = (int)Math.Round(pixelX * scale, 0);   // convert to pixels
                    scaledPixelY = (int)Math.Round(pixelY * scale, 0);   // convert to pixels
                }

                Tile tile = grid.Tile;

                Program.DrawTileOntoImage(ref image, tile, tileLength, pixelX, pixelY, scale);
            }
        }

        public static void DrawUpdatedGridsOntoImage(ref Bitmap image, int tilesWide, int tileLength, double scale)
        {
            while (allUpdatedMapGrids.Count > 0)
            {
                Grid grid = allUpdatedMapGrids.Dequeue();
                Point corner = grid.Location;
                int scaledPixelX = (int)(corner.X * scale * tileLength);    // convert to pixels
                int scaledPixelY = (int)(corner.Y * scale * tileLength);    // convert to pixels

                Tile tile = grid.Tile;

                Program.DrawTileOntoImage(ref image, tile, tileLength, scaledPixelX, scaledPixelY, scale);
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

        public static Grid GetMapGrid(int pixelX, int pixelY, double scale)
        {
            return Program.GetGrid(pixelX, pixelY, scale, MapSize, AllMapGrids);
        }

        public static Grid GetGrid(int pixelX, int pixelY, double scale, Size size, List<Grid> collection)
        {
            int tilesWide = size.Width;

            int x = pixelX / (int)(TileLength * scale);
            int y = pixelY / (int)(TileLength * scale);

            Grid dummy = new Grid(x, y);
            Grid gridToFind = collection.Find(g => g.Equals(dummy));

            return gridToFind;
        }

        public static List<Grid> GetGridsInArea(Point startPixel, Point endPixel)
        {
            return GetGridsInArea(startPixel, endPixel, 1);
        }

        public static List<Grid> GetGridsInArea(Point startPixel, Point endPixel, double scale)
        {
            // Get the rectangular area
            int cornerPixelX = Math.Min(startPixel.X, endPixel.X);
            int cornerPixelY = Math.Min(startPixel.Y, endPixel.Y);

            int pixelWidth = Math.Abs(endPixel.X - startPixel.X);
            int pixelHeight = Math.Abs(endPixel.Y - startPixel.Y);

            if (cornerPixelX < 0)
            {
                pixelWidth += cornerPixelX;
                cornerPixelX = 0;
            }
            if (cornerPixelY < 0)
            {
                pixelHeight += cornerPixelY;
                cornerPixelY = 0;
            }

            int normalCornerPixelX = cornerPixelX;
            int normalCornerPixelY = cornerPixelY;
            int normalPixelWidth = pixelWidth;
            int normalPixelHeight = pixelHeight;

            // Adjust rectangle to full miniMapScale
            if (scale != 1)
            {
                normalCornerPixelX = (int)Math.Round(cornerPixelX / scale, 0);
                normalCornerPixelY = (int)Math.Round(cornerPixelY / scale, 0);
                normalPixelWidth = (int)Math.Round(pixelWidth / scale, 0);
                normalPixelHeight = (int)Math.Round(pixelHeight / scale, 0);
            }

            // Snap to grid
            int remainder = normalCornerPixelX % TileLength;
            normalCornerPixelX -= remainder;
            normalPixelWidth += remainder;

            remainder = normalCornerPixelY % TileLength;
            normalCornerPixelY -= remainder;
            normalPixelHeight += remainder;

            remainder = normalPixelWidth % TileLength;
            int difference = (TileLength - remainder) % TileLength;
            normalPixelWidth += difference;

            remainder = normalPixelHeight % TileLength;
            difference = (TileLength - remainder) % TileLength;
            normalPixelHeight += difference;

            // Get all affected Grids
            List<Grid> selectedGrids = new List<Grid>();

            // Convert pixels to units (tiles)
            int startGridX = normalCornerPixelX / TileLength;
            int startGridY = normalCornerPixelY / TileLength;
            int normalWidth = normalPixelWidth / TileLength;
            int normalHeight = normalPixelHeight / TileLength;

            // Keep selection within the bounds of the map
            startGridX = Math.Max(startGridX, 0);
            startGridY = Math.Max(startGridY, 0);
            normalWidth = Math.Min(normalWidth, MapSize.Width - startGridX);
            normalHeight = Math.Min(normalHeight, MapSize.Height - startGridY);

            int endGridX = startGridX + normalWidth;
            int endGridY = startGridY + normalHeight;

            for (int y = startGridY; y < endGridY; y++)
            {
                for (int x = startGridX; x < endGridX; x++)
                {
                    Grid dummy = new Grid(x, y);
                    Grid grid = AllMapGrids.Find(g => g.Equals(dummy));
                    if (grid == null)
                    {
                        // Map file is corrupted, attempting to repair
                        Grid previousGrid = selectedGrids[selectedGrids.Count - 1];
                        int previousGridId = previousGrid.Id;

                        int nextGridId = previousGridId + 1;
                        Point location = new Point(x, y);

                        Console.Write(string.Format("Map file is corrupted. Attempting to repair. Replacing grid {0}", nextGridId));

                        grid = new Grid(nextGridId, location, previousGrid.AllowsDriving, previousGrid.AllowsFlying, previousGrid.AllowsConstruction, previousGrid.Tile);
                        AllMapGrids.Add(grid);
                        AllMapGrids.Sort((a, b) => a.Id.CompareTo(b.Id));
                    }

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

            Point corner = tile.Location;
            int pixelX = corner.X * TileLength;   // Convert to pixels
            int pixelY = corner.Y * TileLength;   // Convert to pixels

            Rectangle bounds = new Rectangle(pixelX, pixelY, TileLength, TileLength);
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

            // Remove excess undo history
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

        public static void AddToInitialDrawState(Zone zone)
        {
            if(currentDrawState == -1)
            {
                return;
            }

            DrawState state = allDrawStates.ElementAtOrDefault(currentDrawState);
            if(state == null)
            {
                state = new DrawState();
            }

            state.AddMemento(zone);
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

        public static void AddToFinalDrawState(Zone zone)
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

            state.AddOmen(zone);
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

            List<Grid> gridMementos = currentState.AllGridMementos;
            foreach (Grid previousState in gridMementos)
            {
                Grid grid = AllMapGrids.Find(g => g.Equals(previousState));

                grid.MatchCopy(previousState);

                allUpdatedMapGrids.Enqueue(grid);
            }

            List<Zone> zoneMementos = currentState.AllZoneMementos;
            foreach(Zone previousState in zoneMementos)
            {
                Zone zone = AllZones[previousState.Id];

                zone.MatchCopy(previousState);
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
            if (drawEvent.AllGridOmens.Count == 0)
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
            List<Grid> omens = currentState.AllGridOmens;
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

        private static Point GetMapPoint(double hScrollPercent, double vScrollPercent, Size displayPixelSize)
        {
            int mapPixelWidth = MapSize.Width * TileLength;
            int mapPixelHeight = MapSize.Height * TileLength;

            int scrollBarLength = 25;
            int displayPixelWidth = displayPixelSize.Width;
            int displayPixelHeight = displayPixelSize.Height;

            int viewPixelWidth = displayPixelWidth - scrollBarLength;
            int viewPixelHeight = displayPixelHeight - scrollBarLength;

            int maxPixelX = mapPixelWidth - viewPixelWidth;
            int maxPixelY = mapPixelHeight - viewPixelHeight;

            int pixelX = (int)Math.Round(hScrollPercent * maxPixelX + viewPixelWidth / 2, 0);
            int pixelY = (int)Math.Round(vScrollPercent * maxPixelY + viewPixelHeight / 2, 0);

            // Snap to grid
            Point mapPoint = SnapToGrid(pixelX, pixelY);
            return mapPoint;
        }

        public static Point SnapToGrid(int pixelX, int pixelY)
        {
            int remainder;
            remainder = pixelX % TileLength;
            if (remainder >= (TileLength / 2))
            {
                pixelX += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                pixelX -= remainder;
            }

            remainder = pixelY % TileLength;
            if (remainder >= (TileLength / 2))
            {
                pixelY += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                pixelY -= remainder;
            }

            // convert to Units (tiles)
            int x = pixelX / TileLength;
            int y = pixelY / TileLength;

            return new Point(x, y);
        }

        public static void SnapToGrid(int pixelX, int pixelY, int pixelWidth, int pixelHeight, out Point location, out Size size)
        {
            int remainder;
            remainder = pixelX % TileLength;
            if (remainder >= (TileLength / 2))
            {
                pixelX += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                pixelX -= remainder;
            }

            remainder = pixelWidth % TileLength;
            if (remainder >= (TileLength / 2))
            {
                pixelWidth += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                pixelWidth -= remainder;
            }

            remainder = pixelY % TileLength;
            if (remainder >= (TileLength / 2))
            {
                pixelY += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                pixelY -= remainder;
            }

            remainder = pixelHeight % TileLength;
            if (remainder >= (TileLength / 2))
            {
                pixelHeight += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                pixelHeight -= remainder;
            }

            // convert to Units (tiles)
            int x = pixelX / TileLength;
            int y = pixelY / TileLength;
            int width = pixelWidth / TileLength;
            int height = pixelHeight / TileLength;

            location = new Point(x, y);
            size = new Size(width, height);
        }

        public static void AddSector(double hScrollPercent, double vScrollPercent, Size displaySize)
        {
            Point mapPoint = GetMapPoint(hScrollPercent, vScrollPercent, displaySize);
            int x = mapPoint.X;
            int y = mapPoint.Y;

            int nextSectorId = AllSectors.Count + 1;
            int width = 3;
            int height = 3;
            Sector toAdd = new Sector(nextSectorId, x, y, width, height);
            Rectangle areaToAdd = toAdd.PixelArea;

            foreach (Sector sector in AllSectors.Values)
            {
                bool intersects = sector.PixelArea.IntersectsWith(areaToAdd);
                if (intersects == false)
                {
                    continue;
                }

                return;
            }

            // Get parent Zone
            foreach(Zone zone in AllZones.Values)
            {
                bool intersects = zone.PixelArea.IntersectsWith(areaToAdd);
                if(intersects == false)
                {
                    continue;
                }

                zone.AddSector(toAdd);
                break;
            }

            Program.OverlayHasChanged();
        }

        public static void DeleteSector()
        {
            if (SelectedRegion == null)
            {
                return;
            }

            Sector selectedSector = (Sector)SelectedRegion;
            selectedSector.Delete();

            SelectedRegion = null;
        }

        public static void AddZone(double vScrollPercent, Size displayPixelSize)
        {
            int mapPixelWidth = MapSize.Width * TileLength;
            int mapPixelHeight = MapSize.Height * TileLength;

            int scrollBarLength = 25;
            int displayPixelHeight = displayPixelSize.Height;

            int viewPixelHeight = displayPixelHeight - scrollBarLength;

            int maxPixelY = mapPixelHeight - viewPixelHeight;

            int pixelX = 0;
            int pixelY = (int)Math.Round(vScrollPercent * maxPixelY + viewPixelHeight / 2, 0);

            // Snap to grid
            int remainder;

            remainder = pixelY % TileLength;
            if (remainder >= (TileLength / 2))
            {
                pixelY += TileLength - remainder;
            }
            if (remainder < (TileLength / 2))
            {
                pixelY -= remainder;
            }

            int nextZoneId = AllZones.Count + 1;

            int x = pixelX / TileLength;
            int y = pixelY / TileLength;
            int width = MapSize.Width;
            int height = 3;
            Zone zoneToAdd = new Zone(nextZoneId, x, y, width, height);
            Rectangle areaToAdd = zoneToAdd.PixelArea;

            foreach (Zone zone in AllZones.Values)
            {
                bool intersects = zone.PixelArea.IntersectsWith(areaToAdd);
                if (intersects == false)
                {
                    continue;
                }

                return;
            }

            AllZones.Add(nextZoneId, zoneToAdd);

            int nextSectorId = AllSectors.Count + 1;

            Sector sectorToAdd = new Sector(nextSectorId, x, y, width, height);
            zoneToAdd.AddSector(sectorToAdd);

            Program.OverlayHasChanged();
        }

        public static void DeleteZone()
        {
            if (SelectedRegion == null)
            {
                return;
            }

            Zone selectedZone = (Zone)SelectedRegion;
            selectedZone.Delete();

            SelectedRegion = null;

            Program.OverlayHasChanged();
        }

        public static void AddCheckPoint(double vScrollPercent, Size displaySize)
        {
            Point mapPixelPoint = GetMapPoint(0, vScrollPercent, displaySize);
            int y = mapPixelPoint.Y;

            bool alreadyExists = AllCheckpoints.ContainsKey(y);
            if (alreadyExists == true)
            {
                return;
            }

            int mapWidth = MapSize.Width;
            Checkpoint checkpoint = new Checkpoint(y, mapWidth);
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

        public static void DeleteCheckpoint()
        {
            if (SelectedRegion == null)
            {
                return;
            }

            Checkpoint selectedCheckpoint = (Checkpoint)SelectedRegion;
            selectedCheckpoint.Delete();

            SelectedRegion = null;
        }

        public static void AddSpawnpoint(Spawnpoint spawnpoint)
        {
            AllSpawnpoints.Add(spawnpoint);
            OverlayHasChanged();
        }

        public static void AddSpawnpoint(double hScrollPercent, double vScrollPercent, Size displaySize)
        {
            Point mapPoint = GetMapPoint(hScrollPercent, vScrollPercent, displaySize);

            Sector selectedSector = null;
            foreach (Sector sector in AllSectors.Values)
            {
                bool contains = sector.PixelArea.Contains(mapPoint);
                if (contains == false)
                {
                    continue;
                }

                selectedSector = sector;
                break;
            }

            if (selectedSector == null)
            {
                MessageBox.Show(string.Format("No sector selected"));
                return;
            }

            try
            {
                selectedSector.AddSpawnpointInCenter();
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(string.Format("{0}", exception.Message));
            }
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