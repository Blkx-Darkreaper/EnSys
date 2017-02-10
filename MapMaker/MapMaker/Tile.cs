using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MapMaker
{
    public class Tile : IEquatable<Tile>
    {
        public Point Corner { get; protected set; }
        public int TilesetIndex { get; set; }

        public Tile(int index, int x, int y)
        {
            this.TilesetIndex = index;
            this.Corner = new Point(x, y);
        }

        public bool Equals(Tile other)
        {
            return TilesetIndex.Equals(other.TilesetIndex);
        }
    }
}