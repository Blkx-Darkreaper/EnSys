using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MapMaker
{
    public class Sector : ExpandableRegion
    {
        [JsonIgnore]
        public Zone Parent { get; protected set; }
        public Spawnpoint Spawn { get; protected set; }
        [JsonIgnore]
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

        [JsonConstructor]
        public Sector(int id, Point location, Size size, Spawnpoint spawn) : base(id, location.X, location.Y, size.Width, size.Height)
        {
            this.AlreadyHasSpawnErrorMessage = string.Format("Sector {0} already contains a spawnpoint", id);
            this.Spawn = spawn;
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

            Point initialLocation = new Point(Location.X, Location.Y);

            base.Move(deltaPixelX, deltaPixelY);

            // Keep Sector within bounds of parent Zone
            int x = this.Location.X;
            int y = this.Location.Y;
            int width = this.Size.Width;
            int height = this.Size.Height;
            KeepInBounds(Parent.PixelArea, x, y, width, height);

            int tileLength = Program.TileLength;
            deltaPixelX = (Location.X - initialLocation.X) * tileLength;
            deltaPixelY = (Location.Y - initialLocation.Y) * tileLength;

            UpdateSpawnPosition(this.PixelArea, deltaPixelX, deltaPixelY);
        }

        public void SetParentZone(Zone parent)
        {
            if (parent == null)
            {
                throw new InvalidOperationException(string.Format("Parent zone is null"));
            }

            this.Parent = parent;
        }

        public override void Delete()
        {
            this.Spawn.Delete();

            Program.AllSectors.Remove(this.Id);
            Parent.AllSectors.Remove(this);
        }

        protected virtual void UpdateSpawnPosition(Rectangle updatedPixelArea, int deltaPixelX, int deltaPixelY)
        {
            if (Spawn == null)
            {
                return;
            }

            Spawn.Move(deltaPixelX, deltaPixelY);
            Spawn.KeepInBounds(updatedPixelArea, Spawn.Location.X, Spawn.Location.Y, Spawn.Size.Width, Spawn.Size.Height);
        }

        public void AddSpawnpointInCenter()
        {
            if (Spawn != null)
            {
                throw new InvalidOperationException(AlreadyHasSpawnErrorMessage);
            }

            int x = this.Location.X;
            int y = this.Location.Y;

            int pixelWidth = this.PixelArea.Width;
            int pixelHeight = this.PixelArea.Height;

            int tileLength = Program.TileLength;

            int tilesWide = pixelWidth / tileLength;
            int tilesHigh = pixelHeight / tileLength;

            int midX = x + (tilesWide / 2);
            int midY = y + (tilesHigh / 2);

            Spawnpoint spawnpoint = new Spawnpoint(midX, midY, this, false);
        }

        public void AddSpawnpoint(Spawnpoint toAdd)
        {
            if(this.Spawn == toAdd)
            {
                return;
            }
            
            if (this.Spawn != null)
            {
                throw new InvalidOperationException(AlreadyHasSpawnErrorMessage);
            }

            // Check spawnpoint has correct parent
            if (toAdd.Parent != this)
            {
                throw new InvalidOperationException(toAdd.AlreadyHasParentErrorMessage);
            }

            this.Spawn = toAdd;
        }

        public void RemoveSpawnpoint()
        {
            this.Spawn = null;
        }
    }
}