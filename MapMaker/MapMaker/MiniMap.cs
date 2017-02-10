using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace MapMaker
{
    public class MiniMap : PictureBox
    {
        public Rectangle ViewBox { get; set; }
        protected Bitmap miniMap { get; set; }
        protected Rectangle miniMapBounds { get; set; }
        protected bool isDragging { get; set; }
        protected Point previousCursor { get; set; }
        protected Panel mapPanel { get; set; }

        public MiniMap()
            : base()
        {
            this.miniMap = null;
            this.isDragging = false;
            //this.Cursor = Cursors.Hand;
        }

        public virtual void UpdateImage(ref Bitmap image)
        {
            double imageWidth = image.Width;
            double imageHeight = image.Height;

            double miniMapScale = this.Height / imageHeight;
            int miniMapWidth = (int)(imageWidth * miniMapScale);

            int viewWidth = (int)(this.mapPanel.Width * miniMapScale);
            int viewHeight = (int)(this.mapPanel.Height * miniMapScale);

            double changeOfScale = this.ViewBox.Width / (double)viewWidth;

            if (viewWidth > miniMapWidth)
            {
                viewWidth = miniMapWidth;
            }

            // Center mini map
            int x = this.Width / 2;
            x -= miniMapWidth / 2;

            this.miniMapBounds = new Rectangle(x, 0, miniMapWidth, this.Height);

            //int viewX = this.ViewBox.X;
            //int viewY = 0;
            //if (changeOfScale != 0)
            //{
            //    viewY = (int)(this.ViewBox.Y / changeOfScale);
            //}
            //int maxY = this.Height - viewHeight;
            //if (viewY > maxY)
            //{
            //    viewY = maxY;
            //}

            //this.ViewBox = new Rectangle(deltaX, viewY, viewWidth, viewHeight);

            Bitmap miniMapImage = new Bitmap(this.Width, this.Height);
            Graphics graphics = Graphics.FromImage(miniMapImage);
            graphics.DrawImage(image, x, 0, miniMapWidth, this.Height);

            this.Image = miniMapImage;
            this.miniMap = miniMapImage;

            this.ViewBox = new Rectangle(0, 0, viewWidth, viewHeight);
            UpdateViewBoxPosition();
        }

        public virtual void SetImage(ref Bitmap image, Panel mapPanel)
        {
            this.mapPanel = mapPanel;

            UpdateImage(ref image);
        }

        public virtual void Scroll(object sender, ScrollEventArgs e)
        {
            int scrollAmount = e.NewValue - e.OldValue;
            if (scrollAmount == 0)
            {
                return;
            }

            Panel mapPanel = (Panel)sender;
            UpdateViewBoxPosition(mapPanel);
        }

        private void UpdateViewBoxPosition()
        {
            UpdateViewBoxPosition(mapPanel);
        }

        private void UpdateViewBoxPosition(Panel panel)
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

            // Minimap corner
            graphics.DrawRectangle(Pens.DarkGray, new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            if (this.miniMap == null)
            {
                return;
            }

            // ViewBox box
            Pen pen = new Pen(Program.SelectionColour);
            pen.Alignment = PenAlignment.Inset;
            graphics.DrawRectangle(pen, this.ViewBox);
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
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Default;
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
            this.Cursor = Cursors.NoMove2D;

            int deltaX = cursor.X - previousCursor.X;
            int deltaY = cursor.Y - previousCursor.Y;

            int hScrollChange = mapPanel.HorizontalScroll.SmallChange;
            int hScroll = mapPanel.HorizontalScroll.Value;
            int hScrollMax = mapPanel.HorizontalScroll.Maximum;

            int vScrollChange = mapPanel.VerticalScroll.SmallChange;
            int vScroll = mapPanel.VerticalScroll.Value;
            int vScrollMax = mapPanel.VerticalScroll.Maximum;

            int x = hScroll + deltaX * hScrollChange * 2;
            int y = vScroll + deltaY * vScrollChange * 2;

            x = Program.Clamp(x, 0, hScrollMax);
            y = Program.Clamp(y, 0, vScrollMax);

            mapPanel.HorizontalScroll.Value = x;
            mapPanel.VerticalScroll.Value = y;

            UpdateViewBoxPosition();

            this.previousCursor = cursor;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.isDragging = false;
        }
    }
}
