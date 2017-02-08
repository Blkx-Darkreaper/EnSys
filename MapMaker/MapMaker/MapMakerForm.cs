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
        protected Bitmap mapImage = null;
        protected bool isDrawingOnMap { get; set; }
        protected Point previousCursor { get; set; }
        protected Tool selectedTool { get; set; }
        protected Overlay selectedOverlay { get; set; }
        protected Timer timer { get; set; }
        protected string version = "1.5.10";

        public enum Tool
        {
            Pen, Line, Rectangle, Fill
        }

        public MapMakerForm()
        {
            InitializeComponent();

            Program.MapScale = 1;
            this.isDrawingOnMap = false;
            this.selectedTool = Tool.Pen;
            this.selectedOverlay = Overlay.None;

            int tilesetDisplayWidth = TilesetDisplay.Width;

            int tileLength = Program.TileLength;
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
            base.OnPaint(e);

            bool displayChanged = Program.HasDisplayChanged;
            if (displayChanged == false)
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
            bool addPadding = (bool)GridToggle.Checked;
            double scale = Program.MapScale;

            Bitmap image = (Bitmap)MapDisplay.Image;
            if (image == null)
            {
                image = Program.GetMapImage(scale);
                mapImage = GetImageCopy(ref image);
            }
            else
            {
                image = GetUpdatedMapImage(ref image);
            }

            // Mini map
            if (MiniMapDisplay.Image == null)
            {
                MiniMapDisplay.SetImage(ref image, MapPanel);
            }
            else
            {
                MiniMapDisplay.UpdateImage(ref image);
            }

            // Line and Rectangle tools
            DrawToolPreview(ref image);

            // Overlay
            UpdateOverlay(ref image);

            // Main display
            MapDisplay.Size = image.Size;
            MapDisplay.Image = image;

            Program.DisplayIsUpToDate();
        }

        private void DrawToolPreview(ref Bitmap image)
        {
            if (isDrawingOnMap == false)
            {
                return;
            }

            if (selectedTool != Tool.Line)
            {
                if (selectedTool != Tool.Rectangle)
                {
                    return;
                }
            }

            Point start = previousCursor;
            Point end = MapDisplay.PointToClient(Cursor.Position);

            double scale = Program.MapScale;
            List<Grid> selectedGrids = Program.GetGridsInArea(start, end, scale);

            Pen pen = new Pen(Brushes.Red);
            pen.Width = 2;
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

            switch (selectedTool)
            {
                case Tool.Line:
                    // Draw tile preview
                    //foreach (Grid grid in selectedGrids)
                    //{
                    //    int scaledX = (int)Math.Round(grid.Corner.X * MapScale, 0);
                    //    int scaledY = (int)Math.Round(grid.Corner.Y * MapScale, 0);
                    //    int scaledLength = (int)Math.Round(TileLength * MapScale, 0);

                    //    Rectangle bounds = new Rectangle(scaledX, scaledY, scaledLength, scaledLength);
                    //    bool intersects = Program.LineIntersectsRect(start, end, bounds);
                    //    if (intersects == false)
                    //    {
                    //        continue;
                    //    }

                    //    Program.DrawTileOntoImage(ref image, Program.SelectedTile, TileLength, scaledX, scaledY, MapScale);
                    //}

                    // Draw line
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.DrawLine(pen, start, end);
                    }
                    break;

                case Tool.Rectangle:
                    // Draw tile preview
                    //foreach (Grid grid in selectedGrids)
                    //{
                    //    int scaledX = (int)Math.Round(grid.Corner.X * MapScale, 0);
                    //    int scaledY = (int)Math.Round(grid.Corner.Y * MapScale, 0);
                    //    Program.DrawTileOntoImage(ref image, Program.SelectedTile, TileLength, scaledX, scaledY, MapScale);
                    //}

                    // Draw rectangle
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        int x = Math.Min(start.X, end.X);
                        int y = Math.Min(start.Y, end.Y);
                        int width = Math.Abs(start.X - end.X);
                        int height = Math.Abs(start.Y - end.Y);

                        Rectangle bounds = new Rectangle(x, y, width, height);
                        g.DrawRectangle(pen, bounds);
                    }
                    break;

                default:
                    return;
            }
        }

        protected void UpdateOverlay(ref Bitmap image)
        {
            double scale = Program.MapScale;

            switch (selectedOverlay)
            {
                // Sectors/Zones
                case Overlay.Sectors:
                    Program.DrawSectors(ref image, scale);
                    break;

                case Overlay.Zones:
                    Program.DrawZones(ref image, scale);
                    break;

                // Construction
                case Overlay.Construction:
                    Program.DrawConstructibility(ref image, scale);
                    break;

                // Ground passability
                case Overlay.Drivable:
                    Program.DrawDrivability(ref image, scale);
                    break;

                // Air passability
                case Overlay.Flyable:
                    Program.DrawFlyability(ref image, scale);
                    break;

                case Overlay.None:
                default:
                    break;
            }

            Program.OverlayIsUpToDate();
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

            double scale = Program.MapScale;
            image = Program.DrawMap(ref image, scale);
            this.mapImage = GetImageCopy(ref image);

            return image;
        }

        public void UpdateTileset()
        {
            //bool addPadding = (bool)GridToggle.Checked;
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
                case Tool.Fill:
                    Program.FillTool(grid, selectedOverlay);
                    break;

                case Tool.Rectangle:
                    break;

                case Tool.Line:
                    break;

                case Tool.Pen:
                default:
                    Program.PenTool(grid, selectedOverlay);
                    break;
            }
        }

        protected void MapDisplay_DoubleClick(object sender, EventArgs e)
        {
            if (selectedOverlay != Overlay.Sectors)
            {
                return;
            }

            int clicks = 2;
            Point cursor = MapDisplay.PointToClient(Cursor.Position);
            int x = cursor.X;
            int y = cursor.Y;
            int delta = 0;
            MouseEventArgs mouseEvent = new MouseEventArgs(MouseButtons.None, clicks, x, y, delta);

            double scale = Program.MapScale;

            Program.HandleSectorOverlayDoubleClick(mouseEvent, scale);
        }

        protected void MapDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.isDrawingOnMap = true;
            Program.AddDrawState();
            SetRedoEnabled();

            switch (selectedTool)
            {
                case Tool.Line:
                case Tool.Rectangle:
                    Program.DisplayHasChanged();
                    previousCursor = MapDisplay.PointToClient(Cursor.Position);
                    break;

                case Tool.Pen:
                case Tool.Fill:
                default:
                    Grid grid = GetGridAtCursor();
                    double scale = Program.MapScale;

                    switch (selectedOverlay)
                    {
                        case Overlay.Zones:
                            bool editZones = !LockZones.Checked;
                            Program.HandleZoneOverlayMouseDown(e, editZones, scale);
                            break;

                        case Overlay.Sectors:
                            Program.HandleSectorOverlayMouseDown(e, scale);
                            break;

                        case Overlay.Construction:
                            Program.SetSelectedValue(!grid.AllowsConstruction);
                            break;

                        case Overlay.Drivable:
                            Program.SetSelectedValue(!grid.AllowsDriving);
                            break;

                        case Overlay.Flyable:
                            Program.SetSelectedValue(!grid.AllowsFlying);
                            break;

                        case Overlay.None:
                        default:
                            break;
                    }
                    break;
            }
        }

        protected void MapDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            this.isDrawingOnMap = false;
            double scale = Program.MapScale;

            switch (selectedOverlay)
            {
                case Overlay.Sectors:
                    Program.HandleSectorOverlayMouseUp(e, scale);
                    break;

                case Overlay.Zones:
                    bool editZones = !LockZones.Checked;
                    Program.HandleZoneOverlayMouseUp(e, editZones, scale);
                    break;
            }

            if (e.Button == MouseButtons.Right)
            {
                CancelDrawing();
                return;
            }

            Point cursor = MapDisplay.PointToClient(Cursor.Position);

            switch (selectedTool)
            {
                case Tool.Line:
                    Program.LineTool(previousCursor, cursor, selectedOverlay, scale);
                    break;

                case Tool.Rectangle:
                    Program.RectangleTool(previousCursor, cursor, selectedOverlay, scale);
                    break;
            }

            if (Program.HasDisplayChanged == false)
            {
                return;
            }

            //Program.EndDrawState();

            undoToolStripMenuItem.Enabled = true;

            //UpdateMap();
        }

        protected void MapDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            double scale = Program.MapScale;
            switch (selectedOverlay)
            {
                case Overlay.Zones:
                bool editZones = !LockZones.Checked;
                Program.HandleZoneOverlayMouseMove(e, editZones, scale);
                return;

                case Overlay.Sectors:
                Program.HandleSectorOverlayMouseMove(e, scale);
                return;
            }

            if (isDrawingOnMap == false)
            {
                return;
            }

            switch (selectedTool)
            {
                case Tool.Line:
                case Tool.Rectangle:
                    Program.MapHasChanged();
                    break;

                case (int)Tool.Pen:
                    Grid grid = GetGridAtCursor();
                    Program.PenTool(grid, selectedOverlay);
                    break;
            }
        }

        protected void MapDisplay_MouseEnter(object sender, EventArgs e)
        {
            if (selectedOverlay != Overlay.Sectors)
            {
                return;
            }

            int clicks = 0;
            Point cursor = MapDisplay.PointToClient(Cursor.Position);
            int x = cursor.X;
            int y = cursor.Y;
            int delta = 0;
            MouseEventArgs mouseEvent = new MouseEventArgs(MouseButtons.None, clicks, x, y, delta);

            bool editSectors = selectedOverlay == Overlay.Sectors;
            double scale = Program.MapScale;

            switch (selectedOverlay)
            {
                case Overlay.Zones:
                    bool editZones = !LockZones.Checked;
                    Program.HandleZoneOverlayMouseEnter(mouseEvent, editZones, scale);
                    break;

                case Overlay.Sectors:
                    Program.HandleSectorOverlayMouseEnter(mouseEvent, scale);
                    break;
            }
        }

        protected void MapDisplay_MouseLeave(object sender, EventArgs e)
        {
            if (selectedOverlay != Overlay.Sectors)
            {
                return;
            }

            int clicks = 0;
            Point cursor = MapDisplay.PointToClient(Cursor.Position);
            int x = cursor.X;
            int y = cursor.Y;
            int delta = 0;
            MouseEventArgs mouseEvent = new MouseEventArgs(MouseButtons.None, clicks, x, y, delta);

            bool editSectors = selectedOverlay == Overlay.Sectors;
            double scale = Program.MapScale;

            switch (selectedOverlay)
            {
                case Overlay.Zones:
                    bool editZones = !LockZones.Checked;
                    Program.HandleZoneOverlayMouseLeave(mouseEvent, editZones, scale);
                    break;

                case Overlay.Sectors:
                    Program.HandleSectorOverlayMouseLeave(mouseEvent, scale);
                    break;
            }
        }

        protected Grid GetGridAtCursor()
        {
            Point cursor = MapDisplay.PointToClient(Cursor.Position);
            int x = cursor.X;
            int y = cursor.Y;

            double scale = Program.MapScale;
            Grid grid = Program.GetMapGrid(x, y, scale);
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

        private static int GetLetterKeyNumber(Keys keyCode)
        {
            bool isLetter = char.IsLetter((char)keyCode);
            if (isLetter == false)
            {
                return -1;
            }

            char letter = (char)keyCode;
            int number = Program.CharToNumber(letter);
            return number;
        }

        protected void CancelDrawing()
        {
            Program.CancelDrawing();

            SetUndoEnabled();
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
            int tileLength = newFileForm.TileLength;
            int tilesetDisplayWidth = TilesetDisplay.Width;
            Init(tileLength, tilesetDisplayWidth);

            Program.LoadTileset(filename, tileLength, tilesetDisplayWidth);

            Size mapSize = newFileForm.MapSize;
            MapDisplay.Size = mapSize;
            Program.BuildMap(mapSize);

            SetToolsEnabled(true);
            SetMapOptionsEnabled(true);
            ResetView();
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
            // Get the Filename of the selected file
            OpenFileDialog dialog = Program.GetMapOpenDialog();
            string filename = Program.GetFilenameToOpen(dialog);
            if (filename == null)
            {
                return;
            }

            Init();
            Program.LoadMapFile(filename);

            SetScalingEnabled(true);
            SetToolsEnabled(true);
            SetMapOptionsEnabled(true);
            ResetView();
            UpdateDisplay();
            timer.Start();
        }

        protected void Init()
        {
            int tileLength = Program.TileLength;
            int tilesetDisplayWidth = TilesetDisplay.Width;
            Init(tileLength, tilesetDisplayWidth);
        }

        protected void Init(int tileLength, int tilesetDisplayWidth)
        {
            MapDisplay.Image = null;
            Program.Init(tileLength, tilesetDisplayWidth);
        }

        protected void ResetView()
        {
            MapPanel.HorizontalScroll.Value = 0;
            MapPanel.VerticalScroll.Value = 0;
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

        protected void MapMakerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool unsavedChanges = Program.HasUnsavedChanges;
            if (unsavedChanges == true)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save?",
                       "Strikeforce Map Maker",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Information);

                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    Program.UpdateMapFile();
                }
            }
        }

        protected void displayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayForm displayForm = Program.GetDisplayOptions(this.Size);
            if (displayForm == null)
            {
                return;
            }

            this.WindowState = FormWindowState.Normal;

            Size resolution = displayForm.Resolution;
            int width = resolution.Width;
            int height = resolution.Height;

            this.Size = new Size(width, height);
            this.CenterToScreen();
            ResizeComponents(width, height);
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
            Program.InvalidateMap();
            UpdateMap();
        }

        protected void mapPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapPropertiesForm mapPropertiesForm = Program.GetMapProperties();
            if (mapPropertiesForm == null)
            {
                return;
            }

            Program.Author = mapPropertiesForm.Author;

            string filename = mapPropertiesForm.TilesetFilename;
            int tileLength = mapPropertiesForm.TileLength;
            int tilesetDisplayWidth = TilesetDisplay.Width;

            Program.LoadTileset(filename, tileLength, tilesetDisplayWidth);

            Size mapSize = mapPropertiesForm.MapSize;
            MapDisplay.Size = mapSize;
            MapDisplay.Image = new Bitmap(MapDisplay.Image, mapSize);

            Program.ResizeMap(mapSize.Width, mapSize.Height);
            Program.InvalidateMap();

            UpdateDisplay();
        }

        protected void ScaleNone_Click(object sender, EventArgs e)
        {
            if (Program.MapScale == 1)
            {
                return;
            }

            Program.MapScale = 1;
            Program.InvalidateMap();
            UpdateMap();
        }

        protected void ScaleHalf_Click(object sender, EventArgs e)
        {
            if (Program.MapScale == 0.5)
            {
                return;
            }

            Program.MapScale = 0.5;
            Program.InvalidateMap();
            UpdateMap();
        }

        protected void ScaleQuarter_Click(object sender, EventArgs e)
        {
            if (Program.MapScale == 0.25)
            {
                return;
            }

            Program.MapScale = 0.25;
            Program.InvalidateMap();
            UpdateMap();
        }

        protected void PenTool_Click(object sender, EventArgs e)
        {
            this.selectedTool = Tool.Pen;
        }

        protected void LineTool_Click(object sender, EventArgs e)
        {
            this.selectedTool = Tool.Line;
        }

        protected void RectangleTool_Click(object sender, EventArgs e)
        {
            this.selectedTool = Tool.Rectangle;
        }

        protected void FillTool_Click(object sender, EventArgs e)
        {
            this.selectedTool = Tool.Fill;
        }

        protected void MapPanel_Scroll(object sender, ScrollEventArgs e)
        {
            MiniMapDisplay.Scroll(sender, e);
        }

        protected void noOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = Overlay.None;
            UpdateMap();

            SetZoneControlsEnabled(false);
        }

        protected void sectorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = Overlay.Sectors;
            UpdateMap();

            SetSectorControlsEnabled(true);
        }

        protected void zonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = Overlay.Zones;
            UpdateMap();

            SetZoneControlsEnabled(true);
        }

        protected void constructionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = Overlay.Construction;
            UpdateMap();

            SetZoneControlsEnabled(false);
        }

        protected void drivableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = Overlay.Drivable;
            UpdateMap();

            SetZoneControlsEnabled(false);
        }

        protected void flyableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = Overlay.Flyable;
            UpdateMap();

            SetZoneControlsEnabled(false);
        }

        protected void SetZoneControlsEnabled(bool enabled)
        {
            SetToolsEnabled(!enabled);

            AddCheckpointButton.Enabled = enabled;
            AddButton.Enabled = enabled;
            addCheckpointToolStripMenuItem.Enabled = enabled;
            LockZones.Enabled = enabled;
        }

        protected void SetSectorControlsEnabled(bool enabled)
        {
            SetToolsEnabled(!enabled);

            AddButton.Enabled = enabled;
            addSectorToolStripMenuItem.Enabled = enabled;
            addSpawnpointToolStripMenuItem.Enabled = enabled;
        }

        protected void SetToolsEnabled(bool enabled)
        {
            PenTool.Enabled = enabled;
            LineTool.Enabled = enabled;
            RectangleTool.Enabled = enabled;
            FillTool.Enabled = enabled;
        }

        protected void SetScalingEnabled(bool enabled)
        {
            ZoomX1.Enabled = enabled;
            ZoomX2.Enabled = enabled;
            ZoomX4.Enabled = enabled;
        }

        protected void SetMapOptionsEnabled(bool enabled)
        {
            saveToolStripMenuItem.Enabled = enabled;
            saveAsToolStripMenuItem.Enabled = enabled;
            SaveFileButton.Enabled = enabled;

            noOverlayToolStripMenuItem.Enabled = enabled;
            zonesToolStripMenuItem.Enabled = enabled;
            sectorsToolStripMenuItem.Enabled = enabled;
            constructionToolStripMenuItem.Enabled = enabled;
            drivableToolStripMenuItem.Enabled = enabled;
            flyableToolStripMenuItem.Enabled = enabled;

            mapPropertiesToolStripMenuItem.Enabled = enabled;
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            AddSectorOrCheckpoint();
        }

        protected void AddSectorOrCheckpoint()
        {
            int vScroll = MapPanel.VerticalScroll.Value;
            int vScrollMax = 1 + MapPanel.VerticalScroll.Maximum - MapPanel.VerticalScroll.LargeChange;
            double vScrollPercent = vScroll / (double)vScrollMax;

            switch (selectedOverlay)
            {
                case Overlay.Sectors:
                    int hScroll = MapPanel.HorizontalScroll.Value;
                    int hScrollMax = 1 + MapPanel.HorizontalScroll.Maximum - MapPanel.HorizontalScroll.LargeChange;
                    double hScrollPercent = hScroll / (double)hScrollMax;

                    Program.AddSector(hScrollPercent, vScrollPercent, MapPanel.Size);
                    break;

                case Overlay.Zones:
                    Program.AddZone(vScrollPercent, MapPanel.Size);
                    break;

                default:
                    break;
            }
        }

        protected void AddCheckpoint_Click(object sender, EventArgs e)
        {
            int vScroll = MapPanel.VerticalScroll.Value;
            int vScrollMax = 1 + MapPanel.VerticalScroll.Maximum - MapPanel.VerticalScroll.LargeChange;
            double vScrollPercent = vScroll / (double)vScrollMax;

            LockZones.Checked = true;

            Program.AddCheckPoint(vScrollPercent, MapPanel.Size);
        }

        protected void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(String.Format("Strikeforce MapMaker\nVersion {0}\n2016", version), "About Strikeforce MapMaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected void GridToggle_CheckedChanged(object sender, EventArgs e)
        {
            Program.MapHasChanged();
        }

        protected void addSectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSectorOrCheckpoint();
        }

        protected void addCheckpointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSectorOrCheckpoint();
        }

        protected void addSpawnpointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int vScroll = MapPanel.VerticalScroll.Value;
            int vScrollMax = 1 + MapPanel.VerticalScroll.Maximum - MapPanel.VerticalScroll.LargeChange;
            double vScrollPercent = vScroll / (double)vScrollMax;

            int hScroll = MapPanel.HorizontalScroll.Value;
            int hScrollMax = 1 + MapPanel.HorizontalScroll.Maximum - MapPanel.HorizontalScroll.LargeChange;
            double hScrollPercent = hScroll / (double)hScrollMax;

            Program.AddSpawnpoint(hScrollPercent, vScrollPercent, MapPanel.Size);
        }
    }
}