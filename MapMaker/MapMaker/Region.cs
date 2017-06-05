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
        [JsonIgnore] public Rectangle PixelArea { get; protected set; }  // pixels
        protected Rectangle previousPixelArea { get; set; }  // pixels
        public Point Location {
            get {
                if(Program.TileLength == 0)
                {
                    return new Point();
                }

                Point location = new Point(PixelArea.Location.X / Program.TileLength, PixelArea.Location.Y / Program.TileLength);
                return location;
            }
            protected set { PixelArea = UpdateAreaLocation(new Point(value.X * Program.TileLength, value.Y * Program.TileLength)); } } // Units
        public Size Size {
            get {
                if(Program.TileLength == 0)
                {
                    return new Size();
                }

                Size size = new Size(PixelArea.Width / Program.TileLength, PixelArea.Height / Program.TileLength);
                return size;
            }
            protected set { PixelArea = UpdateAreaSize(new Size(value.Width * Program.TileLength, value.Height * Program.TileLength)); } }  // Units
        [JsonIgnore] public int PixelWidth { get { return PixelArea.Width; } protected set { PixelArea = UpdateAreaWidth(value); } }    // pixels
        [JsonIgnore] public int PixelHeight { get { return PixelArea.Height; } protected set { PixelArea = UpdateAreaHeight(value); } } // pixels
        [JsonIgnore] public Cursor Cursor { get; protected set; }
        [JsonIgnore] public bool IsMouseOver { get; protected set; }
        [JsonIgnore] public bool HasMouseFocus { get; protected set; }
        [JsonIgnore] public bool IsSelected { get { return Program.SelectedRegion == this; } }
        protected Rectangle northMargin { get; set; }   // pixels
        protected Rectangle southMargin { get; set; }   // pixels
        protected Rectangle westMargin { get; set; }    // pixels
        protected Rectangle eastMargin { get; set; }    // pixels
        protected enum borders { center = 0, north = 1, west = 2, east = 4, south = 8 }
        protected int currentMouseBorder { get; set; }
        protected Point previousCursor { get; set; }

        public Region()
        {
            this.PixelArea = new Rectangle();
            this.previousPixelArea = new Rectangle();
            this.Cursor = Cursors.Default;
            this.IsMouseOver = false;
            this.HasMouseFocus = false;
            this.currentMouseBorder = -1;

            SetBorders();
        }

        public Region(int x, int y, int width, int height)
            : this()
        {
            this.Location = new Point(x, y);
            this.Size = new Size(width, height);

            int tileLength = Program.TileLength;

            int pixelX = x * tileLength;
            int pixelY = y * tileLength;

            int pixelWidth = width * tileLength;
            int pixelHeight = height * tileLength;

            this.PixelArea = new Rectangle(pixelX, pixelY, pixelWidth, pixelHeight);
            //this.PixelWidth = width * tileLength;
            //this.PixelHeight = height * tileLength;

            SetBorders();
        }

        public virtual void Draw(Graphics graphics, double scale) { }

        public virtual void Draw(Graphics graphics, double scale, Color colour) { }

        public virtual void UpdateWidth(int width)
        {
            int height = this.Size.Height;
            this.Size = new Size(width, height);
        }

        public virtual void SetBorders()
        {
            SetBordersAbsolute(this.PixelArea);
        }

        protected virtual void SetBordersAbsolute(Rectangle pixelArea)
        {
            int tileLength = Program.TileLength;
            if (tileLength == 0)
            {
                throw new InvalidOperationException("Tile length has not been set");
            }

            int pixelX = pixelArea.X;
            int pixelY = pixelArea.Y;
            int pixelWidth = pixelArea.Width;
            int pixelHeight = pixelArea.Height;

            northMargin = new Rectangle(pixelX, pixelY, pixelWidth, tileLength);

            westMargin = new Rectangle(pixelX, pixelY, tileLength, pixelHeight);

            int bottomY = pixelY + pixelHeight - tileLength;
            southMargin = new Rectangle(pixelX, bottomY, pixelWidth, tileLength);

            int rightX = pixelX + pixelWidth - tileLength;
            eastMargin = new Rectangle(rightX, pixelY, tileLength, pixelHeight);
        }

        protected virtual void SetBordersRelative(Rectangle pixelArea)
        {
            int pixelX = pixelArea.X;
            int pixelY = pixelArea.Y;
            int pixelWidth = pixelArea.Width;
            int pixelHeight = pixelArea.Height;

            int quarterPixelHeight = pixelHeight / 4;
            northMargin = new Rectangle(pixelX, pixelY, pixelWidth, quarterPixelHeight);

            int quarterWidth = pixelWidth / 4;
            westMargin = new Rectangle(pixelX, pixelY, quarterWidth, pixelHeight);

            int bottomY = pixelY + 3 * quarterPixelHeight;
            southMargin = new Rectangle(pixelX, bottomY, pixelWidth, quarterPixelHeight);

            int rightX = pixelX + 3 * quarterWidth;
            eastMargin = new Rectangle(rightX, pixelY, quarterWidth, pixelHeight);
        }

        public Rectangle GetScaledBounds(double scale)
        {
            int scaledPixelX = (int)Math.Round(PixelArea.Location.X * scale, 0);    // pixels
            int scaledPixelY = (int)Math.Round(PixelArea.Location.Y * scale, 0); // pixels

            int scaledPixelWidth = (int)Math.Round(PixelArea.Width * scale, 0); // pixels
            int scaledPixelHeight = (int)Math.Round(PixelArea.Height * scale, 0);   //pixels

            Rectangle scaledBounds = new Rectangle(scaledPixelX, scaledPixelY, scaledPixelWidth, scaledPixelHeight);
            return scaledBounds;
        }

        public int CompareTo(Region other)
        {
            int area = this.PixelWidth * this.PixelHeight;
            int otherArea = other.PixelWidth * other.PixelHeight;

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

            int width = this.PixelWidth;
            int otherWidth = other.PixelWidth;
            if (width != otherWidth)
            {
                return false;
            }

            int height = this.PixelHeight;
            int otherHeight = other.PixelHeight;
            if (height != otherHeight)
            {
                return false;
            }

            return true;
        }

        protected virtual Rectangle UpdateAreaLocation(Point pixelLocation)
        {
            Size pixelSize = new Size(Size.Width * Program.TileLength, Size.Height * Program.TileLength);

            return UpdateArea(pixelLocation, pixelSize);
        }

        protected virtual Rectangle UpdateAreaSize(Size pixelSize)
        {
            Point pixelLocation = new Point(Location.X * Program.TileLength, Location.Y * Program.TileLength);

            return UpdateArea(pixelLocation, pixelSize);
        }

        protected virtual Rectangle UpdateAreaWidth(int pixelWidth)
        {
            Point pixelLocation = new Point(Location.X * Program.TileLength, Location.Y * Program.TileLength);

            return UpdateArea(pixelLocation, new Size(pixelWidth, this.PixelHeight));
        }

        protected virtual Rectangle UpdateAreaHeight(int pixelHeight)
        {
            Point pixelLocation = new Point(Location.X * Program.TileLength, Location.Y * Program.TileLength);

            return UpdateArea(pixelLocation, new Size(this.PixelWidth, pixelHeight));
        }

        protected virtual Rectangle UpdateArea(Point pixelLocation, Size pixelSize)
        {
            int pixelX = pixelLocation.X;
            int pixelY = pixelLocation.Y;
            int pixelWidth = pixelSize.Width;
            int pixelHeight = pixelSize.Height;

            Rectangle updatedArea = new Rectangle(pixelX, pixelY, pixelWidth, pixelHeight);
            SetBordersAbsolute(updatedArea);

            return updatedArea;
        }

        public virtual Rectangle[] GetRemoved()
        {
            return GetRemoved(this.previousPixelArea, this.PixelArea);
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
            return GetAdded(this.previousPixelArea, this.PixelArea);
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
            bool isInBounds = PixelArea.Contains(point);
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

        public virtual void Move(int deltaPixelX, int deltaPixelY)
        {
            if(Program.TileLength == 0)
            {
                throw new InvalidOperationException("Tile length has not been set");
            }

            if (deltaPixelX == 0)
            {
                if (deltaPixelY == 0)
                {
                    return;
                }
            }

            //Rectangle oldArea = new Rectangle(Location.X, Location.Y, Size.Width, Size.Height);

            // Convert to Units (tiles)
            int deltaX = deltaPixelX / Program.TileLength;
            int deltaY = deltaPixelY / Program.TileLength;

            int updatedX = Location.X + deltaX;
            int updatedY = Location.Y + deltaY;

            this.Location = new Point(updatedX, updatedY);
        }

        public virtual bool IsInNorthBorder(Point point)
        {
            bool isInMargin = northMargin.Contains(point);
            return isInMargin;
        }

        public virtual void AdjustNorthEdge(int pixels)
        {
            int tileLength = Program.TileLength;

            int pixelX = Location.X * tileLength;
            int updatedPixelY = Location.Y * tileLength + pixels;
            int updatedPixelHeight = PixelHeight - pixels;

            this.PixelArea = new Rectangle(pixelX, updatedPixelY, PixelWidth, updatedPixelHeight);
        }

        public virtual bool IsInSouthBorder(Point point)
        {
            bool isInMargin = southMargin.Contains(point);
            return isInMargin;
        }

        public virtual void AdjustSouthEdge(int pixels)
        {
            int tileLength = Program.TileLength;

            int pixelX = Location.X * tileLength;
            int pixelY = Location.Y * tileLength;

            int updatedPixelHeight = PixelHeight + pixels;

            this.PixelArea = new Rectangle(pixelX, pixelY, PixelWidth, updatedPixelHeight);
        }

        public virtual bool IsInWestBorder(Point point)
        {
            bool isInMargin = westMargin.Contains(point);
            return isInMargin;
        }

        public virtual void AdjustWestEdge(int pixels)
        {
            int tileLength = Program.TileLength;

            int updatedPixelX = Location.X * tileLength + pixels;
            int pixelY = Location.Y * tileLength;

            int updatedPixelWidth = PixelWidth - pixels;

            this.PixelArea = new Rectangle(updatedPixelX, pixelY, updatedPixelWidth, PixelHeight);
        }

        public virtual bool IsInEastBorder(Point point)
        {
            bool isInMargin = eastMargin.Contains(point);
            return isInMargin;
        }

        public virtual void AdjustEastEdge(int pixels)
        {
            int tileLength = Program.TileLength;

            int pixelX = Location.X * tileLength;
            int pixelY = Location.Y * tileLength;

            int updatedPixelWidth = PixelWidth + pixels;

            this.PixelArea = new Rectangle(pixelX, pixelY, updatedPixelWidth, PixelHeight);
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
            Rectangle area = this.PixelArea;
            int pixelX = area.Location.X;
            int pixelY = area.Location.Y;
            int pixelWidth = area.Width;
            int pixelHeight = area.Height;

            this.previousPixelArea = new Rectangle(pixelX, pixelY, pixelWidth, pixelHeight);
        }

        public virtual void OnMouseMove(MouseEventArgs e)
        {
            Point cursor = e.Location;

            // Double movement
            int deltaPixelY = (cursor.Y - previousCursor.Y) * 2;
            int deltaPixelX = (cursor.X - previousCursor.X) * 2;

            int tileLength = Program.TileLength;
            if(Math.Abs(deltaPixelX) < tileLength)
            {
                if(Math.Abs(deltaPixelY) < tileLength) {
                    return;
                }
            }

            const int nw = (int)borders.north + (int)borders.west;
            const int ne = (int)borders.north + (int)borders.east;

            const int sw = (int)borders.south + (int)borders.west;
            const int se = (int)borders.south + (int)borders.east;

            switch (currentMouseBorder)
            {
                case 0:
                    Move(deltaPixelX, deltaPixelY);
                    break;

                case nw:
                    AdjustNorthEdge(deltaPixelY);
                    AdjustWestEdge(deltaPixelX);
                    break;

                case ne:
                    AdjustNorthEdge(deltaPixelY);
                    AdjustEastEdge(deltaPixelX);
                    break;

                case sw:
                    AdjustSouthEdge(deltaPixelY);
                    AdjustWestEdge(deltaPixelX);
                    break;

                case se:
                    AdjustSouthEdge(deltaPixelY);
                    AdjustEastEdge(deltaPixelX);
                    break;

                case (int)borders.north:
                    AdjustNorthEdge(deltaPixelY);
                    break;

                case (int)borders.south:
                    AdjustSouthEdge(deltaPixelY);
                    break;

                case (int)borders.west:
                    AdjustWestEdge(deltaPixelX);
                    break;

                case (int)borders.east:
                    AdjustEastEdge(deltaPixelX);
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

        public virtual void OnMouseClick(MouseEventArgs e)
        {

        }

        public virtual void Delete() { }

        protected virtual void SnapToGrid()
        {
            if(Program.TileLength == 0)
            {
                throw new InvalidOperationException("Tile length has not been set");
            }

            int pixelX = this.Location.X * Program.TileLength;
            int pixelY = this.Location.Y * Program.TileLength;
            int pixelWidth = this.PixelWidth;
            int pixelHeight = this.PixelHeight;

            Point location;
            Size size;
            Program.SnapToGrid(pixelX, pixelY, pixelWidth, pixelHeight, out location, out size);
            this.Location = location;
            this.Size = size;
        }

        public virtual void KeepInBounds(Rectangle pixelBounds, int x, int y, int width, int height)
        {
            int tileLength = Program.TileLength;

            int minX = pixelBounds.X / tileLength;
            int maxWidth = pixelBounds.Width / tileLength;
            int maxX = minX + maxWidth - width;

            int minY = pixelBounds.Y / tileLength;
            int maxHeight = pixelBounds.Height / tileLength;
            int maxY = minY + maxHeight - height;

            x = Program.Clamp(x, minX, maxX);
            y = Program.Clamp(y, minY, maxY);

            if(this.Location.X == x)
            {
                if(this.Location.Y == y)
                {
                    return;
                }
            }

            this.Location = new Point(x, y);
        }
    }
}