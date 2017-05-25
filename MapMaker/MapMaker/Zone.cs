using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

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
            Rectangle updatedArea = base.UpdateArea(pixelLocation, pixelSize);

            UpdateSectorPositions(this.PixelArea, 0, 0);

            return updatedArea;
        }

        public override void Move(int deltaPixelX, int deltaPixelY)
        {
            if (deltaPixelX == 0)
            {
                if (deltaPixelY == 0)
                {
                    return;
                }
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

            Program.UpdateGridZones(this.PixelArea, 0);
        }

        protected virtual void UpdateSectorPositions(Rectangle updatedArea, int deltaX, int deltaY)
        {
            if(AllSectors == null)
            {
                return;
            }

            if (AllSectors.Count == 0)
            {
                return;
            }

            foreach (Sector sector in AllSectors)
            {
                sector.Move(deltaX, deltaY);
                sector.KeepInBounds(updatedArea, sector.Location.X, sector.Location.Y);
            }
        }

        public void AddSector(Sector sectorToAdd)
        {
            this.AllSectors.Add(sectorToAdd);
            sectorToAdd.Parent = this;
        }
    }
}