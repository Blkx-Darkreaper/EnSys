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

            int scaledX = (int)Math.Round(Location.X * scale, 0);
            int scaledY = (int)Math.Round(Location.Y * scale, 0);
            Point scaledLocation = new Point(scaledX, scaledY);
            graphics.DrawString(Id.ToString(), font, Brushes.White, scaledLocation);
        }

        public override void Delete()
        {
            foreach (Sector sector in AllSectors)
            {
                sector.Delete();
            }

            Program.AllZones.Remove(this.Id);

            Program.UpdateGridZones(this.Area, 0);
        }

        public void AddSector(Sector sectorToAdd)
        {
            this.AllSectors.Add(sectorToAdd);
            sectorToAdd.Parent = this;
        }
    }
}