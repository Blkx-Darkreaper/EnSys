using System;
using System.Collections.Generic;
using System.Drawing;

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

        public void AddSector(Sector sectorToAdd)
        {
            this.AllSectors.Add(sectorToAdd);
            sectorToAdd.Parent = this;
        }
    }
}