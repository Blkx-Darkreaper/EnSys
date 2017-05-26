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
        public Point Location { get; protected set; }   // Units
        public bool AllowsDriving { get; protected set; }
        public bool AllowsFlying { get; protected set; }
        public bool AllowsConstruction { get; protected set; }
        public Tile Tile { get; set; }
        protected bool isCopy { get; set; }
        [JsonIgnore] public bool IsComplete { get; protected set; }

        protected Grid() { }

        protected Grid(Grid copy)
        {
            MatchCopy(copy);
        }

        public Grid(int x, int y)
        {
            this.Location = new Point(x, y);
            this.IsComplete = false;
        }

        public Grid(int x, int y, Tile tile) : this(x, y)
        {
            this.Id = nextId++;
            this.isCopy = false;
            this.IsComplete = true;
            this.AllowsDriving = true;
            this.AllowsFlying = true;
            this.AllowsConstruction = true;
            this.Tile = tile;
        }

        [JsonConstructor]
        public Grid(int id, Point location, bool allowsDriving, bool allowsFlying, bool allowsConstruction, Tile tile) : this(location.X, location.Y, tile) {
            this.Id = id;
            this.AllowsDriving = allowsDriving;
            this.AllowsFlying = allowsFlying;
            this.AllowsConstruction = allowsConstruction;

            SetNextId(id);
        }

        protected void SetNextId(int id)
        {
            if (id < nextId)
            {
                return;
            }

            nextId = id;
        }

        public bool Equals(Grid other)
        {
            int x = this.Location.X;
            int otherX = other.Location.X;
            if (x != otherX)
            {
                return false;
            }

            int y = this.Location.Y;
            int otherY = other.Location.Y;
            if (y != otherY)
            {
                return false;
            }

            return true;
        }

        public int CompareTo(Grid other)
        {
            int comparison;

            int y = Location.Y;
            int otherY = other.Location.Y;

            comparison = y - otherY;
            if (comparison != 0)
            {
                return comparison;
            }

            int x = Location.X;
            int otherX = other.Location.X;

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

        public Grid GetCopy()
        {
            Grid copy = new Grid();
            copy.Id = this.Id;
            copy.Location = new Point(this.Location.X, this.Location.Y);
            copy.AllowsConstruction = this.AllowsConstruction;
            copy.AllowsDriving = this.AllowsDriving;
            copy.AllowsFlying = this.AllowsFlying;
            copy.Tile = this.Tile;
            copy.isCopy = true;
            copy.IsComplete = true;

            return copy;
        }

        public void MatchCopy(Grid copy)
        {
            //this.Id = copy.Id;
            this.Location = copy.Location;
            this.AllowsConstruction = copy.AllowsConstruction;
            this.AllowsDriving = copy.AllowsDriving;
            this.AllowsFlying = copy.AllowsFlying;
            this.Tile = copy.Tile;
        }
    }
}
