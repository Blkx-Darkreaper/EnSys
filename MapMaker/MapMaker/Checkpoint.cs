using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing;
using System.Drawing.Drawing2D;
using Newtonsoft.Json;

namespace MapMaker
{
    public class Checkpoint : Region, IComparable<Checkpoint>, IEquatable<Checkpoint>
    {
        [JsonIgnore] public int Key { get; protected set; }

        public Checkpoint(int key, int width) : base()
        {
            this.Key = key;
            this.Location = new Point(0, key);
            this.Size = new Size(width, 1);
            this.IsMouseOver = false;
            this.HasMouseFocus = false;
        }

        [JsonConstructor] public Checkpoint(Point location, Size size)
            : base()
        {
            this.Key = location.Y;
            this.Location = location;
            this.Size = size;
            this.IsMouseOver = false;
            this.HasMouseFocus = false;
        }

        public int CompareTo(Checkpoint other)
        {
            int y = this.Location.Y;
            int otherY = other.Location.Y;
            return y.CompareTo(otherY);
        }

        public bool Equals(Checkpoint other)
        {
            int y = this.Location.Y;
            int otherY = other.Location.Y;
            return y.Equals(otherY);
        }

        public override void Draw(Graphics graphics, double scale)
        {
            Brush brush = new SolidBrush(Program.SelectionColour);

            int pixelY = (int)Math.Round((this.Location.Y * Program.TileLength + this.PixelHeight / 2) * scale, 0);
            Point start = new Point(2, pixelY);
            int width = this.PixelWidth - 2;
            Point end = new Point(width - 2, pixelY);

            Pen pen = new Pen(Program.SelectionColour, 1);
            if (IsSelected == true)
            {
                pen = new Pen(Program.SelectionColour, 2);
            }

            graphics.DrawLine(pen, start, end);

            Size nodePixelSize = new Size(8, 8);

            int pixelX = -nodePixelSize.Width / 2;
            pixelY = start.Y - nodePixelSize.Height / 2;
            Point startNode = new Point(pixelX, pixelY);
            Rectangle startBounds = new Rectangle(startNode, nodePixelSize);
            graphics.FillEllipse(brush, startBounds);

            pixelX = end.X + 2 - nodePixelSize.Width / 2;
            Point endNode = new Point(pixelX, pixelY);
            Rectangle endBounds = new Rectangle(endNode, nodePixelSize);
            graphics.FillEllipse(brush, endBounds);
        }

        public override void Delete()
        {
            
        }

        public override void SetBorders()
        {
            base.SetBordersRelative(this.PixelArea);
        }

        public override void OnMouseEnter(MouseEventArgs e)
        {
            this.Cursor = Cursors.HSplit;
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
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

            int deltaY = cursor.Y - previousCursor.Y;
            int y = this.Location.Y + deltaY;
            this.Location = new Point(0, y);

            this.previousCursor = cursor;
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            Program.MoveCheckpoint(this);
            this.Key = this.Location.Y;
        }

        protected override void SnapToGrid()
        {
            int x = this.Location.X;
            int y = this.Location.Y;
            int height = this.PixelHeight;
            int remainder = y % height;
            if (remainder >= (height / 2))
            {
                y += height - remainder;
            }
            if (remainder < (height / 2))
            {
                y -= remainder;
            }

            this.Location = new Point(x, y);
        }
    }
}
