using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MapMaker
{
    public class Region : IComparable<Region>, IEquatable<Region>
    {
        [JsonIgnore] public Rectangle Area { get; protected set; }
        protected Rectangle previousArea { get; set; }
        public Point Location { get { return Area.Location; } protected set { Area = UpdateAreaLocation(value); } }
        public Size Size { get { return Area.Size; } protected set { Area = UpdateAreaSize(value); } }
        [JsonIgnore] public int Width { get { return Area.Width; } protected set { Area = UpdateAreaWidth(value); } }
        [JsonIgnore] public int Height { get { return Area.Height; } protected set { Area = UpdateAreaHeight(value); } }
        [JsonIgnore] public Cursor Cursor { get; protected set; }
        [JsonIgnore] public bool IsMouseOver { get; protected set; }
        [JsonIgnore] public bool HasMouseFocus { get; protected set; }
        protected Rectangle northMargin { get; set; }
        protected Rectangle southMargin { get; set; }
        protected Rectangle westMargin { get; set; }
        protected Rectangle eastMargin { get; set; }
        protected enum borders { center = 0, north = 1, west = 2, east = 4, south = 8 }
        protected int currentMouseBorder { get; set; }
        protected Point previousCursor { get; set; }

        public Region()
        {
            this.Area = new Rectangle();
            this.previousArea = new Rectangle();
            this.Cursor = Cursors.Default;
            this.IsMouseOver = false;
            this.HasMouseFocus = false;
            this.currentMouseBorder = -1;

            SetBorders();
        }

        public Region(Point location, int width, int height)
            : this()
        {
            this.Location = location;
            this.Width = width;
            this.Height = height;

            SetBorders();
        }

        public virtual void SetBorders()
        {
            SetBordersAbsolute(this.Area);
        }

        protected virtual void SetBordersAbsolute(Rectangle area)
        {
            int x = area.X;
            int y = area.Y;
            int width = area.Width;
            int height = area.Height;

            int tileLength = Program.TileLength;

            northMargin = new Rectangle(x, y, width, tileLength);

            westMargin = new Rectangle(x, y, tileLength, height);

            int bottomY = y + height - tileLength;
            southMargin = new Rectangle(x, bottomY, width, tileLength);

            int rightX = x + width - tileLength;
            eastMargin = new Rectangle(rightX, y, tileLength, height);
        }

        protected virtual void SetBordersRelative(Rectangle area)
        {
            int x = area.X;
            int y = area.Y;
            int width = area.Width;
            int height = area.Height;

            int quarterHeight = height / 4;
            northMargin = new Rectangle(x, y, width, quarterHeight);

            int quarterWidth = width / 4;
            westMargin = new Rectangle(x, y, quarterWidth, height);

            int bottomY = y + 3 * quarterHeight;
            southMargin = new Rectangle(x, bottomY, width, quarterHeight);

            int rightX = x + 3 * quarterWidth;
            eastMargin = new Rectangle(rightX, y, quarterWidth, height);
        }

        public Rectangle GetScaledBounds(double scale)
        {
            int scaledX = (int)Math.Round(Area.Location.X * scale, 0);
            int scaledY = (int)Math.Round(Area.Location.Y * scale, 0);

            int scaledWidth = (int)Math.Round(Area.Width * scale, 0);
            int scaledHeight = (int)Math.Round(Area.Height * scale, 0);

            Rectangle scaledBounds = new Rectangle(scaledX, scaledY, scaledWidth, scaledHeight);
            return scaledBounds;
        }

        public int CompareTo(Region other)
        {
            int area = this.Width * this.Height;
            int otherArea = other.Width * other.Height;

            int comparison = area.CompareTo(otherArea);
            return comparison;
        }

        public bool Equals(Region other)
        {
            int x = this.Location.X;
            int otherX = other.Location.X;
            if (x != otherX)
            {
                return false;
            }

            int y = this.Location.Y;
            int otherY = other.Location.Y;
            if (y != otherY)
            {
                return false;
            }

            int width = this.Width;
            int otherWidth = other.Width;
            if (width != otherWidth)
            {
                return false;
            }

            int height = this.Height;
            int otherHeight = other.Height;
            if (height != otherHeight)
            {
                return false;
            }

            return true;
        }

        protected virtual Rectangle UpdateAreaLocation(Point location)
        {
            return UpdateArea(location, this.Size);
        }

        protected virtual Rectangle UpdateAreaSize(Size size)
        {
            return UpdateArea(this.Location, size);
        }

        protected virtual Rectangle UpdateAreaWidth(int width)
        {
            return UpdateArea(this.Location, new Size(width, this.Height));
        }

        protected virtual Rectangle UpdateAreaHeight(int height)
        {
            return UpdateArea(this.Location, new Size(this.Width, height));
        }

        protected virtual Rectangle UpdateArea(Point location, Size size)
        {
            int x = location.X;
            int y = location.Y;
            int width = size.Width;
            int height = size.Height;

            Rectangle updatedArea = new Rectangle(location, size);
            SetBordersAbsolute(updatedArea);

            return updatedArea;
        }

        public virtual Rectangle[] GetRemoved()
        {
            return GetRemoved(this.previousArea, this.Area);
        }

        protected virtual Rectangle[] GetRemoved(Rectangle previous, Rectangle current)
        {
            int deltaX = current.X - previous.X;
            int deltaY = current.Y - previous.Y;
            int deltaX2 = (current.X + current.Width) - (previous.X + previous.Width);
            int deltaY2 = (current.Y + current.Height) - (previous.Y + previous.Height);
            int deltaWidth = current.Width - previous.Width;
            int deltaHeight = current.Height - previous.Height;

            int a = Math.Min(previous.X, current.X);
            int b = Math.Max(previous.X, current.X);
            int c = Math.Min(previous.X + previous.Width, current.X + current.Width);
            int d = Math.Max(previous.X + previous.Width, current.X + current.Width);

            int e = Math.Min(previous.Y, current.Y);
            int f = Math.Max(previous.Y, current.Y);
            int g = Math.Min(previous.Y + previous.Height, current.Y + current.Height);
            int h = Math.Max(previous.Y + previous.Height, current.Y + current.Height);

            Rectangle[] result = new Rectangle[6];
            /* +---+-----+---+
               | 4 |  0  | 4 |
               +---+-----+---+
               |   |     |   |
               | 1 |     | 2 |
               |   |     |   |
               +---+-----+---+
               | 5 |  3  | 5 |
               +---+-----+---+
            */

            bool corner1 = previous.X == a && previous.Y == e;
            bool corner2 = current.X == a && current.Y == e;

            bool moveE = deltaX > 0;
            bool moveW = deltaX < 0;
            bool moveN = deltaY < 0;
            bool moveS = deltaY > 0;

            if (moveE == true)
            {
                result[1] = new Rectangle(a, f, b - a, g - f);
            }
            if (moveW == true)
            {
                result[2] = new Rectangle(c, f, d - c, g - f);
            }

            if (moveS == true)
            {
                result[0] = new Rectangle(b, e, c - b, f - e);
            }
            if (moveN == true)
            {
                result[3] = new Rectangle(b, g, c - b, h - g);
            }

            bool moveNW = moveN && moveW;
            bool moveNE = moveN && moveE;
            bool moveSE = moveS && moveE;
            bool moveSW = moveS && moveW;

            if (moveSW == true || moveSE == true)
            {
                if (corner1 || corner2)
                {
                    result[4] = new Rectangle(a, e, b - a, f - e);
                }
                else
                {
                    result[4] = new Rectangle(c, e, d - c, f - e);
                }
            }
            if (moveNW == true || moveNE == true)
            {
                if (corner1 || corner2)
                {
                    result[5] = new Rectangle(c, g, d - c, h - g);
                }
                else
                {
                    result[5] = new Rectangle(a, g, b - a, h - g);
                }
            }

            bool shrinkHor = deltaWidth < 0;
            bool shrinkE = shrinkHor == true && deltaX2 < 0;
            bool shrinkW = shrinkHor == true && deltaX > 0;

            bool shrinkVert = deltaHeight < 0;
            bool shrinkN = shrinkVert == true && deltaY > 0;
            bool shrinkS = shrinkVert == true && deltaY2 < 0;

            if (shrinkE == true)
            {
                result[2] = new Rectangle(c, f, d - c, g - f);
            }
            if (shrinkW == true)
            {
                result[1] = new Rectangle(a, f, b - a, g - f);
            }

            if (shrinkS == true)
            {
                result[3] = new Rectangle(b, g, c - b, h - g);
            }
            if (shrinkN == true)
            {
                result[0] = new Rectangle(b, e, c - b, f - e);
            }

            bool shrinkNW = shrinkN && shrinkW;
            bool shrinkNE = shrinkN && shrinkE;
            bool shrinkSW = shrinkS && shrinkW;
            bool shrinkSE = shrinkS && shrinkE;

            if (shrinkNW == true)
            {
                if (corner1 || corner2)
                {
                    result[4] = new Rectangle(a, e, b - a, f - e);
                }
                else
                {
                    result[4] = new Rectangle(c, e, d - c, f - e);
                }
            }
            if (shrinkNE == true)
            {
                if (corner1 || corner2)
                {
                    result[4] = new Rectangle(c, e, d - c, f - e);
                }
                else
                {
                    result[4] = new Rectangle(a, e, b - a, f - e);
                }
            }
            if (shrinkSW == true)
            {
                if (corner1 || corner2)
                {
                    result[5] = new Rectangle(a, g, b - a, h - g);
                }
                else
                {
                    result[5] = new Rectangle(c, g, d - c, h - g);
                }
            }
            if (shrinkSE == true)
            {
                if (corner1 || corner2)
                {
                    result[5] = new Rectangle(c, g, d - c, h - g);
                }
                else
                {
                    result[5] = new Rectangle(a, g, b - a, h - g);
                }
            }

            return result;
        }

        public virtual Rectangle[] GetAdded()
        {
            return GetAdded(this.previousArea, this.Area);
        }

        protected virtual Rectangle[] GetAdded(Rectangle previous, Rectangle current)
        {
            int deltaX = current.X - previous.X;
            int deltaY = current.Y - previous.Y;
            int deltaX2 = (current.X + current.Width) - (previous.X + previous.Width);
            int deltaY2 = (current.Y + current.Height) - (previous.Y + previous.Height);
            int deltaWidth = current.Width - previous.Width;
            int deltaHeight = current.Height - previous.Height;

            int a = Math.Min(previous.X, current.X);
            int b = Math.Max(previous.X, current.X);
            int c = Math.Min(previous.X + previous.Width, current.X + current.Width);
            int d = Math.Max(previous.X + previous.Width, current.X + current.Width);

            int e = Math.Min(previous.Y, current.Y);
            int f = Math.Max(previous.Y, current.Y);
            int g = Math.Min(previous.Y + previous.Height, current.Y + current.Height);
            int h = Math.Max(previous.Y + previous.Height, current.Y + current.Height);

            Rectangle[] result = new Rectangle[6];
            /* +---+-----+---+
               | 4 |  0  | 4 |
               +---+-----+---+
               |   |     |   |
               | 1 |     | 2 |
               |   |     |   |
               +---+-----+---+
               | 5 |  3  | 5 |
               +---+-----+---+
            */

            bool corner1 = previous.X == a && previous.Y == e;
            bool corner2 = current.X == a && current.Y == e;

            bool moveE = deltaX > 0;
            bool moveW = deltaX < 0;
            bool moveN = deltaY < 0;
            bool moveS = deltaY > 0;

            if (moveE == true)
            {
                result[2] = new Rectangle(c, f, d - c, g - f);
            }
            if (moveW == true)
            {
                result[1] = new Rectangle(a, f, b - a, g - f);
            }

            if (moveS == true)
            {
                result[3] = new Rectangle(b, g, c - b, h - g);
            }
            if (moveN == true)
            {
                result[0] = new Rectangle(b, e, c - b, f - e);
            }

            bool moveNW = moveN && moveW;
            bool moveNE = moveN && moveE;
            bool moveSE = moveS && moveE;
            bool moveSW = moveS && moveW;

            if (moveSW == true || moveSE == true)
            {
                if (corner1 || corner2)
                {
                    result[5] = new Rectangle(c, g, d - c, h - g);
                }
                else
                {
                    result[5] = new Rectangle(a, g, b - a, h - g);
                }
            }
            if (moveNW == true || moveNE == true)
            {
                if (corner1 || corner2)
                {
                    result[4] = new Rectangle(a, e, b - a, f - e);
                }
                else
                {
                    result[4] = new Rectangle(c, e, d - c, f - e);
                }
            }

            bool growHor = deltaWidth > 0;
            bool growE = growHor == true && deltaX2 > 0;
            bool growW = growHor == true && deltaX < 0;

            bool growVert = deltaHeight > 0;
            bool growN = growVert == true && deltaY < 0;
            bool growS = growVert == true && deltaY2 > 0;

            if (growE == true)
            {
                result[2] = new Rectangle(c, f, d - c, g - f);
            }
            if (growW == true)
            {
                result[1] = new Rectangle(a, f, b - a, g - f);
            }

            if (growS == true)
            {
                result[3] = new Rectangle(b, g, c - b, h - g);
            }
            if (growN == true)
            {
                result[0] = new Rectangle(b, e, c - b, f - e);
            }

            bool growNW = growN && growW;
            bool growNE = growN && growE;
            bool growSW = growS && growW;
            bool growSE = growS && growE;

            if (growNW == true)
            {
                if (corner1 || corner2)
                {
                    result[4] = new Rectangle(a, e, b - a, f - e);
                }
                else
                {
                    result[4] = new Rectangle(c, e, d - c, f - e);
                }
            }
            if (growNE == true)
            {
                if (corner1 || corner2)
                {
                    result[4] = new Rectangle(c, e, d - c, f - e);
                }
                else
                {
                    result[4] = new Rectangle(a, e, b - a, f - e);
                }
            }
            if (growSW == true)
            {
                if (corner1 || corner2)
                {
                    result[5] = new Rectangle(a, g, b - a, h - g);
                }
                else
                {
                    result[5] = new Rectangle(c, g, d - c, h - g);
                }
            }
            if (growSE == true)
            {
                if (corner1 || corner2)
                {
                    result[5] = new Rectangle(c, g, d - c, h - g);
                }
                else
                {
                    result[5] = new Rectangle(a, g, b - a, h - g);
                }
            }

            return result;
        }

        public virtual bool IsInCenter(Point point)
        {
            bool isInBounds = Area.Contains(point);
            if (isInBounds == false)
            {
                return false;
            }

            bool isInMargin;

            isInMargin = IsInNorthBorder(point);
            if (isInMargin == true)
            {
                return false;
            }

            isInMargin = IsInEastBorder(point);
            if (isInMargin == true)
            {
                return false;
            }

            isInMargin = IsInSouthBorder(point);
            if (isInMargin == true)
            {
                return false;
            }

            isInMargin = IsInWestBorder(point);
            if (isInMargin == true)
            {
                return false;
            }

            return true;
        }

        public virtual void Move(int deltaX, int deltaY)
        {
            if (deltaX == 0)
            {
                if (deltaY == 0)
                {
                    return;
                }
            }

            Rectangle oldArea = new Rectangle(Location.X, Location.Y, Size.Width, Size.Height);

            int updatedX = Location.X + deltaX;
            int updatedY = Location.Y + deltaY;

            Location = new Point(updatedX, updatedY);
        }

        public virtual bool IsInNorthBorder(Point point)
        {
            bool isInMargin = northMargin.Contains(point);
            return isInMargin;
        }

        public virtual void AdjustNorthEdge(int amount)
        {
            int updatedY = Location.Y + amount;
            int updatedHeight = Height - amount;

            Area = new Rectangle(Location.X, updatedY, Width, updatedHeight);
        }

        public virtual bool IsInSouthBorder(Point point)
        {
            bool isInMargin = southMargin.Contains(point);
            return isInMargin;
        }

        public virtual void AdjustSouthEdge(int amount)
        {
            int updatedHeight = Height + amount;

            Area = new Rectangle(Location.X, Location.Y, Width, updatedHeight);
        }

        public virtual bool IsInWestBorder(Point point)
        {
            bool isInMargin = westMargin.Contains(point);
            return isInMargin;
        }

        public virtual void AdjustWestEdge(int amount)
        {
            int updatedX = Location.X + amount;
            int updatedWidth = Width - amount;

            Area = new Rectangle(updatedX, Location.Y, updatedWidth, Height);
        }

        public virtual bool IsInEastBorder(Point point)
        {
            bool isInMargin = eastMargin.Contains(point);
            return isInMargin;
        }

        public virtual void AdjustEastEdge(int amount)
        {
            int updatedWidth = Width + amount;

            Area = new Rectangle(Location.X, Location.Y, updatedWidth, Height);
        }

        public virtual void OnMouseEnter(MouseEventArgs e)
        {
            Point cursor = e.Location;

            this.IsMouseOver = true;

            int state = GetCursorLocation(cursor);

            const int nw = (int)borders.north + (int)borders.west;
            const int ne = (int)borders.north + (int)borders.east;

            const int sw = (int)borders.south + (int)borders.west;
            const int se = (int)borders.south + (int)borders.east;

            switch (state)
            {
                // Resize vertically
                case (int)borders.north:
                case (int)borders.south:
                    this.Cursor = Cursors.SizeNS;
                    break;

                // Resize horizontally
                case (int)borders.west:
                case (int)borders.east:
                    this.Cursor = Cursors.SizeWE;
                    break;

                // Resize NWSE
                case nw:
                case se:
                    this.Cursor = Cursors.SizeNWSE;
                    break;

                // Resize NESW
                case ne:
                case sw:
                    this.Cursor = Cursors.SizeNESW;
                    break;

                // Move
                case (int)borders.center:
                default:
                    this.Cursor = Cursors.NoMove2D;
                    break;
            }
        }

        protected virtual int GetCursorLocation(Point cursor)
        {
            int location = 0;

            bool northBorder = IsInNorthBorder(cursor);
            if (northBorder == true)
            {
                location += (int)borders.north;
            }

            bool southBorder = IsInSouthBorder(cursor);
            if (southBorder == true)
            {
                location += (int)borders.south;
            }

            bool westBorder = IsInWestBorder(cursor);
            if (westBorder == true)
            {
                location += (int)borders.west;
            }

            bool eastBorder = IsInEastBorder(cursor);
            if (eastBorder == true)
            {
                location += (int)borders.east;
            }

            return location;
        }

        protected virtual int GetCursorBorder(Point cursor)
        {
            int border = 0;

            bool northBorder = IsInNorthBorder(cursor);
            if (northBorder == true)
            {
                border = (int)borders.north;
                return border;
            }

            bool southBorder = IsInSouthBorder(cursor);
            if (southBorder == true)
            {
                border = (int)borders.south;
                return border;
            }

            bool westBorder = IsInWestBorder(cursor);
            if (westBorder == true)
            {
                border = (int)borders.west;
                return border;
            }

            bool eastBorder = IsInEastBorder(cursor);
            if (eastBorder == true)
            {
                border = (int)borders.east;
                return border;
            }

            return border;
        }

        public virtual void OnMouseLeave(MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
            this.IsMouseOver = false;
        }

        public virtual void OnMouseDown(MouseEventArgs e)
        {
            Point cursor = e.Location;
            this.previousCursor = cursor;

            int margin = GetCursorLocation(cursor);
            this.currentMouseBorder = margin;
            this.HasMouseFocus = true;

            // Save the curent Area
            SetPreviousArea();
        }

        protected virtual void SetPreviousArea()
        {
            Rectangle area = this.Area;
            int x = area.Location.X;
            int y = area.Location.Y;
            int width = area.Width;
            int height = area.Height;

            this.previousArea = new Rectangle(x, y, width, height);
        }

        public virtual void OnMouseMove(MouseEventArgs e)
        {
            if (HasMouseFocus == false)
            {
                return;
            }

            Point cursor = e.Location;

            int deltaY = cursor.Y - previousCursor.Y;
            int deltaX = cursor.X - previousCursor.X;

            const int nw = (int)borders.north + (int)borders.west;
            const int ne = (int)borders.north + (int)borders.east;

            const int sw = (int)borders.south + (int)borders.west;
            const int se = (int)borders.south + (int)borders.east;

            switch (currentMouseBorder)
            {
                case 0:
                    Move(deltaX, deltaY);
                    break;

                case nw:
                    AdjustNorthEdge(deltaY);
                    AdjustWestEdge(deltaX);
                    break;

                case ne:
                    AdjustNorthEdge(deltaY);
                    AdjustEastEdge(deltaX);
                    break;

                case sw:
                    AdjustSouthEdge(deltaY);
                    AdjustWestEdge(deltaX);
                    break;

                case se:
                    AdjustSouthEdge(deltaY);
                    AdjustEastEdge(deltaX);
                    break;

                case (int)borders.north:
                    AdjustNorthEdge(deltaY);
                    break;

                case (int)borders.south:
                    AdjustSouthEdge(deltaY);
                    break;

                case (int)borders.west:
                    AdjustWestEdge(deltaX);
                    break;

                case (int)borders.east:
                    AdjustEastEdge(deltaX);
                    break;

                default:
                    return;
            }

            this.previousCursor = cursor;
        }

        public virtual void OnMouseUp(MouseEventArgs e)
        {
            SnapToGrid();
            SetBorders();
            this.currentMouseBorder = -1;
            this.HasMouseFocus = false;
        }

        protected virtual void SnapToGrid()
        {
            int x = this.Location.X;
            int y = this.Location.Y;
            int width = this.Width;
            int height = this.Height;

            Point location;
            Size size;
            Program.SnapToGrid(x, y, width, height, out location, out size);
            this.Location = location;
            this.Size = size;
        }

        public virtual void KeepInBounds(Rectangle bounds, int x, int y)
        {
            int tileLength = Program.TileLength;

            int minX = bounds.X;
            int maxX = minX + bounds.Width - tileLength;

            int minY = bounds.Y;
            int maxY = minY + bounds.Height - tileLength;

            x = Program.Clamp(x, minX, maxX);
            y = Program.Clamp(y, minY, maxY);

            this.Location = new Point(x, y);
        }
    }
}