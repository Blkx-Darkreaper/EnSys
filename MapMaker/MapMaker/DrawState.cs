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
        public LinkedList<Grid> allMementos { get; protected set; } // previous states
        public LinkedList<Grid> allOmens { get; protected set; }    // future states

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
            allMementos = new LinkedList<Grid>();
            allOmens = new LinkedList<Grid>();
        }

        public void AddMemento(Grid grid)
        {
            if (grid == null)
            {
                return;
            }

            Grid memento = grid.GetCopy();
            allMementos.AddLast(memento);
        }

        public void AddOmen(Grid grid)
        {
            if (grid == null)
            {
                return;
            }

            Grid omen = grid.GetCopy();
            allOmens.AddLast(omen);
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
