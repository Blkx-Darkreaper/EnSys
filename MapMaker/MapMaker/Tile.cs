using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;

namespace MapMaker
{
    public class Tile : IEquatable<Tile>
    {
        public Point Location { get; protected set; }   // Units
        public int TilesetIndex { get; set; }

        [JsonConstructor]
        public Tile(int index, Point location) : this(index, location.X, location.Y) { }

        public Tile(int index, int x, int y)
        {
            this.TilesetIndex = index;
            this.Location = new Point(x, y);
        }

        public bool Equals(Tile other)
        {
            return TilesetIndex.Equals(other.TilesetIndex);
        }
    }
}