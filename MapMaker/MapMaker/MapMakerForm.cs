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
        protected Point previousCursor { get; set; }
        protected int selectedTool { get; set; }
        protected int selectedOverlay { get; set; }
        protected Timer timer { get; set; }

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
            Bitmap image = (Bitmap)MapDisplay.Image;
            if (image == null)
            {
                image = Program.GetMapImage(MapScale);
                mapImage = GetImageCopy(ref image);
            }
            else
            {
                image = GetUpdatedMapImage(ref image);
            }

            // Mini map
            MiniMapDisplay.SetImage(ref image, MapPanel);

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

            if (selectedTool != (int)Tools.Line)
            {
                if (selectedTool != (int)Tools.Rectangle)
                {
                    return;
                }
            }

            Point start = previousCursor;
            Point end = MapDisplay.PointToClient(Cursor.Position);
            int tileLength = (int)TileLengthControl.Value;

            List<Grid> selectedGrids = Program.GetGridsInArea(start, end, MapScale);

            Pen pen = new Pen(Brushes.Red);
            pen.Width = 2;
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

            switch (selectedTool)
            {
                case (int)Tools.Line:
                    //foreach (Grid grid in selectedGrids)
                    //{
                    //    int scaledX = (int)Math.Round(grid.Corner.X * MapScale, 0);
                    //    int scaledY = (int)Math.Round(grid.Corner.Y * MapScale, 0);
                    //    int scaledLength = (int)Math.Round(tileLength * MapScale, 0);

                    //    Rectangle bounds = new Rectangle(scaledX, scaledY, scaledLength, scaledLength);
                    //    bool intersects = Program.LineIntersectsRect(start, end, bounds);
                    //    if (intersects == false)
                    //    {
                    //        continue;
                    //    }

                    //    Program.DrawTileOntoImage(ref image, Program.SelectedTile, tileLength, scaledX, scaledY, MapScale);
                    //}

                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.DrawLine(pen, start, end);
                    }
                    break;

                case (int)Tools.Rectangle:
                    //foreach (Grid grid in selectedGrids)
                    //{
                    //    int scaledX = (int)Math.Round(grid.Corner.X * MapScale, 0);
                    //    int scaledY = (int)Math.Round(grid.Corner.Y * MapScale, 0);
                    //    Program.DrawTileOntoImage(ref image, Program.SelectedTile, tileLength, scaledX, scaledY, MapScale);
                    //}

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
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.isDrawingOnMap = true;
            Program.AddDrawState();
            SetRedoEnabled();

            switch (selectedTool)
            {
                case (int)Tools.Line:
                case (int)Tools.Rectangle:
                    Program.DisplayHasChanged();
                    previousCursor = MapDisplay.PointToClient(Cursor.Position);
                    break;

                case (int)Tools.Pen:
                case (int)Tools.Fill:
                default:
                    Grid grid = GetGridAtCursor();

                    switch (selectedOverlay)
                    {
                        case (int)Program.Overlays.Sectors:
                            bool editSectors = SectorChkptToggle.Checked;
                            Program.HandleSectorOverlayMouseDown(e, editSectors);
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
                    break;
            }
        }

        protected void MapDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            this.isDrawingOnMap = false;

            if (selectedOverlay == (int)Program.Overlays.Sectors)
            {
                bool editSectors = SectorChkptToggle.Checked;
                Program.HandleSectorOverlayMouseUp(e, editSectors, MapScale);
            }

            if (e.Button == MouseButtons.Right)
            {
                CancelDrawing();
                return;
            }

            Point cursor = MapDisplay.PointToClient(Cursor.Position);

            switch (selectedTool)
            {
                case (int)Tools.Line:
                    Program.LineTool(previousCursor, cursor, selectedOverlay, MapScale);
                    break;

                case (int)Tools.Rectangle:
                    Program.RectangleTool(previousCursor, cursor, selectedOverlay, MapScale);
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
            if (selectedOverlay == (int)Program.Overlays.Sectors)
            {
                bool editSectors = SectorChkptToggle.Checked;
                Program.HandleSectorOverlayMouseMove(e, editSectors, MapScale);
                return;
            }

            if (isDrawingOnMap == false)
            {
                return;
            }

            switch (selectedTool)
            {
                case (int)Tools.Line:
                case (int)Tools.Rectangle:
                    Program.MapHasChanged();
                    break;

                case (int)Tools.Pen:
                    Grid grid = GetGridAtCursor();
                    Program.PenTool(grid, selectedOverlay);
                    break;
            }
        }

        protected void MapDisplay_MouseEnter(object sender, EventArgs e)
        {
            if (selectedOverlay != (int)Program.Overlays.Sectors)
            {
                return;
            }

            int clicks = 0;
            Point cursor = MapDisplay.PointToClient(Cursor.Position);
            int x = (int)Math.Round(cursor.X / MapScale, 0);
            int y = (int)Math.Round(cursor.Y / MapScale, 0);
            int delta = 0;
            MouseEventArgs mouseEvent = new MouseEventArgs(MouseButtons.None, clicks, x, y, delta);

            bool editSectors = SectorChkptToggle.Checked;
            Program.HandleSectorOverlayMouseEnter(mouseEvent, editSectors, MapScale);
        }

        protected void MapDisplay_MouseLeave(object sender, EventArgs e)
        {
            if (selectedOverlay != (int)Program.Overlays.Sectors)
            {
                return;
            }

            int clicks = 0;
            Point cursor = MapDisplay.PointToClient(Cursor.Position);
            int x = cursor.X;
            int y = cursor.Y;
            int delta = 0;
            MouseEventArgs mouseEvent = new MouseEventArgs(MouseButtons.None, clicks, x, y, delta);

            bool editSectors = SectorChkptToggle.Checked;
            Program.HandleSectorOverlayMouseLeave(mouseEvent, editSectors);
        }

        protected Grid GetGridAtCursor()
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

        //public void SetSector(Keys keyCode)
        //{
        //    if (selectedOverlay != (int)Program.Overlays.Sectors)
        //    {
        //        return;
        //    }

        //    int sectorId = GetLetterKeyNumber(keyCode);
        //    if (sectorId == -1)
        //    {
        //        return;
        //    }

        //    sectorId %= 25;

        //    Program.SetSelectedGridSector(sectorId);
        //}

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
            int tileLength = (int)TileLengthControl.Value;
            int tilesetDisplayWidth = TilesetDisplay.Width;
            Init(tileLength, tilesetDisplayWidth);

            Program.LoadTileset(filename, tileLength, tilesetDisplayWidth);

            Size mapSize = newFileForm.MapSize;
            MapDisplay.Size = mapSize;
            Program.BuildMap(mapSize);

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
            int tileLength = (int)TileLengthControl.Value;
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
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you still want to quit?",
                       "Strikeforce Map Maker",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    return;
                }

                e.Cancel = true;
            }
        }

        protected void displayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayForm displayForm = Program.GetDisplayOptions();
            if (displayForm == null)
            {
                return;
            }

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
        }

        protected void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapPropertiesForm mapPropertiesForm = Program.GetMapProperties();
            if (mapPropertiesForm == null)
            {
                return;
            }

            Program.Author = mapPropertiesForm.Author;

            string filename = mapPropertiesForm.TilesetFilename;
            int tileLength = (int)TileLengthControl.Value;
            int tilesetDisplayWidth = TilesetDisplay.Width;

            Program.LoadTileset(filename, tileLength, tilesetDisplayWidth);

            Size mapSize = mapPropertiesForm.MapSize;
            MapDisplay.Size = mapSize;

            Program.ResizeMap(mapSize.Width, mapSize.Height);
            Program.InvalidateMap();

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
            this.MapScale = 1 / 2.0;
            Program.InvalidateMap();
            UpdateMap();
        }

        protected void ScaleQuarter_Click(object sender, EventArgs e)
        {
            this.MapScale = 1 / 4.0;
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

            SetSectorControlsEnabled(false);
        }

        protected void sectorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = (int)Program.Overlays.Sectors;
            UpdateMap();

            SetSectorControlsEnabled(true);
        }

        protected void constructionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = (int)Program.Overlays.Construction;
            UpdateMap();

            SetSectorControlsEnabled(false);
        }

        protected void drivableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = (int)Program.Overlays.Drivable;
            UpdateMap();

            SetSectorControlsEnabled(false);
        }

        protected void flyableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectedOverlay = (int)Program.Overlays.Flyable;
            UpdateMap();

            SetSectorControlsEnabled(false);
        }

        protected void SetSectorControlsEnabled(bool enabled)
        {
            SetToolsEnabled(!enabled);

            SectorChkptToggle.Enabled = enabled;
            AddToSectorButton.Enabled = enabled;
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
            sectorsToolStripMenuItem.Enabled = enabled;
            constructionToolStripMenuItem.Enabled = enabled;
            drivableToolStripMenuItem.Enabled = enabled;
            flyableToolStripMenuItem.Enabled = enabled;

            mapPropertiesToolStripMenuItem.Enabled = enabled;
        }

        protected void AddToSectorsButton_Click(object sender, EventArgs e)
        {
            int vScroll = MapPanel.VerticalScroll.Value;
            int vScrollMax = 1 + MapPanel.VerticalScroll.Maximum - MapPanel.VerticalScroll.LargeChange;
            double vScrollPercent = vScroll / (double)vScrollMax;

            bool addSector = SectorChkptToggle.Checked;
            if (addSector == true)
            {
                int hScroll = MapPanel.HorizontalScroll.Value;
                int hScrollMax = 1 + MapPanel.HorizontalScroll.Maximum - MapPanel.HorizontalScroll.LargeChange;
                double hScrollPercent = hScroll / (double)hScrollMax;

                Program.AddSector(hScrollPercent, vScrollPercent, MapPanel.Size, MapScale);
            }
            else
            {
                Program.AddCheckPoint(vScrollPercent, MapPanel.Size);
            }
        }
    }
}