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
        public bool IsHeadquartersSpawn
        {
            get { return headquartersSpawn == this; }
            protected set
            {
                if (value == false)
                {
                    return;
                }

                headquartersSpawn = this;
            }
        }
        protected static Spawnpoint headquartersSpawn { get; set; }
        [JsonIgnore] public string AlreadyHasParentErrorMessage { get; protected set; }

        public Spawnpoint(Point corner, Sector parentSector, bool isHQSpawn) : base(corner, Program.TileLength, Program.TileLength)
        {
            this.IsHeadquartersSpawn = isHQSpawn;

            SetParentSector(parentSector);
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

        public void Draw(Graphics graphics, double scale)
        {
            Brush brush = new SolidBrush(Program.SelectionColour);

            int x = (int)Math.Round(this.Corner.X * scale, 0);
            int y = (int)Math.Round(this.Corner.Y * scale, 0);

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
            int x = this.Corner.X + deltaX;

            int deltaY = cursor.Y - previousCursor.Y;
            int y = this.Corner.Y + deltaY;

            // Keep Spawnpoint within bounds of parent Sector
            KeepInBounds(ParentSector.Area, x, y);

            this.previousCursor = cursor;
        }
    }
}