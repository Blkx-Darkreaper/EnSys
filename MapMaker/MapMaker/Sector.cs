using System;
using System.Drawing;
using System.Drawing.Drawing2D;

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

            int scaledPixelX = (int)Math.Round(Location.X * Program.TileLength * scale, 0);
            int scaledPixelY = (int)Math.Round(Location.Y * Program.TileLength * scale, 0);
            Point scaledLocation = new Point(scaledPixelX, scaledPixelY);
            graphics.DrawString(Id.ToString(), font, Brushes.White, scaledLocation);
        }

        protected override Rectangle UpdateArea(Point pixelLocation, Size pixelSize)
        {
            Rectangle updatedArea = base.UpdateArea(pixelLocation, pixelSize);

            UpdateSpawnPosition(this.PixelArea, 0, 0);

            return updatedArea;
        }

        public override void Move(int deltaPixelX, int deltaPixelY)
        {
            if (deltaPixelX == 0)
            {
                if (deltaPixelY == 0)
                {
                    return;
                }
            }

            base.Move(deltaPixelX, deltaPixelY);

            UpdateSpawnPosition(this.PixelArea, deltaPixelX, deltaPixelY);
        }

        public override void Delete()
        {
            this.Spawn.Delete();

            Program.AllSectors.Remove(this.Id);
            Parent.AllSectors.Remove(this);

            Program.UpdateGridSectors(this.PixelArea, 0);
        }

        protected virtual void UpdateSpawnPosition(Rectangle updatedPixelArea, int deltaPixelX, int deltaPixelY)
        {
            if (Spawn == null)
            {
                return;
            }

            Spawn.Move(deltaPixelX, deltaPixelY);
            Spawn.KeepInBounds(updatedPixelArea, Spawn.Location.X, Spawn.Location.Y);
        }

        public void AddSpawnpointInCenter()
        {
            if (Spawn != null)
            {
                throw new InvalidOperationException(AlreadyHasSpawnErrorMessage);
            }

            int x = this.Location.X;
            int y = this.Location.Y;

            int width = this.PixelArea.Width;
            int height = this.PixelArea.Height;

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