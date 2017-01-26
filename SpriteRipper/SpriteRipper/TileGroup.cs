using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteRipper
{
    class TileGroup : IEnumerator<int>, IEnumerable<int>
    {
        //public int MasterIndex { get; protected set; }
        public int MasterIndex { get { return Master.Index; } }
        public Tile Master { get; protected set; }
        protected int position = -1;
        protected SortedList<int, int> similarTiles { get; set; }
        public int Count { get { return similarTiles.Values.Count + 1; } }
        //public Object Current { get { return similarTiles[position]; } }
        Object IEnumerator.Current { get { return Current; } }
        public int Current { get { try { return similarTiles.Values[position]; } catch (IndexOutOfRangeException) { throw new InvalidOperationException(); } } }
        public int this[int index]
        {
            get { return similarTiles[index]; }
            set { similarTiles.Add(index, value); }
        }

        //public TileGroup(int MasterIndex)
        //{
        //    this.MasterIndex = MasterIndex;
        //    similarTiles = new SortedList<int, int>();
        //}
        public TileGroup(Tile master)
        {
            this.Master = master;
            similarTiles = new SortedList<int, int>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return (IEnumerator<int>)similarTiles.GetEnumerator();
        }

        public void Dispose()
        {
            similarTiles.Clear();
            similarTiles = null;
        }

        public bool MoveNext()
        {
            position++;
            bool moveNext = (position < similarTiles.Count);
            return moveNext;
        }

        public void Reset()
        {
            position = -1;
        }

        public bool ContainsKey(int key)
        {
            return similarTiles.ContainsKey(key);
        }

        public void AddSimilar(int key, int i)
        {
            similarTiles.Add(key, i);
        }

        public List<int> GetSortedTiles()
        {
            List<int> sortedTiles = new List<int>();
            foreach (int key in similarTiles.Keys)
            {
                int index = similarTiles[key];
                sortedTiles.Add(index);
            }

            return sortedTiles;
        }
    }
}
