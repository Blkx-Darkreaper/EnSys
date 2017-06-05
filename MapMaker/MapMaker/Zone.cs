using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MapMaker
{
    public class Zone : ExpandableRegion
    {
        public List<Sector> AllSectors { get; protected set; }

        public Zone(int id)
            : base(id)
        {
            this.AllSectors = new List<Sector>();
        }

        public Zone(int id, int x, int y, int width, int height) : base(id, x, y, width, height)
        {
            this.AllSectors = new List<Sector>();
        }

        [JsonConstructor]
        public Zone(int id, Point location, Size size, List<Sector> allSectors) : base(id, location.X, location.Y, size.Width, size.Height)
        {
            this.AllSectors = allSectors;
        }

        public override void Draw(Graphics graphics, double scale, Color colour)
        {
            Pen pen = new Pen(colour, 2);
            if (IsMouseOver == true || IsSelected == true)
            {
                pen = new Pen(colour, 4);
            }

            pen.Alignment = PenAlignment.Inset;

            Rectangle bounds = GetScaledBounds(scale);

            graphics.DrawRectangle(pen, bounds);

            Font font = new Font(FontFamily.GenericSansSerif, 12f);

            int scaledPixelX = (int)Math.Round(Location.X * Program.TileLength * scale, 0); // pixels
            int scaledPixelY = (int)Math.Round(Location.Y * Program.TileLength * scale, 0); // pixels
            Point scaledLocation = new Point(scaledPixelX, scaledPixelY);
            graphics.DrawString(Id.ToString(), font, Brushes.White, scaledLocation);
        }

        protected override Rectangle UpdateArea(Point pixelLocation, Size pixelSize)
        {
            int pixelX = 0;
            int pixelY = pixelLocation.Y;
            pixelLocation = new Point(pixelX, pixelY);

            int pixelWidth = Program.MapSize.Width * Program.TileLength;
            int pixelHeight = pixelSize.Height;
            pixelSize = new Size(pixelWidth, pixelHeight);

            Rectangle updatedArea = base.UpdateArea(pixelLocation, pixelSize);

            UpdateSectorPositions(this.PixelArea, 0, 0);

            return updatedArea;
        }

        public override void Move(int deltaPixelX, int deltaPixelY)
        {
            deltaPixelX = 0;    // Cannot move horizontally
            if (deltaPixelY == 0)
            {
                return;
            }

            base.Move(deltaPixelX, deltaPixelY);

            UpdateSectorPositions(this.PixelArea, deltaPixelX, deltaPixelY);
        }

        public override void OnMouseEnter(MouseEventArgs e)
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
                case nw:
                case se:
                case ne:
                case sw:
                    this.Cursor = Cursors.SizeNS;
                    break;

                // Cannot resize horizontally
                case (int)borders.west:
                case (int)borders.east:
                    this.Cursor = Cursors.Default;
                    break;

                // Move
                case (int)borders.center:
                default:
                    this.Cursor = Cursors.NoMove2D;
                    break;
            }

            this.previousCursor = cursor;
        }

        public override void AdjustEastEdge(int pixels)
        {
            
        }

        public override void AdjustWestEdge(int pixels)
        {
            
        }

        public override void Delete()
        {
            foreach (Sector sector in AllSectors)
            {
                sector.Delete();
            }

            Program.AllZones.Remove(this.Id);
        }

        protected virtual void UpdateSectorPositions(Rectangle updatedArea, int deltaPixelX, int deltaPixelY)
        {
            if (AllSectors == null)
            {
                return;
            }

            if (AllSectors.Count == 0)
            {
                return;
            }

            foreach (Sector sector in AllSectors)
            {
                sector.Move(deltaPixelX, deltaPixelY);
                sector.KeepInBounds(updatedArea, sector.Location.X, sector.Location.Y, sector.Size.Width, sector.Size.Height);
            }
        }

        public void AddSector(Sector sectorToAdd)
        {
            this.AllSectors.Add(sectorToAdd);
            sectorToAdd.SetParentZone(this);
        }

        public new Zone GetCopy()
        {
            Region regionCopy = base.GetCopy();
            Zone copy = (Zone)regionCopy;

            copy.Id = this.Id;
            copy.AllSectors = new List<Sector>(this.AllSectors);

            return copy;
        }

        public void MatchCopy(Zone copy)
        {
            this.Id = copy.Id;
            this.AllSectors = copy.AllSectors;

            base.MatchCopy(copy);
        }
    }
}