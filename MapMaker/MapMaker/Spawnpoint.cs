using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MapMaker
{
    public class Spawnpoint : Region
    {
        [JsonIgnore]
        public Sector ParentSector { get; protected set; }
        [JsonIgnore]
        protected Grid childGrid { get; set; }
        public bool IsHeadquartersSpawn
        {
            get { return headquartersSpawn == this; }
            set
            {
                if (value == true)
                {
                    headquartersSpawn = this;
                    return;
                }

                if (headquartersSpawn != this)
                {
                    return;
                }

                headquartersSpawn = null;
            }
        }
        protected static Spawnpoint headquartersSpawn { get; set; }
        [JsonIgnore]
        public string AlreadyHasParentErrorMessage { get; protected set; }
        public bool IsLocked { get; set; }

        public Spawnpoint(int x, int y, Sector parentSector, bool isHQSpawn)
            : base(x, y, 1, 1)
        {
            this.IsHeadquartersSpawn = isHQSpawn;
            this.IsLocked = false;

            SetParentSector(parentSector);
            UpdateChildGrid();
        }

        public void SetParentSector(Sector parent)
        {
            this.ParentSector = parent;
            if (parent == null)
            {
                throw new InvalidOperationException(string.Format("Parent sector is null"));
            }

            this.AlreadyHasParentErrorMessage = string.Format("Spawnpoint already has parent sector {0}", parent.Id);

            parent.AddSpawnpoint(this);
            Program.AddSpawnpoint(this);
        }

        public void UpdateChildGrid()
        {
            if (childGrid != null)
            {
                childGrid.IsHeadquartersSpawn = false;
                childGrid.IsSectorSpawn = false;
            }

            int startPixelX = Location.X * Program.TileLength;   // convert to pixels
            int startPixelY = Location.Y * Program.TileLength;    // convert to pixels
            Point start = new Point(startPixelX, startPixelY);

            int width = PixelArea.Width;
            int height = PixelArea.Height;

            int endPixelX = startPixelX + width;
            int endPixelY = startPixelY + height;

            Point end = new Point(endPixelX, endPixelY);

            List<Grid> gridsToUpdate = Program.GetGridsInArea(start, end);
            if (gridsToUpdate.Count == 0)
            {
                throw new InvalidOperationException(string.Format("No grids found at spawnpoint location {0}, {1}", Location.X, Location.Y));
            }

            childGrid = gridsToUpdate[0];

            childGrid.IsSectorSpawn = !IsHeadquartersSpawn;
            childGrid.IsHeadquartersSpawn = IsHeadquartersSpawn;
        }

        public override void Delete()
        {
            childGrid.IsSectorSpawn = false;
            childGrid.IsHeadquartersSpawn = false;
            childGrid = null;

            Program.AllSpawnpoints.Remove(this);
        }

        public override void Draw(Graphics graphics, double scale)
        {
            int tileLength = Program.TileLength;

            int pixelX = (int)Math.Round(this.Location.X * tileLength * scale, 0);
            int pixelY = (int)Math.Round(this.Location.Y * tileLength * scale, 0);

            Pen lightPen = new Pen(Program.SelectionColour, 1);
            Pen heavyPen = new Pen(Program.SelectionColour, 2);
            Pen pen = lightPen;
            if (IsSelected == true)
            {
                pen = heavyPen;
            }

            graphics.DrawRectangle(pen, pixelX, pixelY, tileLength, tileLength);

            pen = lightPen;

            if (IsHeadquartersSpawn == true)
            {
                Point startPixel = new Point(pixelX, pixelY);
                Point endPixel = new Point(pixelX + tileLength, pixelY + tileLength);
                graphics.DrawLine(pen, startPixel, endPixel);

                startPixel = new Point(pixelX + tileLength, pixelY);
                endPixel = new Point(pixelX, pixelY + tileLength);
                graphics.DrawLine(pen, startPixel, endPixel);
            }
            else
            {
                graphics.DrawEllipse(pen, pixelX, pixelY, tileLength, tileLength);
            }
        }

        public override void OnMouseEnter(MouseEventArgs e)
        {
            this.Cursor = Cursors.NoMove2D;
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        public virtual void OnDoubleClick(MouseEventArgs e)
        {
            this.IsHeadquartersSpawn = !this.IsHeadquartersSpawn;
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            Point cursor = e.Location;

            this.currentMouseBorder = 0;
            this.HasMouseFocus = true;
            this.previousCursor = cursor;

            SetPreviousArea();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (currentMouseBorder != 0)
            {
                return;
            }

            Point cursor = e.Location;

            int deltaPixelY = cursor.Y - previousCursor.Y;
            int deltaPixelX = cursor.X - previousCursor.X;

            int tileLength = Program.TileLength;
            if (Math.Abs(deltaPixelX) < tileLength)
            {
                if (Math.Abs(deltaPixelY) < tileLength)
                {
                    return;
                }
            }

            Move(deltaPixelX, deltaPixelY);

            int x = this.Location.X;
            int y = this.Location.Y;

            // Keep Spawnpoint within bounds of parent Sector
            KeepInBounds(ParentSector.PixelArea, x, y);

            this.previousCursor = cursor;
        }

        public override void Move(int deltaPixelX, int deltaPixelY)
        {
            if (IsLocked == true)
            {
                return;
            }

            if (deltaPixelX == 0)
            {
                if (deltaPixelY == 0)
                {
                    return;
                }
            }

            base.Move(deltaPixelX, deltaPixelY);

            UpdateChildGrid();
        }

        public override void KeepInBounds(Rectangle pixelBounds, int x, int y)
        {
            if (IsLocked == true)
            {
                return;
            }

            base.KeepInBounds(pixelBounds, x, y);

            UpdateChildGrid();
        }
    }
}