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
    public class Checkpoint : Control, IComparable<Checkpoint>, IEquatable<Checkpoint>
    {
        public int Key { get; protected set; }
        protected bool isDragging { get; set; }
        protected Point previousCursor { get; set; }

        [JsonConstructor]
        public Checkpoint(int y, int width, int height)
            : base()
        {
            this.Size = new Size(width, height);
            this.Location = new Point(0, y);
            this.Key = y;
            this.isDragging = false;
            this.Cursor = Cursors.HSplit;
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.SetStyle(ControlStyles.Opaque, true);
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            //this.BackColor = Color.Transparent;
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        const int WS_EX_TRANSPARENT = 0x20;
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= WS_EX_TRANSPARENT;
        //        return cp;
        //    }
        //}

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

        protected virtual Rectangle GetBounds()
        {
            int width = this.Width;
            int height = this.Height;
            int y = this.Location.Y;

            Rectangle bounds = new Rectangle(0, y, width, height);
            return bounds;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            //Rectangle bounds = GetBounds();
            //graphics.DrawRectangle(Pens.Red, bounds);

            Draw(graphics);
        }

        //protected override void OnPaintBackground(PaintEventArgs pevent)
        //{
        //    // Don't call the base subroutine
        //    //base.OnPaintBackground(pevent);
        //}

        public void Draw(Graphics graphics)
        {
            Brush brush = new SolidBrush(Program.SelectionColour);

            int y = this.Location.Y + this.Height / 2;
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

        //protected override void OnMouseEnter(EventArgs e)
        //{
        //    base.OnMouseEnter(e);

        //    Point cursor = this.PointToClient(Cursor.Position);
        //    //Rectangle bounds = this.GetBounds();

        //    //bool cursorInsideBounds = bounds.Contains(cursor);
        //    //if (cursorInsideBounds == false)
        //    //{
        //    //    return;
        //    //}

        //    Cursor.Current = Cursors.NoMove2D;
        //}

        //protected override void OnMouseLeave(EventArgs e)
        //{
        //    base.OnMouseLeave(e);

        //    Point cursor = this.PointToClient(Cursor.Position);
        //    //Rectangle bounds = this.GetBounds();

        //    //bool cursorInsideBounds = bounds.Contains(cursor);
        //    //if (cursorInsideBounds == true)
        //    //{
        //    //    return;
        //    //}

        //    Cursor.Current = Cursors.Default;
        //}

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Point cursor = this.PointToClient(Cursor.Position);

            this.isDragging = true;
            this.previousCursor = cursor;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isDragging == false)
            {
                return;
            }

            Point cursor = this.PointToClient(Cursor.Position);

            int deltaY = cursor.Y - previousCursor.Y;
            int sign = Program.GetSign(deltaY);

            int y = this.Location.Y;
            y += sign;

            this.Location = new Point(0, y);
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.isDragging = false;

            SnapToGrid();
            Program.MoveCheckpoint(this);
            this.Key = this.Location.Y;

            this.Invalidate();
        }

        protected virtual void SnapToGrid()
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
