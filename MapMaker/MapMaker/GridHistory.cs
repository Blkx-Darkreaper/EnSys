using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MapMaker
{
    public class GridHistory : Grid
    {
        protected int maxStates { get; set; }
        protected List<Grid> states { get; set; }
        protected int currentStateIndex { get; set; }
        protected int firstStateIndex { get; set; }
        protected int lastStateIndex { get; set; }
        public bool hasChanged { get; set; }

        public GridHistory(Grid grid) :base(grid)
        {
            Init();
        }

        public GridHistory(Point corner, Tile tile) : base(corner, tile)
        {
            Init();
        }

        protected virtual void Init()
        {
            this.maxStates = 10;
            this.states = new List<Grid>(maxStates);
            this.currentStateIndex = -1;
            this.firstStateIndex = -1;
            this.lastStateIndex = -1;
            this.hasChanged = true;

            SaveState();
        }

        public virtual void SaveState()
        {
            Grid copy = GetCopy();

            currentStateIndex++;
            currentStateIndex %= maxStates;

            if (currentStateIndex == firstStateIndex)
            {
                firstStateIndex++;
                firstStateIndex %= maxStates;
            }

            lastStateIndex = currentStateIndex;

            if (states.Count == 0)
            {
                firstStateIndex = 0;
            }

            if (states.Count > currentStateIndex)
            {
                states[currentStateIndex] = copy;
            }
            else
            {
                states.Add(copy);
            }
        }

        public virtual void PreviousState()
        {
            if (currentStateIndex == firstStateIndex)
            {
                return; // No previous nextState
            }

            currentStateIndex--;
            if (currentStateIndex < 0)
            {
                currentStateIndex += maxStates;
            }

            Grid copy = states.ElementAt(currentStateIndex);
            base.MatchCopy(copy);
        }

        public virtual void NextState()
        {
            if (currentStateIndex == lastStateIndex)
            {
                return; // No next nextState
            }

            currentStateIndex++;
            currentStateIndex %= maxStates;

            Grid copy = states.ElementAt(currentStateIndex);
            base.MatchCopy(copy);
        }
    }
}
