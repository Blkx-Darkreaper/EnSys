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
        [JsonIgnore] public Sector ParentSector { get; protected set; }
        [JsonIgnore] protected int parentSectorId;
        public int ParentSectorId { get { if (ParentSector != null) { return ParentSector.SectorId; } else { return parentSectorId; } } protected set { parentSectorId = value; } }
        protected bool isHeadquartersSpawn;
        public bool IsHeadquartersSpawn
        {
            get { return isHeadquartersSpawn; }
            protected set
            {
                if (value == false)
                {
                    this.isHeadquartersSpawn = false;
                    return;
                }

                if (headquartersSpawn != null)
                {
                    headquartersSpawn.IsHeadquartersSpawn = false;
                }

                this.isHeadquartersSpawn = value;
                if (value == false)
                {
                    return;
                }

                headquartersSpawn = this;
            }
        }
        protected static Spawnpoint headquartersSpawn { get; set; }
        protected bool isRaiderSpawn;
        public bool IsRaiderSpawn
        {
            get { return isRaiderSpawn; }
            protected set
            {
                if (value == false)
                {
                    this.isRaiderSpawn = false;
                    return;
                }

                if (raiderSpawn != null)
                {
                    raiderSpawn.IsRaiderSpawn = false;
                }

                this.isRaiderSpawn = value;
                if (value == false)
                {
                    return;
                }

                raiderSpawn = this;
            }
        }
        protected static Spawnpoint raiderSpawn { get; set; }

        [JsonConstructor] public Spawnpoint(Point location, Size size, int parentSectorId, bool isHeadquartersLocation)
            : this(location, size, isHeadquartersLocation)
        {
            this.ParentSectorId = parentSectorId;
        }

        public Spawnpoint(Point location) : this(location, new Size(Program.TileLength, Program.TileLength), false) { }

        public Spawnpoint(Point location, Size size, bool isHeadquartersLocation)
            : base(location, size.Width, size.Height)
        {
            this.IsHeadquartersSpawn = isHeadquartersLocation;
        }

        public void SetParentSector(Sector parent)
        {
            ParentSector = parent;
            CheckLinkage();
        }

        protected void CheckLinkage()
        {
            if (ParentSector == null)
            {
                return;
            }

            if (ParentSector.Spawnpoint == this)
            {
                return;
            }

            // If parent already has a toAdd, break linkage
            this.ParentSector = null;
        }

        public void Draw(Graphics graphics, double scale)
        {
            Brush brush = new SolidBrush(Program.SelectionColour);

            int x = (int)Math.Round(this.Location.X * scale, 0);
            int y = (int)Math.Round(this.Location.Y * scale, 0);

            int tileLength = Program.TileLength;

            Pen pen = new Pen(brush);
            graphics.DrawRectangle(pen, x, y, tileLength, tileLength);

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
    }
}