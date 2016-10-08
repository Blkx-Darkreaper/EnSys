using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;

namespace MapMaker
{
    public class Grid : IComparable<Grid>, IEquatable<Grid>
    {
        public int Id { get; protected set; }
        protected static int nextId = 1;
        public Point Corner { get; protected set; }
        public bool AllowsDriving { get; protected set; }
        public bool AllowsFlying { get; protected set; }
        public bool AllowsConstruction { get; protected set; }
        public int SectorId { get; set; }
        public static int NextSector = 0;
        public int Zone { get; protected set; }
        public Tile Tile { get; set; }
        protected bool isCopy { get; set; }

        protected Grid(Grid copy)
        {
            MatchCopy(copy);
        }

        public Grid(Point corner) : this(corner, 0) { }

        public Grid(Point corner, Tile tile)
        {
            this.Corner = corner;
            this.Tile = tile;
        }

        public Grid(Point corner, int zone)
        {
            this.Id = nextId++;
            this.Corner = corner;
            this.AllowsDriving = true;
            this.AllowsFlying = true;
            this.AllowsConstruction = true;
            this.SectorId = NextSector;
            this.Zone = zone;
            this.isCopy = false;
        }

        [JsonConstructor]
        public Grid(int id, Point corner, bool allowsDriving, bool allowsFlying, bool allowsConstruction, int sector, int zone, Tile tile) : this(corner, tile) {
            this.Id = id;
            this.AllowsDriving = allowsDriving;
            this.AllowsFlying = allowsFlying;
            this.AllowsConstruction = allowsConstruction;
            this.SectorId = sector;
            this.Zone = zone;
        }

        //public bool Equals(Grid other)
        //{
        //    if (other == null)
        //    {
        //        return false;
        //    }

        //    return Id.Equals(other.Id);
        //}

        public bool Equals(Grid other)
        {
            int x = this.Corner.X;
            int otherX = other.Corner.X;
            if (x != otherX)
            {
                return false;
            }

            int y = this.Corner.Y;
            int otherY = other.Corner.Y;
            if (y != otherY)
            {
                return false;
            }

            return true;
        }

        public int CompareTo(Grid other)
        {
            int comparison;

            int y = Corner.Y;
            int otherY = other.Corner.Y;

            comparison = y - otherY;
            if (comparison != 0)
            {
                return comparison;
            }

            int x = Corner.X;
            int otherX = other.Corner.X;

            comparison = x - otherX;

            return comparison;
        }

        public virtual void ToggleDriveable()
        {
            this.AllowsDriving = !this.AllowsDriving;
        }

        public virtual void SetDrivable(bool value)
        {
            this.AllowsDriving = value;
        }

        public virtual void ToggleFlyable()
        {
            this.AllowsFlying = !this.AllowsFlying;
        }

        public virtual void SetFlyable(bool value)
        {
            this.AllowsFlying = value;
        }

        public virtual void ToggleConstructable()
        {
            this.AllowsConstruction = !this.AllowsConstruction;
        }

        public virtual void SetConstructable(bool value)
        {
            this.AllowsConstruction = value;
        }

        public virtual void CycleSector()
        {
            this.SectorId++;
            this.SectorId %= Program.MaxSectors;
        }

        public Grid GetCopy()
        {
            Grid copy = new Grid(this.Corner);
            copy.Id = this.Id;
            copy.AllowsConstruction = this.AllowsConstruction;
            copy.AllowsDriving = this.AllowsDriving;
            copy.AllowsFlying = this.AllowsFlying;
            copy.SectorId = this.SectorId;
            copy.Tile = this.Tile;
            copy.Zone = this.Zone;
            copy.isCopy = true;

            return copy;
        }

        public void MatchCopy(Grid copy)
        {
            //this.Id = copy.Id;
            this.Corner = copy.Corner;
            this.AllowsConstruction = copy.AllowsConstruction;
            this.AllowsDriving = copy.AllowsDriving;
            this.AllowsFlying = copy.AllowsFlying;
            this.SectorId = copy.SectorId;
            this.Tile = copy.Tile;
            this.Zone = copy.Zone;
        }
    }
}
