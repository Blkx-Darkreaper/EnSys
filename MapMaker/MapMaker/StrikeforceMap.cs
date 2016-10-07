using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;

namespace MapMaker
{
    public class StrikeforceMap
    {
        // Meta data
        public string Author { get; set; }
        public DateTime DateCreated { get; set; }

        // Map data
        public string TilesetFilename { get; set; }
        public int TileLength { get; set; }
        public int NextSector { get; set; }
        public Size MapSize { get; set; }
        public List<Grid> AllMapGrids { get; set; }
        public List<Checkpoint> AllCheckpoints { get; set; }

        //public StrikeforceMap(string Author, DateTime dateCreated, string tilesetFilename, int tileLength, int nextSector, Size mapSize, List<GridHistory> allMapGridHistories) 
        //    : this(Author, dateCreated, tilesetFilename, tileLength, nextSector, mapSize, allMapGridHistories.Cast<Grid>().ToList()) { }

        [JsonConstructor]
        public StrikeforceMap(string author, DateTime dateCreated, string tilesetFilename, int tileLength, int nextSector, Size mapSize, List<Grid> allMapGrids, List<Checkpoint> allCheckpoints)
        {
            this.Author = author;
            this.DateCreated = dateCreated;
            this.TilesetFilename = tilesetFilename;
            this.TileLength = tileLength;
            Grid.NextSector = nextSector;
            this.MapSize = mapSize;
            this.AllMapGrids = allMapGrids;
            this.AllCheckpoints = allCheckpoints;
        }
    }
}
