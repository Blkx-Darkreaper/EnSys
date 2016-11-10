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
        protected bool isEmpty { get; set; }

        public Sector(int sectorId, int tileLength)
            : base(tileLength)
        {
            this.SectorId = sectorId;
            this.isEmpty = true;
        }

        public Sector(int sectorId, int tileLength, int x, int y, int width, int height) : this(sectorId, tileLength)
        {
            this.Location = new Point(x, y);
            this.Size = new Size(width, height);
            this.isEmpty = false;
        }

        protected override void SetBorders()
        {
            int length = Math.Min(this.Width, this.Height);
            if (length <= tileLength)
            {
                base.SetBordersRelative(this.Area);
            }
            else
            {
                base.SetBordersAbsolute(this.Area);
            }
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
    }
}