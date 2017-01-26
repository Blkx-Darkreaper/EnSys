using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MapMaker
{
    public class Sector : Region
    {
        public int SectorId { get; protected set; }
        public Spawnpoint Spawnpoint { get; protected set; }
        protected bool isEmpty { get; set; }

        public Sector(int sectorId, int tileLength)
            : base()
        {
            this.SectorId = sectorId;
            this.isEmpty = true;
        }

        public Sector(int sectorId, int tileLength, int x, int y, int width, int height) : this(sectorId, tileLength)
        {
            this.Location = new Point(x, y);
            this.Size = new Size(width, height);
            this.isEmpty = false;

            AddSpawnpoint();
        }

        public override void UpdateBorders()
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

        protected virtual void UpdateSpawnPosition(Rectangle updatedArea, int deltaX, int deltaY)
        {
            if (Spawnpoint == null)
            {
                return;
            }

            Spawnpoint.Move(deltaX, deltaY);

            Spawnpoint.KeepInBounds(updatedArea, Spawnpoint.Location.X, Spawnpoint.Location.Y);
        }

        public void AddGrid(Grid grid, int tileLength)
        {
            int gridNwX = grid.Corner.X;
            int gridNwY = grid.Corner.Y;

            if (isEmpty == true)
            {
                this.Location = new Point(gridNwX, gridNwY);
                this.Width = tileLength;
                this.Height = tileLength;
                this.isEmpty = false;
                return;
            }

            int sectorNwX = this.Location.X;
            int updatedSectorX = sectorNwX;
            if (sectorNwX > gridNwX)
            {
                updatedSectorX = gridNwX;
            }

            int sectorNwY = this.Location.Y;
            int updatedSectorY = sectorNwY;
            if (sectorNwY > gridNwY)
            {
                updatedSectorY = gridNwY;
            }

            this.Location = new Point(updatedSectorX, updatedSectorY);

            int gridSeX = gridNwX + tileLength;
            int gridSeY = gridNwY + tileLength;

            int sectorSeX = updatedSectorX + this.Width;
            int sectorSeY = updatedSectorY + this.Height;

            int deltaWidth = gridSeX - sectorSeX;
            int deltaHeight = gridSeY - sectorSeY;

            this.Width += deltaWidth;
            this.Height += deltaHeight;
        }

        public void AddSpawnpoint()
        {
            if (Spawnpoint != null)
            {
                throw new InvalidOperationException(string.Format("Sector {0} already contains a spawnpoint", SectorId));
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

            Spawnpoint spawnpoint = new Spawnpoint(centerTileLocation);
            AddSpawnpoint(spawnpoint);
        }

        public void AddSpawnpoint(Spawnpoint toAdd)
        {
            this.Spawnpoint = toAdd;
            toAdd.SetParentSector(this);
            Program.AddSpawnpoint(toAdd);
        }
    }
}