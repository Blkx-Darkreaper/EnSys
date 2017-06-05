using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker
{
    public class DrawState  // Caretaker
    {
        // previous states
        public List<Grid> AllGridMementos { get; protected set; }
        public List<Region> AllRegionMementos { get; protected set; }
        // future states
        public List<Grid> AllGridOmens { get; protected set; }
        public List<Region> AllRegionOmens { get; protected set; }
        public bool IsEmpty { get; protected set; }

        public DrawState(Grid grid)
            : this()
        {
            AddMemento(grid);
        }

        public DrawState()
        {
            AllGridMementos = new List<Grid>();
            AllRegionMementos = new List<Region>();
            AllGridOmens = new List<Grid>();
            AllRegionOmens = new List<Region>();
            IsEmpty = true;
        }

        public void AddMemento(Grid grid)
        {
            if (grid == null)
            {
                return;
            }

            Grid memento = grid.GetCopy();

            int index = AllGridMementos.FindIndex(g => g.Equals(grid));
            if (index != -1)
            {
                //AllMementos[index] = memento;
                return;
            }

            AllGridMementos.Add(memento);
            IsEmpty = false;
        }

        public void AddMemento(Region region)
        {
            if (region == null)
            {
                return;
            }

            Region memento = region.GetCopy();

            int index = AllRegionMementos.FindIndex(g => g.Equals(region));
            if (index != -1)
            {
                //AllMementos[index] = memento;
                return;
            }

            AllRegionMementos.Add(memento);
            IsEmpty = false;
        }

        public void AddOmen(Grid grid)
        {
            if (grid == null)
            {
                return;
            }

            Grid omen = grid.GetCopy();

            int index = AllGridOmens.FindIndex(g => g.Equals(grid));
            if (index != -1)
            {
                return;
            }

            AllGridOmens.Add(omen);
            IsEmpty = false;
        }

        public void AddOmen(Region region)
        {
            if (region == null)
            {
                return;
            }

            Region omen = region.GetCopy();

            int index = AllRegionOmens.FindIndex(g => g.Equals(region));
            if (index != -1)
            {
                return;
            }

            AllRegionOmens.Add(omen);
            IsEmpty = false;
        }
    }
}
