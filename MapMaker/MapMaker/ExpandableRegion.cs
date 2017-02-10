using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MapMaker
{
    public class ExpandableRegion : Region
    {
        public int Id { get; protected set; }
        protected bool isEmpty { get; set; }

        public ExpandableRegion(int id)
            : base()
        {
            this.Id = id;
            this.isEmpty = true;
        }

        public ExpandableRegion(int id, int x, int y, int width, int height)
            : this(id)
        {
            this.Corner = new Point(x, y);
            this.Size = new Size(width, height);
            this.isEmpty = false;
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

        public void AddGrid(Grid grid, int tileLength)
        {
            int gridNwX = grid.Corner.X;
            int gridNwY = grid.Corner.Y;

            if (isEmpty == true)
            {
                this.Corner = new Point(gridNwX, gridNwY);
                this.Width = tileLength;
                this.Height = tileLength;
                this.isEmpty = false;
                return;
            }

            int nwX = this.Corner.X;
            int updatedX = nwX;
            if (nwX > gridNwX)
            {
                updatedX = gridNwX;
            }

            int nwY = this.Corner.Y;
            int updatedY = nwY;
            if (nwY > gridNwY)
            {
                updatedY = gridNwY;
            }

            this.Corner = new Point(updatedX, updatedY);

            int gridSeX = gridNwX + tileLength;
            int gridSeY = gridNwY + tileLength;

            int seX = updatedX + this.Width;
            int seY = updatedY + this.Height;

            int deltaWidth = gridSeX - seX;
            int deltaHeight = gridSeY - seY;

            this.Width += deltaWidth;
            this.Height += deltaHeight;
        }
    }
}
