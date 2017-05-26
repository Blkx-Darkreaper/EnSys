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
        public Sector Parent { get; protected set; }
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

        public Spawnpoint(int x, int y, Sector parentSector, bool isHQSpawn)
            : base(x, y, 1, 1)
        {
            this.IsHeadquartersSpawn = isHQSpawn;

            SetParentSector(parentSector);
        }

        [JsonConstructor]
        public Spawnpoint(int x, int y, bool isHQSpawn) : base(x, y, 1, 1)
        {
            this.IsHeadquartersSpawn = isHQSpawn;
        }

        public void SetParentSector(Sector parent)
        {
            this.Parent = parent;
            if (parent == null)
            {
                throw new InvalidOperationException(string.Format("Parent sector is null"));
            }

            this.AlreadyHasParentErrorMessage = string.Format("Spawnpoint already has parent sector {0}", parent.Id);

            parent.AddSpawnpoint(this);
            Program.AddSpawnpoint(this);
        }

        public override void Delete()
        {
            Parent.RemoveSpawnpoint();

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
            KeepInBounds(Parent.PixelArea, x, y);

            this.previousCursor = cursor;
        }
    }
}