﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapMaker
{
    public partial class MapMakerForm : Form
    {
        public double MapScale { get; protected set; }
        protected Bitmap mapImage = null;
        protected bool isDrawingOnMap { get; set; }
        protected int selectedTool { get; set; }
        protected int selectedOverlay { get; set; }
        protected Timer timer { get; set; }

        public enum Resolutions
        {
            SixFortyByFourEighty, EightHundredBySixHundred, TenTwentyFourBySevenSixtyEight
        }

        public enum Tools
        {
            Pen, Line, Rectangle, Fill
        }

        public MapMakerForm()
        {
            InitializeComponent();

            this.MapScale = 1;
            this.isDrawingOnMap = false;
            this.selectedTool = (int)Tools.Pen;
            this.selectedOverlay = (int)Program.Overlays.None;

            int tileLength = (int)TileLengthControl.Value;
            int tilesetDisplayWidth = TilesetDisplay.Width;

            Program.Init(tileLength, tilesetDisplayWidth);

            timer = new Timer();
            timer.Tick += new EventHandler(RefreshMap);
            timer.Interval = 200;
        }

        protected void RefreshMap(object sender, EventArgs e)
        {
            MapDisplay.Refresh();
        }

        protected void MapDisplay_Paint(object sender, PaintEventArgs e)
        {
            bool mapChanged = Program.HasMapChanged;
            if (mapChanged == false)
            {
                return;
            }

            UpdateMap();
        }

        public void UpdateDisplay()
        {
            UpdateTileset();

            UpdateMap();
        }

        public void UpdateMap()
        {
            Bitmap image = (Bitmap)MapDisplay.Image;
            if (image == null)
            {
                image = Program.GetMap(MapScale);
                mapImage = GetImageCopy(ref image);
            }
            else
            {
                image = GetUpdatedMapImage(ref image);
            }

            // Mini map
            MiniMapDisplay.SetImage(ref image, MapPanel);

            // Overlay
            UpdateOverlay(ref image);

            // Main display
            MapDisplay.Size = image.Size;
            MapDisplay.Image = image;

            Program.SetMapIsUpToDate();
        }

        protected void UpdateOverlay(ref Bitmap image)
        {
            switch (selectedOverlay)
            {
                // Sectors/Zones
                case (int)Program.Overlays.Sectors:
                    Program.DrawSectors(ref image, MapScale);
                    break;

                // Construction
                case (int)Program.Overlays.Construction:
                    Program.DrawConstructibility(ref image, MapScale);
                    break;

                // Ground passability
                case (int)Program.Overlays.Drivable:
                    Program.DrawDrivability(ref image, MapScale);
                    break;

                // Air passability
                case (int)Program.Overlays.Flyable:
                    Program.DrawFlyability(ref image, MapScale);
                    break;

                case (int)Program.Overlays.None:
                default:
                    break;
            }

            Program.SetOverlayIsUpToDate();
        }

        private Bitmap GetImageCopy(ref Bitmap original)
        {
            Bitmap copy = new Bitmap(original.Width, original.Height);
            using (Graphics graphics = Graphics.FromImage(copy))
            {
                graphics.DrawImage(original, 0, 0);
            }

            return copy;
        }

        private Bitmap GetUpdatedMapImage(ref Bitmap image)
        {
            bool mapImageOutdated = Program.HasTerrainChanged;
            if (mapImageOutdated == false)
            {
                image = GetImageCopy(ref mapImage);
                return image;
            }

            image = Program.DrawMap(ref image, MapScale);
            this.mapImage = GetImageCopy(ref image);

            return image;
        }

        public void UpdateTileset()
        {
            //bool addPadding = (bool)Padding.Checked;
            bool addPadding = false;

            Bitmap image = Program.DrawTileset(addPadding);
            TilesetDisplay.Height = image.Height;
            TilesetDisplay.Image = image;
        }

        protected void MapDisplay_Click(object sender, EventArgs e)
        {
            Grid grid = GetGridAtCursor();

            switch (selectedTool)
            {
                case (int)Tools.Fill:
                    Program.FillTool(grid, selectedOverlay);
                    break;

                case (int)Tools.Rectangle:
                    break;

                case (int)Tools.Line:
                    break;

                case (int)Tools.Pen:
                default:
                    Program.PenTool(grid, selectedOverlay);
                    break;
            }
        }

        protected void MapDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            this.isDrawingOnMap = true;
            Program.AddDrawState();
            SetRedoEnabled();

            Grid grid = GetGridAtCursor();
            
            switch (selectedOverlay)
            {
                case (int)Program.Overlays.Sectors:

                    break;

                case (int)Program.Overlays.Construction:
                    Program.SetSelectedValue(!grid.AllowsConstruction);
                    break;

                case (int)Program.Overlays.Drivable:
                    Program.SetSelectedValue(!grid.AllowsDriving);
                    break;

                case (int)Program.Overlays.Flyable:
                    Program.SetSelectedValue(!grid.AllowsFlying);
                    break;

                case (int)Program.Overlays.None:
                default:
                    break;
            }
        }

        protected void MapDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            this.isDrawingOnMap = false;

            if (Program.HasMapChanged == false)
            {
                return;
            }

            //Program.EndDrawState();

            undoToolStripMenuItem.Enabled = true;

            //UpdateMap();
        }

        protected void MapDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawingOnMap == false)
            {
                return;
            }

            Grid grid = GetGridAtCursor();
            Program.PenTool(grid, selectedOverlay);
        }

        private Grid GetGridAtCursor()
        {
            Point cursor = MapDisplay.PointToClient(Cursor.Position);
            int x = cursor.X;
            int y = cursor.Y;

            Grid grid = Program.GetMapGrid(x, y, MapScale);
            return grid;
        }

        protected void TilesetDisplay_Click(object sender, EventArgs e)
        {
            Point cursor = TilesetDisplay.PointToClient(Cursor.Position);
            Program.SelectTile(cursor);

            UpdateTileset();
        }

        protected void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        protected void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        protected void MapMakerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers != Keys.Control)
            {
                return;
            }

            if (e.KeyCode == Keys.Z)
            {
                Undo();
            }

            if (e.KeyCode == Keys.R)
            {
                Redo();
            }
        }

        protected void Undo()
        {
            Program.Undo();
            //UpdateMap();

            redoToolStripMenuItem.Enabled = true;

            SetUndoEnabled();
        }

        private void SetUndoEnabled()
        {
            bool canUndoMore = Program.CheckCanUndo();
            undoToolStripMenuItem.Enabled = canUndoMore;
        }

        protected void Redo()
        {
            Program.Redo();
            //UpdateMap();

            undoToolStripMenuItem.Enabled = true;

            SetRedoEnabled();
        }

        private void SetRedoEnabled()
        {
            bool canRedoMore = Program.CheckCanRedo();
            redoToolStripMenuItem.Enabled = canRedoMore;
        }

        protected void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        protected void NewFile_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        protected void NewFile()
        {
            NewFileForm newFileForm = Program.CreateNewFile();
            if (newFileForm == null)
            {
                return;
            }

            string filename = newFileForm.TilesetFilename;
            int tileWidth = (int)TileLengthControl.Value;
            int tilesetDisplayWidth = TilesetDisplay.Width;
            Program.LoadTileset(filename, tileWidth, tilesetDisplayWidth);

            Size mapSize = newFileForm.MapSize;
            MapDisplay.Size = mapSize;
            Program.BuildMap(mapSize);

            UpdateDisplay();
            timer.Start();
        }

        protected void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFile();
        }

        protected void LoadFileButton_Click(object sender, EventArgs e)
        {
            LoadFile();
        }

        protected void LoadFile()
        {
            Program.LoadMapFile();
            UpdateDisplay();
            timer.Start();
        }

        protected void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.UpdateMapFile();
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            Program.UpdateMapFile();
        }

        protected void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.SaveMap();
        }

        protected void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected void displayToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public void SetWindowSize(int resolutionCode)
        {
            int[] windowWidths = { 640, 800, 1024 };
            int[] windowHeights = { 480, 600, 768 };

            int windowWidth = windowWidths[resolutionCode];
            int windowHeight = windowHeights[resolutionCode];
            MapMakerForm.ActiveForm.Size = new Size(windowWidth, windowHeight);

            ResizeComponents(windowWidth, windowHeight);
        }

        protected void ResizeComponents(int windowWidth, int windowHeight)
        {
            int x, y, width, height;

            // Menu panel
            x = 12;
            y = 27;
            width = (int)Math.Ceiling(windowWidth * 0.9375);
            height = (int)Math.Round(windowHeight * 0.07292, 0);
            MenuPanel.Location = new Point(x, y);
            MenuPanel.Size = new Size(width, height);

            // Mini map
            x = 12;
            y = 33 + MenuPanel.Height;
            width = (int)Math.Ceiling(windowWidth * 0.15625);
            height = (int)Math.Ceiling(windowHeight * 0.20833);
            MiniMapDisplay.Location = new Point(x, y);
            MiniMapDisplay.Size = new Size(width, height);

            // tileset panel
            x = 12;
            y = 6 + MiniMapDisplay.Height + MiniMapDisplay.Location.Y;
            width = (int)Math.Ceiling(windowWidth * 0.15625);
            height = (int)Math.Ceiling(windowHeight * 0.53125);
            TilesetPanel.Location = new Point(x, y);
            TilesetPanel.Size = new Size(width, height);

            // Map panel
            x = 18 + TilesetPanel.Width;
            y = 33 + MenuPanel.Height;
            width = (int)Math.Round(windowWidth * 0.77188, 0);
            height = (int)Math.Ceiling(windowHeight * 0.75208);
            MapPanel.Location = new Point(x, y);
            MapPanel.Size = new Size(width, height);
        }

        protected void MapMaker_Resize(object sender, EventArgs e)
        {
            int width = MapMakerForm.ActiveForm.Width;
            int height = MapMakerForm.ActiveForm.Height;

            ResizeComponents(width, height);
        }

        protected void x480ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int code = (int)Resolutions.SixFortyByFourEighty;
            SetWindowSize(code);
        }

        protected void x600ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int code = (int)Resolutions.EightHundredBySixHundred;
            SetWindowSize(code);
        }

        protected void x768ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int code = (int)Resolutions.TenTwentyFourBySevenSixtyEight;
            SetWindowSize(code);
        }

        protected void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the Filename of the selected file
            OpenFileDialog dialog = Program.GetImageOpenDialog();
            string filename = Program.GetFilenameToOpen(dialog);
            if (filename == null)
            {
                return;
            }

            Program.LoadTilesetImageFromFile(filename);

            UpdateDisplay();
        }

        protected void ScaleNone_Click(object sender, EventArgs e)
        {
            this.MapScale = 1;
            Program.InvalidateMap();
            UpdateMap();
        }

        protected void ScaleHalf_Click(object sender, EventArgs e)
        {
            this.MapScale = 1/2.0;
            Program.InvalidateMap();
            UpdateMap();
        }

        protected void ScaleQuarter_Click(object sender, EventArgs e)
        {
            this.MapScale = 1/4.0;
            Program.InvalidateMap();
            UpdateMap();
        }

        protected void TileWidth_ValueChanged(object sender, EventArgs e)
        {
            int value = (int)TileLengthControl.Value;
            int tileLength = Program.TileLength;

            int remainder = value % 8;
            if (remainder == 0)
            {
                TileLengthControl.Value = value;
                tileLength = (int)TileLengthControl.Value;
                Program.TileLength = tileLength;
                return;
            }

            if (value > tileLength)
            {
                TileLengthControl.Value = tileLength * 2;
            }
            else
            {
                TileLengthControl.Value = tileLength / 2;
            }

            tileLength = (int)TileLengthControl.Value;
            Program.TileLength = tileLength;
        }

        protected void PenTool_Click(object sender, EventArgs e)
        {
            this.selectedTool = (int)Tools.Pen;
        }

        protected void LineTool_Click(object sender, EventArgs e)
        {
            this.selectedTool = (int)Tools.Line;
        }

        protected void RectangleTool_Click(object sender, EventArgs e)
        {
            this.selectedTool = (int)Tools.Rectangle;
        }

        protected void FillTool_Click(object sender, EventArgs e)
        {
            this.selectedTool = (int)Tools.Fill;
        }

        protected void MapPanel_Scroll(object sender, ScrollEventArgs e)
        {
            MiniMapDisplay.Scroll(sender, e);
        }

        protected void noOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = (int)Program.Overlays.None;
            UpdateMap();
        }

        protected void sectorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = (int)Program.Overlays.Sectors;
            UpdateMap();
        }

        protected void constructionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = (int)Program.Overlays.Construction;
            UpdateMap();
        }

        protected void drivableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = (int)Program.Overlays.Drivable;
            UpdateMap();
        }

        protected void flyableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = (int)Program.Overlays.Flyable;
            UpdateMap();
        }

        protected void AddCheckpointButton_Click(object sender, EventArgs e)
        {

        }
    }
}