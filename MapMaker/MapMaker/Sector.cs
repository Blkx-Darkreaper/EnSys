using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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

        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int x = this.Corner.X;
            int y = this.Corner.Y;
            KeepInBounds(Area, x, y);
        }

        protected virtual void UpdateSpawnPosition(Rectangle updatedArea, int deltaX, int deltaY)
        {
            if (Spawn == null)
            {
                return;
            }

            Spawn.Move(deltaX, deltaY);

            Spawn.KeepInBounds(updatedArea, Spawn.Corner.X, Spawn.Corner.Y);
        }

        public void AddSpawnpointInCenter()
        {
            if (Spawn != null)
            {
                throw new InvalidOperationException(AlreadyHasSpawnErrorMessage);
            }

            int x = this.Corner.X;
            int y = this.Corner.Y;

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