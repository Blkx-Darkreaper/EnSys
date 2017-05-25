using System;
using System.Drawing;
using System.Windows.Forms;

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
            this.Location = new Point(x, y);
            this.Size = new Size(width, height);
            this.isEmpty = false;
        }

        public override void SetBorders()
        {
            int length = Math.Min(this.PixelWidth, this.PixelHeight);
            if (length <= Program.TileLength)
            {
                base.SetBordersRelative(this.PixelArea);
            }
            else
            {
                base.SetBordersAbsolute(this.PixelArea);
            }
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int x = this.Location.X;
            int y = this.Location.Y;
            KeepInBounds(PixelArea, x, y);
        }

        public void AddGrid(Grid grid)
        {
            int gridNwX = grid.Location.X;
            int gridNwY = grid.Location.Y;

            if (isEmpty == true)
            {
                this.Location = new Point(gridNwX, gridNwY);
                this.PixelWidth = Program.TileLength;
                this.PixelHeight = Program.TileLength;
                this.isEmpty = false;
                return;
            }

            int nwX = this.Location.X;
            int updatedX = nwX;
            if (nwX > gridNwX)
            {
                updatedX = gridNwX;
            }

            int nwY = this.Location.Y;
            int updatedY = nwY;
            if (nwY > gridNwY)
            {
                updatedY = gridNwY;
            }

            this.Location = new Point(updatedX, updatedY);

            int gridSeX = gridNwX + 1;
            int gridSeY = gridNwY + 1;

            int seX = updatedX + this.Size.Width;
            int seY = updatedY + this.Size.Height;

            int deltaWidth = (gridSeX - seX) * Program.TileLength; // convert to pixels
            int deltaHeight = (gridSeY - seY) * Program.TileLength;    // convert to pixels

            this.PixelWidth += deltaWidth;
            this.PixelHeight += deltaHeight;
        }
    }
}
