using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MapMaker
{
    public class Sector : ExpandableRegion
    {
        public Zone Parent { get; set; }
        public Point SpawnPoint { get; protected set; }

        public Sector(int id, int tileLength)
            : base(id, tileLength)
        {
        }

        public Sector(int id, int tileLength, int x, int y, int width, int height) : base(id, tileLength, x, y, width, height)
        {
        }
    }
}