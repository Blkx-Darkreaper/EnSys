using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        public void UpdateWidth(int width)
        {
            int height = this.Size.Height;
            this.Size = new Size(width, height);
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
            deltaPixelX = 0;
            if (deltaPixelY == 0)
            {
                return;
            }

            base.Move(deltaPixelX, deltaPixelY);

            UpdateSectorPositions(this.PixelArea, deltaPixelX, deltaPixelY);
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
    }
}