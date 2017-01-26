using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SpriteRipper
{
    public class Tile : IComparable<Tile>
    {
        public int Index { get; protected set; }
        //public Bitmap loadedImage { get; set; }
        protected List<int> pattern { get; set; }
        public int BitsPerColour { get; protected set; }
        //protected int cornerX { get; set; }
        //protected int cornerY { get; set; }
        protected int tileSize { get; set; }

        public Tile(Bitmap image, int bitsPerColour, int tileSize)
        {
            //this.loadedImage = loadedImage;
            this.BitsPerColour = bitsPerColour;
            this.tileSize = tileSize;
            this.pattern = GetPattern(image, bitsPerColour, tileSize);
            image.Dispose();
            //loadedImage = null;
        }

        public Tile(Bitmap image, int bitsPerColour, int tileSize, int index) : this(image, bitsPerColour, tileSize)
        {
            this.Index = index;
        }

        //public Tile(Bitmap canvas, int BitsPerColour, int x, int y, int TileSize)
        //    : this(canvas, BitsPerColour, TileSize)
        //{
        //    this.cornerX = x;
        //    this.cornerY = y;
        //    this.TileSize = TileSize;
        //}

        //public Tile(Bitmap canvas, int BitsPerColour, int tileIndex, int x, int y, int TileSize) : this(canvas, BitsPerColour, TileSize, tileIndex)
        //{
        //    this.cornerX = x;
        //    this.cornerY = y;
        //    this.TileSize = TileSize;
        //}

        public int CompareTo(Tile other)
        {
            if (other == null)
            {
                return 1;
            }

            return Index.CompareTo(other.Index);
        }

        public override string ToString()
        {
            string message = "{";
            bool first = true;
            foreach (int number in pattern)
            {
                if(first == false) {
                    message += ", ";
                }

                message += number;
                first = false;
            }

            message += "}";
            return message;
        }

        public bool IdenticalTo(Tile otherTile)
        {
            Tuple<float, float> results = GetMatches(otherTile);
            return IdenticalTo(results);
        }

        public static bool IdenticalTo(Tuple<float, float> results)
        {
            float patternMatch = results.Item1;
            float colourMatch = results.Item2;
            return IdenticalTo(patternMatch, colourMatch);
        }

        public static bool IdenticalTo(float patternMatch, float colourMatch)
        {
            if (patternMatch < 1)
            {
                return false;
            }

            if (colourMatch < 1)
            {
                return false;
            }

            return true;
        }

        public bool SimilarTo(Tile otherTile, float patternThreshold, float colourThreshold)
        {
            Tuple<float, float> results = GetMatches(otherTile);
            float patternMatch = results.Item1;
            if (patternMatch < patternThreshold)
            {
                return false;
            }

            return true;
        }

        public static bool SimilarTo(float patternMatch, float patternThreshold)
        {
            if (patternMatch < patternThreshold)
            {
                return false;
            }

            return true;
        }

        public static int GetHashcode(int accuracy, float patternMatch, float colourMatch, int tileCount)
        {
            int multiplier = (int)Math.Pow(10, accuracy);

            int patternCode = multiplier - (int)Math.Round(patternMatch * multiplier);
            int colourCode = multiplier - (int)Math.Round(colourMatch * multiplier);

            // Reduce tileNumber by 1 for tileToDraw, and 1 for first match
            int digits = (int)Math.Ceiling(Math.Log10(tileCount - 2));

            int hashcode = (10 * multiplier * patternCode + colourCode) * (int)Math.Pow(10, digits);
            return hashcode;
        }

        public Bitmap GetTileImage()
        {
            //Bitmap canvas = Program.GetTileImage(cornerX, cornerY, TileSize);
            Bitmap image = Program.GetTileImage(Index);
            return image;
        }

        protected List<int> GetPattern(Bitmap image, int bitsPerColour, int tileSize)
        {
            int? previousPixelInt = null;

            List<int> pattern = new List<int>();
            for (int y = 0; y < tileSize; y++)
            {
                for (int x = 0; x < tileSize; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    int red = pixel.R;
                    int green = pixel.G;
                    int blue = pixel.B;

                    int currentPixelInt = GetCompressedColourInt(red, green, blue, bitsPerColour);

                    if (previousPixelInt != null)
                    {
                        int difference = currentPixelInt - (int)previousPixelInt;
                        pattern.Add(difference);
                    }

                    previousPixelInt = currentPixelInt;
                }
            }

            return pattern;
        }

        protected int GetCompressedColourInt(int red, int green, int blue, int bitsPerColour)
        {
            int pixelInt;

            if (bitsPerColour == 8)
            {
                pixelInt = GetPixelInteger(red, green, blue, bitsPerColour);
                return pixelInt;
            }

            int compressionFactor = (int)Math.Pow(2, 8 - bitsPerColour);

            int compressedRed = (int)Math.Ceiling((double)((red + 1) / compressionFactor)) - 1;
            int compressedGreen = (int)Math.Ceiling((double)((green + 1) / compressionFactor)) - 1;
            int compressedBlue = (int)Math.Ceiling((double)((blue + 1) / compressionFactor)) - 1;

            pixelInt = GetPixelInteger(compressedRed, compressedGreen, compressedBlue, bitsPerColour);
            return pixelInt;
        }

        protected int GetPixelInteger(int red, int green, int blue, int bitsPerColour)
        {
            int multiplier = (int)Math.Pow(2, bitsPerColour);

            int shiftedRed = red * (int)Math.Pow(multiplier, 2);
            int shiftedGreen = green * multiplier;

            int pixelInt = shiftedRed + shiftedGreen + blue;
            return pixelInt;
        }

        public Tuple<float, float> GetMatches(Tile otherTile)
        {
            //List<bool> patternMatches = GetPatternMatches(otherTile);
            //float patternMatch = GetPatternMatch(patternMatches);
            float patternMatch = 0f;
            List<bool> patternMatches = GetPatternMatchAndMatches(otherTile, out patternMatch);

            //List<int?> colourMatches = GetColourMatches(otherTile, patternMatches); //debug
            //float colourMatch2 = GetColourMatch(colourMatches); //debug
            float colourMatch = GetColourMatch(otherTile, patternMatches);

            //if (colourMatch2 != colourMatch)
            //{
            //    Console.WriteLine("Failed");
            //}

            Tuple<float, float> results = new Tuple<float, float>(patternMatch, colourMatch);
            return results;
        }

        protected List<bool> GetPatternMatchAndMatches(Tile otherTile, out float patternMatch)
        {
            // Compare bitCompression
            if (BitsPerColour != otherTile.BitsPerColour)
            {
                throw new Exception("Bits per colour does not match");
            }

            // Compare tileToCompare sizes
            if (tileSize != otherTile.tileSize)
            {
                throw new Exception("Tile sizes do not match");
            }

            int totalMatches = 0;
            List<bool> patternMatches = new List<bool>();
            for (int i = 0; i < pattern.Count; i++)
            {
                int diff = pattern[i];
                int otherDiff = otherTile.pattern[i];

                if (diff == otherDiff)
                {
                    patternMatches.Add(true);
                    totalMatches++;
                }
                else
                {
                    patternMatches.Add(false);
                }
            }

            patternMatch = (float)totalMatches / (float)patternMatches.Count;

            return patternMatches;
        }

        protected List<bool> GetPatternMatches(Tile otherTile)
        {
            float patternMatch = 0f;
            return GetPatternMatchAndMatches(otherTile, out patternMatch);
        }

        protected float GetPatternMatch(Tile otherTile)
        {
            // Compare bitCompression
            if (BitsPerColour != otherTile.BitsPerColour)
            {
                throw new Exception("Bits per colour does not match");
            }

            // Compare tileToCompare sizes
            if (tileSize != otherTile.tileSize)
            {
                throw new Exception("Tile sizes do not match");
            }

            int totalMatches = 0;
            for (int i = 0; i < pattern.Count; i++)
            {
                int diff = pattern[i];
                int otherDiff = otherTile.pattern[i];

                if (diff != otherDiff)
                {
                    continue;
                }

                totalMatches++;
            }

            float matchPercentage = (float)totalMatches / (float)pattern.Count;
            return matchPercentage;
        }

        public static float GetPatternMatch(List<bool> patternMatches)
        {
            int totalMatches = 0;
            foreach (bool match in patternMatches)
            {
                if (match == false)
                {
                    continue;
                }

                totalMatches++;
            }

            float matchPercentage = (float)totalMatches / (float)patternMatches.Count;
            return matchPercentage;
        }

        public List<int?> GetColourMatches(Tile otherTile)
        {
            List<bool> patternMatches = GetPatternMatches(otherTile);

            return GetColourMatches(otherTile, patternMatches);
        }

        public List<int?> GetColourMatches(Tile otherTile, List<bool> patternMatches)
        {
            List<int?> colourMatches = new List<int?>();

            //Bitmap canvas = Program.GetTileImage(cornerX, cornerY, TileSize);

            //int otherX = otherTile.cornerX;
            //int otherY = otherTile.cornerY;
            //Bitmap otherImage = Program.GetTileImage(otherX, otherY, TileSize);
            Bitmap image = Program.GetTileImage(Index);

            using (Bitmap otherImage = Program.GetTileImage(otherTile.Index))
            {
                for (int y = 0; y < tileSize; y++)
                {
                    for (int x = 0; x < tileSize; x++)
                    {
                        int matchIndex = x + tileSize * y - 1;

                        if (matchIndex != -1)
                        {
                            bool match = patternMatches[matchIndex];

                            if (match == true)
                            {
                                colourMatches.Add(null);
                                continue;
                            }
                        }

                        Color pixel = image.GetPixel(x, y);
                        Color otherPixel = otherImage.GetPixel(x, y);

                        int pixelInt = pixel.ToArgb();
                        int otherPixelInt = otherPixel.ToArgb();

                        int difference = otherPixelInt - pixelInt;
                        colourMatches.Add(difference);
                    }
                }
            }

            return colourMatches;
        }

        public float GetColourMatch(Tile otherTile)
        {
            List<bool> patternMatches = GetPatternMatches(otherTile);
            return GetColourMatch(otherTile, patternMatches);
        }

        public float GetColourMatch(Tile otherTile, List<bool> patternMatches)
        {
            // Group and tileNumber all matches
            Dictionary<int, int> groups = new Dictionary<int, int>();
            int previousMatch = 0;

            List<int?> colourMatches = new List<int?>();

            //Bitmap canvas = Program.GetTileImage(cornerX, cornerY, TileSize);

            //int otherX = otherTile.cornerX;
            //int otherY = otherTile.cornerY;
            //Bitmap otherImage = Program.GetTileImage(otherX, otherY, TileSize);
            Bitmap image = Program.GetTileImage(Index);
            using (Bitmap otherImage = Program.GetTileImage(otherTile.Index))
            {
                for (int y = 0; y < tileSize; y++)
                {
                    for (int x = 0; x < tileSize; x++)
                    {
                        int matchIndex = x + tileSize * y - 1;

                        int key;
                        bool hasKey;

                        if (matchIndex != -1)
                        {
                            bool match = patternMatches[matchIndex];

                            if (match == true)
                            {
                                colourMatches.Add(null);
                                key = previousMatch;

                                hasKey = groups.ContainsKey(key);
                                if (hasKey == true)
                                {
                                    int count = groups[key];
                                    groups[key] = count + 1;
                                }
                                else
                                {
                                    groups.Add(key, 1);
                                }

                                continue;
                            }
                        }

                        Color pixel = image.GetPixel(x, y);
                        Color otherPixel = otherImage.GetPixel(x, y);

                        int pixelInt = pixel.ToArgb();
                        int otherPixelInt = otherPixel.ToArgb();

                        int difference = otherPixelInt - pixelInt;
                        colourMatches.Add(difference);
                        key = difference;

                        hasKey = groups.ContainsKey(key);
                        if (hasKey == true)
                        {
                            int count = groups[key];
                            groups[key] = count + 1;
                        }
                        else
                        {
                            groups.Add(key, 1);
                        }

                        previousMatch = key;
                    }
                }
            }

            // Get the mode
            int maxCount = groups.Max(g => g.Value);
            int mode = groups.First(g => g.Value == maxCount).Key;

            // Count how many matches match the mode
            int totalMatches = 0;
            previousMatch = 0;
            foreach (int? nullableMatch in colourMatches)
            {
                int match;
                if (nullableMatch == null)
                {
                    match = previousMatch;
                }
                else
                {
                    match = (int)nullableMatch;
                }

                previousMatch = match;

                if (match != mode)
                {
                    continue;
                }

                totalMatches++;
            }

            float maxDiff = 16777215;

            float colourMatch = (float)totalMatches / (float)colourMatches.Count - (float)mode / maxDiff;
            return colourMatch;
        }

        public float GetColourMatch(List<int?> colourMatches)
        {
            // Group and tileNumber all matches
            Dictionary<int, int> groups = new Dictionary<int, int>();
            int previousMatch = 0;
            foreach (int? match in colourMatches)
            {
                int key;
                if (match == null)
                {
                    key = previousMatch;
                }
                else
                {
                    key = (int)match;
                }

                bool hasKey = groups.ContainsKey(key);
                if (hasKey == true)
                {
                    int count = groups[key];
                    groups[key] = count + 1;
                }
                else
                {
                    groups.Add(key, 1);
                }

                previousMatch = key;
            }

            // Get the mode
            int maxCount = groups.Max(g => g.Value);
            int mode = groups.First(g => g.Value == maxCount).Key;

            // Count how many matches match the mode
            int totalMatches = 0;
            previousMatch = 0;
            foreach (int? nullableMatch in colourMatches)
            {
                int match;
                if (nullableMatch == null)
                {
                    match = previousMatch;
                }
                else
                {
                    match = (int)nullableMatch;
                }

                previousMatch = match;

                if (match != mode)
                {
                    continue;
                }

                totalMatches++;
            }

            float maxDiff = 16777215;

            float colourMatch = (float)totalMatches / (float)colourMatches.Count - (float)mode / maxDiff;
            return colourMatch;
        }
    }
}