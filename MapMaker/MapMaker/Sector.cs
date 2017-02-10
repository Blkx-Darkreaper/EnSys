using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MapMaker
{
    public class Sector : ExpandableRegion
    {
        public Zone Parent { get; set; }
        public Spawnpoint Spawn { get; protected set; }
        public string AlreadyHasSpawnErrorMessage { get; protected set; }

        public Sector(int id)
            : base(id)
        {
            this.AlreadyHasSpawnErrorMessage = string.Format("Sector {0} already contains a spawnpoint", id);
        }

        public Sector(int id, int x, int y, int width, int height) : base(id, x, y, width, height)
        {
            this.AlreadyHasSpawnErrorMessage = string.Format("Sector {0} already contains a spawnpoint", id);

            AddSpawnpointInCenter();
        }

        public override void Draw(Graphics graphics, double scale, Color colour)
        {
            Pen pen = new Pen(colour, 4);
            if (IsMouseOver == true || IsSelected == true)
            {
                pen = new Pen(colour, 6);
            }

            pen.Alignment = PenAlignment.Inset;

            Rectangle bounds = GetScaledBounds(scale);

            graphics.DrawRectangle(pen, bounds);

            Font font = new Font(FontFamily.GenericSansSerif, 12f);

            int scaledX = (int)Math.Round(Location.X * scale, 0);
            int scaledY = (int)Math.Round(Location.Y * scale, 0);
            Point scaledLocation = new Point(scaledX, scaledY);
            graphics.DrawString(Id.ToString(), font, Brushes.White, scaledLocation);
        }

        public override void SetBorders()
        {
            int length = Math.Min(this.Width, this.Height);
            if (length <= Program.TileLength)
            {
                base.SetBordersRelative(this.Area);
            }
            else
            {
                base.SetBordersAbsolute(this.Area);
            }
        }

        protected override Rectangle UpdateArea(Point location, Size size)
        {
            Rectangle updatedArea = base.UpdateArea(location, size);

            UpdateSpawnPosition(this.Area, 0, 0);

            return updatedArea;
        }

        public override void Move(int deltaX, int deltaY)
        {
            base.Move(deltaX, deltaY);

            UpdateSpawnPosition(this.Area, deltaX, deltaY);
        }

        public override void Delete()
        {
            this.Spawn.Delete();

            Program.AllSectors.Remove(this.Id);
            Parent.AllSectors.Remove(this);

            Program.UpdateGridSectors(this.Area, 0);
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int x = this.Location.X;
            int y = this.Location.Y;
            KeepInBounds(Area, x, y);
        }

        protected virtual void UpdateSpawnPosition(Rectangle updatedArea, int deltaX, int deltaY)
        {
            if (Spawn == null)
            {
                return;
            }

            Spawn.Move(deltaX, deltaY);
            Spawn.KeepInBounds(updatedArea, Spawn.Location.X, Spawn.Location.Y);
        }

        public void AddSpawnpointInCenter()
        {
            if (Spawn != null)
            {
                throw new InvalidOperationException(AlreadyHasSpawnErrorMessage);
            }

            int x = this.Location.X;
            int y = this.Location.Y;

            int width = this.Area.Width;
            int height = this.Area.Height;

            int tileLength = Program.TileLength;

            int tilesWide = width / tileLength;
            int tilesHigh = height / tileLength;

            int midX = x + tileLength * (tilesWide / 2);
            int midY = y + tileLength * (tilesHigh / 2);

            Point centerTileLocation = Program.SnapToGrid(midX, midY);

            Spawnpoint spawnpoint = new Spawnpoint(centerTileLocation.X, centerTileLocation.Y, this, false);
        }

        public void AddSpawnpoint(Spawnpoint toAdd)
        {
            if (this.Spawn != null)
            {
                throw new InvalidOperationException(AlreadyHasSpawnErrorMessage);
            }

            // Check spawnpoint has correct parent
            if (toAdd.ParentSector != this)
            {
                throw new InvalidOperationException(toAdd.AlreadyHasParentErrorMessage);
            }

            this.Spawn = toAdd;
        }
    }
}