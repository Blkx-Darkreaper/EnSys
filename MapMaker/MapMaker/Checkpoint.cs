using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing;

namespace MapMaker
{
    public class Checkpoint : Control
    {
        protected bool isDragging { get; set; }
        protected Point previousCursor { get; set; }
        protected int width { get; set; }
        protected int y { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;

            Pen pen = new Pen(new SolidBrush(Color.Blue));
            Point start = new Point(0, y);
            Point end = new Point(width, y);

            graphics.DrawLine(pen, start, end);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Point cursor = this.PointToClient(Cursor.Position);
            Rectangle bounds = this.GetBounds();

            bool cursorInsideBounds = bounds.Contains(cursor);
            if (cursorInsideBounds == false)
            {
                return;
            }

            Cursor.Current = Cursors.NoMove2D;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Point cursor = this.PointToClient(Cursor.Position);
            Rectangle bounds = this.GetBounds();

            bool cursorInsideBounds = bounds.Contains(cursor);
            if (cursorInsideBounds == true)
            {
                return;
            }

            Cursor.Current = Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Point cursor = this.PointToClient(Cursor.Position);

            Rectangle bounds = GetBounds();

            bool cursorInsideViewBox = bounds.Contains(cursor);
            if (cursorInsideViewBox == false)
            {
                return;
            }

            this.isDragging = true;
            this.previousCursor = cursor;
        }

        protected virtual Rectangle GetBounds()
        {
            Rectangle bounds = new Rectangle(0, y, width, 10);
            return bounds;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging == false)
            {
                return;
            }

            Point cursor = this.PointToClient(Cursor.Position);

            int deltaY = cursor.Y - previousCursor.Y;
            int sign = Program.GetSign(deltaY);

            this.y += sign;

            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.isDragging = false;
        }
    }
}
