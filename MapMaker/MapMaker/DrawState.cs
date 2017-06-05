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
        public List<Zone> AllZoneMementos { get; protected set; }
        // future states
        public List<Grid> AllGridOmens { get; protected set; }
        public List<Zone> AllZoneOmens { get; protected set; }
        public bool IsEmpty { get; protected set; }

        public DrawState(Grid grid)
            : this()
        {
            AddMemento(grid);
        }

        public DrawState()
        {
            AllGridMementos = new List<Grid>();
            AllZoneMementos = new List<Zone>();
            AllGridOmens = new List<Grid>();
            AllZoneOmens = new List<Zone>();
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

        public void AddMemento(Zone zone)
        {
            if (zone == null)
            {
                return;
            }

            Zone memento = zone.GetCopy();

            int index = AllZoneMementos.FindIndex(g => g.Equals(zone));
            if (index != -1)
            {
                //AllMementos[index] = memento;
                return;
            }

            AllZoneMementos.Add(memento);
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

        public void AddOmen(Zone zone)
        {
            if (zone == null)
            {
                return;
            }

            Zone omen = zone.GetCopy();

            int index = AllZoneOmens.FindIndex(g => g.Equals(zone));
            if (index != -1)
            {
                return;
            }

            AllZoneOmens.Add(omen);
            IsEmpty = false;
        }
    }
}
