using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker
{
    public class DrawState  // Caretaker
    {
        // Non memento pattern
        //public LinkedList<GridHistory> allUpdatedGrids { get; protected set; }
        // Memento pattern
        public List<Grid> AllMementos { get; protected set; } // previous states
        public List<Grid> AllOmens { get; protected set; }    // future states
        public bool IsEmpty { get; protected set; }

        /* Non memento pattern
        public DrawEvent(List<GridHistory> omens)
        {
            allUpdatedGrids = new LinkedList<GridHistory>(omens);
        }

        public DrawEvent(GridHistory grid) : this() {
            AddMemento(grid);
        }

        public DrawEvent()
        {
            allUpdatedGrids = new LinkedList<GridHistory>();
        }

        public void AddMemento(GridHistory grid)
        {
            grid.SaveState();
            allUpdatedGrids.AddLast(grid);
        }
        */

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

        //public void Undo()
        //{
        //    foreach (GridHistory grid in allUpdatedGrids)
        //    {
        //        grid.PreviousState();
        //    }
        //}

        //public void Redo()
        //{
        //    foreach (GridHistory grid in allUpdatedGrids)
        //    {
        //        grid.NextState();
        //    }
        //}
    }
}
