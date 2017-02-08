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
        public bool IsHeadquartersSpawn { get; protected set; }
        public bool IsSectorSpawn { get; protected set; }
        public bool IsRaiderSpawn { get; protected set; }
        public int Sector { get; set; }
        public static int NextSector = 0;
        public int Zone { get; set; }
        public static int NextZone = 0;
        public Tile Tile { get; set; }
        protected bool isCopy { get; set; }
        [JsonIgnore] public bool IsComplete { get; protected set; }

        protected Grid() { }

        protected Grid(Grid copy)
        {
            MatchCopy(copy);
        }

        public Grid(Point corner)
        {
            this.Corner = corner;
            this.IsComplete = false;
        }

        public Grid(Point corner, Tile tile) : this(corner, NextSector, NextZone, tile) { }

        public Grid(Point corner, int sector, Tile tile) : this(corner)
        {
            this.AllowsDriving = true;
            this.AllowsFlying = true;
            this.AllowsConstruction = true;
            this.Sector = sector;
            this.Tile = tile;
        }

        public Grid(Point corner, int sector, int zone, Tile tile) : this(corner, sector, tile)
        {
            this.Id = nextId++;
            this.Zone = zone;
            this.isCopy = false;
            this.IsComplete = true;
        }

        [JsonConstructor]
        public Grid(int id, Point corner, bool isHeadquartersSpawn, bool isSectorSpawn, bool isRaiderSpawn, 
            bool allowsDriving, bool allowsFlying, bool allowsConstruction, int sector, int zone, Tile tile) : this(corner, tile) {
            this.Id = id;
            this.IsHeadquartersSpawn = isHeadquartersSpawn;
            this.IsSectorSpawn = isSectorSpawn;
            this.IsRaiderSpawn = isRaiderSpawn;
            this.AllowsDriving = allowsDriving;
            this.AllowsFlying = allowsFlying;
            this.AllowsConstruction = allowsConstruction;
            this.Sector = sector;
            this.Zone = zone;
        }

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
            this.SetDrivable(!this.AllowsDriving);
        }

        public virtual void SetDrivable(bool value)
        {
            this.AllowsDriving = value;
        }

        public virtual void ToggleFlyable()
        {
            this.SetDrivable(!this.AllowsFlying);
        }

        public virtual void SetFlyable(bool value)
        {
            this.AllowsFlying = value;
        }

        public virtual void ToggleConstructable()
        {
            this.SetDrivable(!this.AllowsConstruction);
        }

        public virtual void SetConstructable(bool value)
        {
            this.AllowsConstruction = value;
        }

        public virtual void CycleSector()
        {
            this.Sector++;
            this.Sector %= (Program.AllSectors.Count + 1);
        }

        public Grid GetCopy()
        {
            Grid copy = new Grid();
            copy.Id = this.Id;
            copy.Corner = new Point(this.Corner.X, this.Corner.Y);
            copy.AllowsConstruction = this.AllowsConstruction;
            copy.AllowsDriving = this.AllowsDriving;
            copy.AllowsFlying = this.AllowsFlying;
            copy.Sector = this.Sector;
            copy.Tile = this.Tile;
            copy.Zone = this.Zone;
            copy.isCopy = true;
            copy.IsComplete = true;

            return copy;
        }

        public void MatchCopy(Grid copy)
        {
            //this.Id = copy.Id;
            this.Corner = copy.Corner;
            this.AllowsConstruction = copy.AllowsConstruction;
            this.AllowsDriving = copy.AllowsDriving;
            this.AllowsFlying = copy.AllowsFlying;
            this.Sector = copy.Sector;
            this.Tile = copy.Tile;
            this.Zone = copy.Zone;
        }
    }
}
