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
        public int Key { get; protected set; }

        public Checkpoint(int key, int width, int height) : base(height)
        {
            this.Key = key;
            this.Location = new Point(0, key);
            this.Size = new Size(width, height);
            this.IsMouseOver = false;
            this.HasMouseFocus = false;
        }

        [JsonConstructor]
        public Checkpoint(int key, Rectangle area, Point location, Size size, int width, int height, string cursor, bool isMouseOver, bool hasMouseFocus)
            : base(height)
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

        public void Draw(Graphics graphics, double scale)
        {
            Brush brush = new SolidBrush(Program.SelectionColour);

            int y = (int)Math.Round((this.Location.Y + this.Height / 2) * scale, 0);
            Point start = new Point(2, y);
            int width = this.Width - 2;
            Point end = new Point(width - 2, y);

            Pen pen = new Pen(brush);
            graphics.DrawLine(pen, start, end);

            Size nodeSize = new Size(8, 8);

            int x = -nodeSize.Width / 2;
            y = start.Y - nodeSize.Height / 2;
            Point startNode = new Point(x, y);
            Rectangle startBounds = new Rectangle(startNode, nodeSize);
            graphics.FillEllipse(brush, startBounds);

            x = end.X + 2 - nodeSize.Width / 2;
            Point endNode = new Point(x, y);
            Rectangle endBounds = new Rectangle(endNode, nodeSize);
            graphics.FillEllipse(brush, endBounds);
        }

        protected override void SetBorders()
        {
            base.SetBordersRelative(this.Area);
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
            int height = this.Height;
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
