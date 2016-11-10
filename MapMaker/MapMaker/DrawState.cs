using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker
{
    public class DrawState  // Caretaker
    {
        public List<Grid> AllMementos { get; protected set; } // previous states
        public List<Grid> AllOmens { get; protected set; }    // future states
        public bool IsEmpty { get; protected set; }

        public DrawState(Grid grid)
            : this()
        {
            AddMemento(grid);
        }

        public DrawState()
        {
            AllMementos = new List<Grid>();
            AllOmens = new List<Grid>();
            IsEmpty = true;
        }

        public void AddMemento(Grid grid)
        {
            if (grid == null)
            {
                return;
            }

            Grid memento = grid.GetCopy();

            int index = AllMementos.FindIndex(g => g.Equals(grid));
            if (index != -1)
            {
                //AllMementos[index] = memento;
                return;
            }

            AllMementos.Add(memento);
            IsEmpty = false;
        }

        public void AddOmen(Grid grid)
        {
            if (grid == null)
            {
                return;
            }

            Grid omen = grid.GetCopy();

            int index = AllOmens.FindIndex(g => g.Equals(grid));
            if (index != -1)
            {
                return;
            }

            AllOmens.Add(omen);
            IsEmpty = false;
        }
    }
}
