using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing;
using System.ComponentModel;

namespace MapMaker
{
    public class MiniMap : PictureBox
    {
        public Rectangle ViewBox { get; set; }
        public double scale { get; set; }
        protected Bitmap miniMap { get; set; }
        protected Rectangle miniMapBounds { get; set; }
        protected bool isDragging { get; set; }
        protected Point previousCursor { get; set; }
        protected Panel mapPanel { get; set; }

        public MiniMap()
            : base()
        {
            miniMap = null;
            isDragging = false;
        }

        public virtual void SetImage(ref Bitmap image, Panel mapPanel)
        {
            this.mapPanel = mapPanel;

            double width = image.Width;
            double height = image.Height;

            int miniMapHeight = this.Height;
            this.scale = miniMapHeight / height;

            int viewWidth = (int)(mapPanel.Width * scale);
            int viewHeight = (int)(mapPanel.Height * scale);

            int miniMapWidth = (int)(width * scale);

            // Center mini map
            int x = this.Width / 2;
            x -= miniMapWidth / 2;

            this.ViewBox = new Rectangle(x, 0, viewWidth, viewHeight);
            this.miniMapBounds = new Rectangle(x, 0, miniMapWidth, miniMapHeight);

            Bitmap miniMapImage = new Bitmap(this.Width, this.Height);
            Graphics graphics = Graphics.FromImage(miniMapImage);
            graphics.DrawImage(image, x, 0, miniMapWidth, miniMapHeight);

            this.Image = miniMapImage;
            this.miniMap = miniMapImage;
        }

        public virtual void Scroll(object sender, ScrollEventArgs e)
        {
            int scrollAmount = e.NewValue - e.OldValue;
            if (scrollAmount == 0)
            {
                return;
            }

            Panel mapPanel = (Panel)sender;
            MoveViewBox(mapPanel);
        }

        private void MoveViewBox()
        {
            MoveViewBox(mapPanel);
        }

        private void MoveViewBox(Panel panel)
        {
            int hScroll = panel.HorizontalScroll.Value;
            int vScroll = panel.VerticalScroll.Value;

            int hScrollMax = 1 + panel.HorizontalScroll.Maximum - panel.HorizontalScroll.LargeChange;
            int vScrollMax = 1 + panel.VerticalScroll.Maximum - panel.VerticalScroll.LargeChange;

            double hScrollPercent = hScroll / (double)hScrollMax;
            double vScrollPercent = vScroll / (double)vScrollMax;

            int maxX = miniMapBounds.Width - ViewBox.Width;
            int maxY = miniMapBounds.Height - ViewBox.Height;

            int offsetX = miniMapBounds.X;

            int x = offsetX + (int)(hScrollPercent * maxX);
            int y = (int)(vScrollPercent * maxY);
            this.ViewBox = new Rectangle(x, y, ViewBox.Width, ViewBox.Height);

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;

            // Minimap border
            Pen pen = new Pen(new SolidBrush(Color.DarkGray));
            graphics.DrawRectangle(pen, new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            if (this.miniMap == null)
            {
                return;
            }

            // ViewBox box
            pen = new Pen(new SolidBrush(Color.Blue));
            Rectangle bounds = ViewBox;
            bounds.Width -= 1;
            bounds.Height -= 1;

            graphics.DrawRectangle(pen, bounds);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.miniMap == null)
            {
                return;
            }

            Point cursor = this.PointToClient(Cursor.Position);

            bool cursorInsideViewBox = ViewBox.Contains(cursor);
            if (cursorInsideViewBox == false)
            {
                return;
            }

            this.isDragging = true;
            this.previousCursor = cursor;
        }

        private void OnHoverOverViewbox()
        {
            if (this.miniMap == null)
            {
                return;
            }

            Point cursor = this.PointToClient(Cursor.Position);

            bool cursorInsideViewBox = ViewBox.Contains(cursor);
            if (cursorInsideViewBox == true)
            {
                Cursor.Current = Cursors.NoMove2D;
            }
            else
            {
                Cursor.Current = Cursors.Default;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            OnHoverOverViewbox();

            if (isDragging == false)
            {
                return;
            }

            Point cursor = this.PointToClient(Cursor.Position);

            int deltaX = cursor.X - previousCursor.X;
            int signX = Program.GetSign(deltaX);
            //deltaX = (int)Math.Log10(Math.Abs(deltaX));
            deltaX = Program.ClampToMin(Math.Abs(deltaX), 1);
            deltaX = signX * deltaX;

            int deltaY = cursor.Y - previousCursor.Y;
            int signY = Program.GetSign(deltaY);
            //deltaY = (int)Math.Log10(Math.Abs(deltaY));
            deltaY = Program.ClampToMin(Math.Abs(deltaY), 1);
            deltaY = signY * deltaY;

            int hScroll = mapPanel.HorizontalScroll.Value;
            int hScrollMax = mapPanel.HorizontalScroll.Maximum;

            int vScroll = mapPanel.VerticalScroll.Value;
            int vScrollMax = mapPanel.VerticalScroll.Maximum;

            int x = hScroll + deltaX;
            int y = vScroll + deltaY;

            x = Program.Clamp(x, 0, hScrollMax);
            y = Program.Clamp(y, 0, vScrollMax);

            mapPanel.HorizontalScroll.Value = x;
            mapPanel.VerticalScroll.Value = y;

            MoveViewBox();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.isDragging = false;
        }
    }
}
