using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteRipper
{
    class TileSorting : IEnumerator<TileGroup>, IEnumerable<TileGroup>
    {
        public List<TileGroup> allTileGroups { get; set; }
        protected int position = -1;
        public int Count { get { return GroupCount; } }
        public int GroupCount
        {
            get
            {
                if (allTileGroups == null)
                {
                    return 0;
                }

                return allTileGroups.Count;
            }
        }
        public int TileCount
        {
            get
            {
                if (allTileGroups == null)
                {
                    return 0;
                }

                int count = 0;
                foreach (TileGroup group in allTileGroups)
                {
                    count += group.Count;
                }

                return count;
            }
        }
        public TileGroup this[int index]
        {
            get { return allTileGroups[index]; }
            set { allTileGroups.Insert(index, value); }
        }
        Object IEnumerator.Current { get { return Current; } }
        public TileGroup Current { get { try { return allTileGroups[position]; } catch (IndexOutOfRangeException) { throw new InvalidOperationException(); } } }
        //public Object Current { get { return allTileGroups[position]; } }

        public TileSorting()
        {
            allTileGroups = new List<TileGroup>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) this.GetEnumerator();
        }

        public IEnumerator<TileGroup> GetEnumerator()
        {
            return allTileGroups.GetEnumerator();
        }

        public void Dispose()
        {
            foreach (TileGroup group in allTileGroups)
            {
                group.Dispose();
            }

            allTileGroups.Clear();
            allTileGroups = null;
        }

        public bool MoveNext()
        {
            position++;
            bool moveNext = (position < allTileGroups.Count);
            return moveNext;
        }

        public void Reset()
        {
            position = -1;
        }

        public int GetMaxGroupSize()
        {
            int maxGroupSize = 0;
            if (allTileGroups == null)
            {
                return maxGroupSize;
            }

            foreach (TileGroup group in allTileGroups)
            {
                int groupSize = group.Count;
                if (groupSize <= maxGroupSize)
                {
                    continue;
                }

                maxGroupSize = groupSize;
            }

            return maxGroupSize;
        }

        //public void AddGroup(int masterIndex)
        //{
        //    TileGroup group = new TileGroup(masterIndex);
        //    allTileGroups.Add(group);
        //}
        public void AddGroup(Tile master)
        {
            TileGroup group = new TileGroup(master);
            allTileGroups.Add(group);
        }
    }
}
