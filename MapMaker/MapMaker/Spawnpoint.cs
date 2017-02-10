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
            : base(x, y, Program.TileLength, Program.TileLength)
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

            Point start = Location;
            int startX = start.X;
            int startY = start.Y;
            int width = Area.Width;
            int height = Area.Height;

            int endX = startX + width;
            int endY = startY + height;

            Point end = new Point(endX, endY);

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
            int x = (int)Math.Round(this.Location.X * scale, 0);
            int y = (int)Math.Round(this.Location.Y * scale, 0);

            int tileLength = Program.TileLength;

            Pen lightPen = new Pen(Program.SelectionColour, 1);
            Pen heavyPen = new Pen(Program.SelectionColour, 2);
            Pen pen = lightPen;
            if (IsSelected == true)
            {
                pen = heavyPen;
            }

            graphics.DrawRectangle(pen, x, y, tileLength, tileLength);

            pen = lightPen;

            if (IsHeadquartersSpawn == true)
            {
                Point start = new Point(x, y);
                Point end = new Point(x + tileLength, y + tileLength);
                graphics.DrawLine(pen, start, end);

                start = new Point(x + tileLength, y);
                end = new Point(x, y + tileLength);
                graphics.DrawLine(pen, start, end);
            }
            else
            {
                graphics.DrawEllipse(pen, x, y, tileLength, tileLength);
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

            int deltaX = cursor.X - previousCursor.X;
            int x = this.Location.X + deltaX;

            int deltaY = cursor.Y - previousCursor.Y;
            int y = this.Location.Y + deltaY;

            // Keep Spawnpoint within bounds of parent Sector
            KeepInBounds(ParentSector.Area, x, y);

            this.previousCursor = cursor;
        }

        public override void Move(int deltaX, int deltaY)
        {
            if (IsLocked == true)
            {
                return;
            }

            base.Move(deltaX, deltaY);

            UpdateChildGrid();
        }

        public override void KeepInBounds(Rectangle bounds, int x, int y)
        {
            if (IsLocked == true)
            {
                return;
            }

            base.KeepInBounds(bounds, x, y);

            UpdateChildGrid();
        }
    }
}