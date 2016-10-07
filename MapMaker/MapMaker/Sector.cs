using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MapMaker
{
    public class Sector
    {
        public int SectorId { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public Point TopLeftCorner { get { return allCorners[0]; } protected set { allCorners[0] = value; } }
        public Point TopRightCorner { get { return allCorners[1]; } protected set { allCorners[1] = value; } }
        public Point BottomRightCorner { get { return allCorners[2]; } protected set { allCorners[2] = value; } }
        public Point BottomLeftCorner { get { return allCorners[3]; } protected set { allCorners[3] = value; } }
        public bool IsEmpty { get; protected set; }
        protected Point[] allCorners { get; set; }

        public Sector(int sectorId)
        {
            this.SectorId = sectorId;
            this.IsEmpty = true;
            this.allCorners = new Point[4];
        }

        protected virtual void SetSize()
        {
            int width = Math.Max(TopRightCorner.X - TopLeftCorner.X, BottomRightCorner.X - BottomRightCorner.X);
            int height = Math.Max(BottomRightCorner.Y - TopRightCorner.Y, BottomLeftCorner.Y - TopLeftCorner.Y);

            this.Width = width;
            this.Height = height;
        }

        public virtual void AddGrid(Grid grid, int gridLength)
        {
            int x = grid.Corner.X;
            int y = grid.Corner.Y;
            Point topLeftCorner = new Point(x, y);
            AddTopLeftCorner(topLeftCorner);

            x += gridLength;
            Point topRightCorner = new Point(x, y);
            AddTopRightCorner(topRightCorner);

            y += gridLength;
            Point bottomRightCorner = new Point(x, y);
            AddBottomRightCorner(bottomRightCorner);

            x -= gridLength;
            Point bottomLeftCorner = new Point(x, y);
            AddBottomLeftCorner(bottomLeftCorner);

            SetSize();

            IsEmpty = false;
        }

        protected virtual void AddTopLeftCorner(Point topLeftCorner)
        {
            if (IsEmpty == true)
            {
                this.TopLeftCorner = topLeftCorner;
                return;
            }

            int currentX = TopLeftCorner.X;
            int x = topLeftCorner.X;
            if (x < currentX)
            {
                this.TopLeftCorner = topLeftCorner;
                return;
            }

            int currentY = TopLeftCorner.Y;
            int y = topLeftCorner.Y;
            if (y < currentY)
            {
                this.TopLeftCorner = topLeftCorner;
                return;
            }
        }

        protected virtual void AddTopRightCorner(Point topRightCorner)
        {
            if (IsEmpty == true)
            {
                this.TopRightCorner = topRightCorner;
                return;
            }

            int currentX = TopRightCorner.X;
            int x = topRightCorner.X;
            if (x > currentX)
            {
                this.TopRightCorner = topRightCorner;
                return;
            }

            int currentY = TopRightCorner.Y;
            int y = topRightCorner.Y;
            if (y < currentY)
            {
                this.TopRightCorner = topRightCorner;
                return;
            }
        }

        protected virtual void AddBottomRightCorner(Point bottomRightCorner)
        {
            if (IsEmpty == true)
            {
                this.BottomRightCorner = bottomRightCorner;
                return;
            }

            int currentX = BottomRightCorner.X;
            int x = bottomRightCorner.X;
            if (x > currentX)
            {
                this.BottomRightCorner = bottomRightCorner;
                return;
            }

            int currentY = BottomRightCorner.Y;
            int y = bottomRightCorner.Y;
            if (y > currentY)
            {
                this.BottomRightCorner = bottomRightCorner;
                return;
            }
        }

        protected virtual void AddBottomLeftCorner(Point bottomLeftCorner)
        {
            if (IsEmpty == true)
            {
                this.BottomLeftCorner = bottomLeftCorner;
                return;
            }

            int currentX = BottomLeftCorner.X;
            int x = bottomLeftCorner.X;
            if (x < currentX)
            {
                this.BottomLeftCorner = bottomLeftCorner;
                return;
            }

            int currentY = BottomLeftCorner.Y;
            int y = bottomLeftCorner.Y;
            if (y > currentY)
            {
                this.BottomLeftCorner = bottomLeftCorner;
                return;
            }
        }

        public virtual void RemoveGrid(Grid grid, int gridLength)
        {
            if (IsEmpty == true)
            {
                return;
            }

            int x = grid.Corner.X;
            int y = grid.Corner.Y;
            Point topLeftCorner = new Point(x, y);
            RemoveTopLeftCorner(topLeftCorner, gridLength);

            x += gridLength;
            Point topRightCorner = new Point(x, y);
            RemoveTopRightCorner(topRightCorner, gridLength);

            y += gridLength;
            Point bottomRightCorner = new Point(x, y);
            RemoveBottomRightCorner(bottomRightCorner, gridLength);

            x -= gridLength;
            Point bottomLeftCorner = new Point(x, y);
            RemoveBottomLeftCorner(bottomLeftCorner, gridLength);

            SetSize();

            if (Width != 0)
            {
                return;
            }
            if (Height != 0)
            {
                return;
            }

            IsEmpty = true;
        }

        protected virtual void RemoveTopLeftCorner(Point point, int gridLength)
        {
            int currentX = TopLeftCorner.X;
            int x = point.X;

            bool matchX = currentX == x;
            if (matchX == false)
            {
                return;
            }

            int currentY = TopLeftCorner.Y;
            int y = point.Y;

            bool matchY = currentY == y;
            if (matchY == false)
            {
                return;
            }

            int updatedX = currentX + gridLength;
            int updatedY = currentY + gridLength;

            this.TopLeftCorner = new Point(updatedX, updatedY);
        }

        protected virtual void RemoveTopRightCorner(Point point, int gridLength)
        {
            int currentX = TopRightCorner.X;
            int x = point.X;

            bool matchX = currentX == x;
            if (matchX == false)
            {
                return;
            }

            int currentY = TopRightCorner.Y;
            int y = point.Y;

            bool matchY = currentY == y;
            if (matchY == false)
            {
                return;
            }

            int updatedX = currentX - gridLength;
            int updatedY = currentY + gridLength;

            this.TopRightCorner = new Point(updatedX, updatedY);
        }

        protected virtual void RemoveBottomRightCorner(Point point, int gridLength)
        {
            int currentX = BottomRightCorner.X;
            int x = point.X;

            bool matchX = currentX == x;
            if (matchX == false)
            {
                return;
            }

            int currentY = BottomRightCorner.Y;
            int y = point.Y;

            bool matchY = currentY == y;
            if (matchY == false)
            {
                return;
            }

            int updatedX = currentX - gridLength;
            int updatedY = currentY - gridLength;

            this.BottomRightCorner = new Point(updatedX, updatedY);
        }

        protected virtual void RemoveBottomLeftCorner(Point point, int gridLength)
        {
            int currentX = BottomLeftCorner.X;
            int x = point.X;

            bool matchX = currentX == x;
            if (matchX == false)
            {
                return;
            }

            int currentY = BottomLeftCorner.Y;
            int y = point.Y;

            bool matchY = currentY == y;
            if (matchY == false)
            {
                return;
            }

            int updatedX = currentX + gridLength;
            int updatedY = currentY - gridLength;

            this.BottomLeftCorner = new Point(updatedX, updatedY);
        }

        public virtual GraphicsPath GetShape(double scale)
        {
            byte[] pathInfo = new byte[] { (byte)0, (byte)1, (byte)1, (byte)129 };

            Point[] scaledPoints = GetScaledPoints(scale);

            GraphicsPath shape = new GraphicsPath(scaledPoints, pathInfo);
            return shape;
        }

        protected virtual Point[] GetScaledPoints(double scale)
        {
            if (scale == 1)
            {
                return allCorners;
            }

            Point[] scaledPoints = new Point[4];
            for (int i = 0; i < 4; i++)
            {
                Point corner = allCorners[i];
                int scaledX = (int)Math.Round(corner.X * scale,0);
                int scaledY = (int)Math.Round(corner.Y * scale, 0);

                scaledPoints[i] = new Point(scaledX, scaledY);
            }

            return scaledPoints;
        }
    }
}
