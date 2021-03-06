﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace SpriteRipper
{
    public class ImageCollection
    {
        public string Filename { get; set; }
        public Point ImageOffset { get; protected set; }
        public Size CroppedImageSize { get; protected set; }
        public int TileSize { get; protected set; }
        public Size CurrentSubImageSize
        {
            get
            {
                if (CurrentSubImageIndex < 0)
                {
                    return new Size();
                }

                return GetSubImageSize(CurrentSubImageIndex);
            }
        }
        protected List<SubImage> allSubImages { get; set; }
        public int SubImagesWide { get; protected set; }
        public int TotalSubImages { get { return allSubImages.Count; } }
        public int CurrentSubImageIndex { get; protected set; }
        public Bitmap CurrentSubImage { get; protected set; }

        protected class SubImage
        {
            public int Index { get; set; }
            public bool IsSorted { get; set; }
            public Size Size { get; set; }

            public SubImage(int index, int width, int height)
            {
                this.Index = index;
                this.Size = new Size(width, height);
                this.IsSorted = false;
            }
        }

        public ImageCollection(string path, int tileSize, int width, int height, int offsetX, int offsetY)
        {
            this.Filename = path;
            this.CroppedImageSize = new Size(width, height);

            BuildImageCollection(tileSize, offsetX, offsetY, -1);
        }

        public void BuildImageCollection(int tileSize, int offsetX, int offsetY, int subImageIndex)
        {
            this.ImageOffset = new Point(offsetX, offsetY);
            this.TileSize = tileSize;
            this.allSubImages = new List<SubImage>();

            int imageWidth = CroppedImageSize.Width;
            int imageHeight = CroppedImageSize.Height;

            int widthDivisor = 1;
            int heightDivisor = 1;

            while (imageWidth / widthDivisor > 256)
            {
                widthDivisor *= 2;
            }

            while (imageHeight / heightDivisor > 256)
            {
                heightDivisor *= 2;
            }

            this.SubImagesWide = widthDivisor;

            int subImageWidth = imageWidth / widthDivisor;
            if (subImageWidth % tileSize != 0)
            {
                float tilesWide = subImageWidth / (float)tileSize;
                subImageWidth = (int)Math.Ceiling(tilesWide) * tileSize;
            }

            int subImageHeight = imageHeight / heightDivisor;
            if (subImageHeight % tileSize != 0)
            {
                float tilesHigh = subImageHeight / (float)tileSize;
                subImageHeight = (int)Math.Ceiling(tilesHigh) * tileSize;
            }

            int lastColumn = widthDivisor - 1;
            int lastRow = heightDivisor - 1;
            int totalSubImages = widthDivisor * heightDivisor;
            for (int i = 0; i < totalSubImages; i++)
            {
                int width = subImageWidth;
                if (i % widthDivisor == lastColumn)
                {
                    width = imageWidth - lastColumn * subImageWidth;
                }

                int height = subImageHeight;
                if (i / widthDivisor == lastRow)
                {
                    height = imageHeight - lastRow * subImageHeight;
                }

                SubImage subImageToAdd = new SubImage(i, width, height);
                allSubImages.Add(subImageToAdd);
            }

            this.CurrentSubImageIndex = subImageIndex;

            Bitmap image = GetImage();
            SetSubImageByRefFromImage(ref image, 0);
            image.Dispose();
        }

        public Size GetSubImageSize(int subImageIndex)
        {
            Size subImageSize = allSubImages[subImageIndex].Size;
            return subImageSize;
        }

        public Tuple<int, int> GetSubImageIndexPair(int tileIndex)
        {
            int subImageIndex = GetSubImageIndexFromTileIndex(tileIndex);
            int subImageTileIndex = GetSubImageTileIndexFromTileIndex(tileIndex, subImageIndex);

            Tuple<int, int> subImageIndexPair = new Tuple<int, int>(subImageIndex, subImageTileIndex);
            return subImageIndexPair;
        }

        public int GetSubImageIndexFromTileIndex(int tileIndex)
        {
            //TODO
            int totalTilesWide = CroppedImageSize.Width / TileSize;

            Size medianSubImageSize = allSubImages[0].Size;
            int subImageTilesWide = medianSubImageSize.Width / TileSize;
            int subImageTilesHigh = medianSubImageSize.Height / TileSize;

            int subImageIndex = tileIndex / (totalTilesWide * subImageTilesHigh) * SubImagesWide
                + (tileIndex % totalTilesWide) / subImageTilesWide;
            return subImageIndex;
        }

        protected int GetSubImageTileIndexFromTileIndex(int tileIndex, int subImageIndex)
        {
            //TODO
            int totalTilesWide = CroppedImageSize.Width / TileSize;

            Size medianSubImageSize = allSubImages[0].Size;
            int medianSubImageTilesWide = medianSubImageSize.Width / TileSize;
            int medianSubImageTilesHigh = medianSubImageSize.Height / TileSize;

            int subImageWidth = allSubImages[subImageIndex].Size.Width;
            int subImageTilesWide = subImageWidth / TileSize;

            int subImageTileIndex = (tileIndex % totalTilesWide) % medianSubImageTilesWide 
                + subImageTilesWide * ((tileIndex % (totalTilesWide * medianSubImageTilesHigh)) / totalTilesWide);
            return subImageTileIndex;
        }

        public int GetTileIndex(int subImageIndex, int subImageTileIndex)
        {
            int width = CroppedImageSize.Width;
            int totalTilesWide = width / TileSize;

            SubImage subImage = allSubImages[subImageIndex];

            int subImageWidth = CurrentSubImageSize.Width;
            int subImageHeight = CurrentSubImageSize.Height;

            int subImageTilesWide = subImageWidth / TileSize;

            int subImagesWide = (int)Math.Ceiling(width / (float)subImageWidth);

            Size medianSubImageSize = allSubImages[0].Size;
            
            int medianSubImageWidth = medianSubImageSize.Width;
            int medianSubImageTilesWide = medianSubImageWidth / TileSize;

            int medianSubImageHeight = medianSubImageSize.Height;
            int medianSubImageTilesHigh = medianSubImageHeight / TileSize;

            int tileIndex = subImageTileIndex % subImageTilesWide
                + subImageTileIndex / subImageTilesWide * totalTilesWide
                + subImageIndex / subImagesWide * medianSubImageTilesHigh * totalTilesWide
                + (subImageIndex % subImagesWide) * medianSubImageTilesWide;
            return tileIndex;
        }

        public Bitmap GetImage()
        {
            int offsetX = ImageOffset.X;
            int offsetY = ImageOffset.Y;
            Bitmap croppedImage = Program.LoadCroppedImage(Filename, TileSize, offsetX, offsetY);
            return croppedImage;
        }

        public Bitmap LoadSubImage(int subImageIndex)
        {
            Bitmap subImage = GetSubImage(subImageIndex);

            SetSubImageByRef(ref subImage, subImageIndex);

            return subImage;
        }

        public Bitmap GetSubImage(int subImageIndex)
        {
            if (subImageIndex == CurrentSubImageIndex)
            {
                return CurrentSubImage;
            }

            Bitmap croppedImage = GetImage();
            Bitmap subImage = GetSubImageFromImageByRef(ref croppedImage, subImageIndex);
            croppedImage.Dispose();

            return subImage;
        }

        public Bitmap GetSubImageFromImageByRef(ref Bitmap image, int subImageIndex)
        {
            int width = image.Width;
            int height = image.Height;

            Size subImageSize = allSubImages[subImageIndex].Size;
            int subImageWidth = subImageSize.Width;
            int subImageHeight = subImageSize.Height;

            Bitmap subImage;
            if (width == subImageWidth)
            {
                if (height == subImageHeight)
                {
                    subImage = new Bitmap(width, height);
                    using (Graphics graphics = Graphics.FromImage(subImage))
                    {
                        graphics.DrawImage(image, 0, 0);
                    }

                    return subImage;
                }
            }

            Size medianSubImageSize = allSubImages[0].Size;
            int medianSubImageWidth = medianSubImageSize.Width;
            int medianSubImageHeight = medianSubImageSize.Height;

            int x = (subImageIndex % SubImagesWide) * medianSubImageWidth;
            int y = (subImageIndex / SubImagesWide) * medianSubImageHeight;
            Rectangle rect = new Rectangle(0, 0, subImageWidth, subImageHeight);
            subImage = new Bitmap(subImageWidth, subImageHeight);
            using (Graphics graphics = Graphics.FromImage(subImage))
            {
                graphics.DrawImage(image, rect, x, y, subImageWidth, subImageHeight, GraphicsUnit.Pixel);
            }

            return subImage;
        }

        public void SetSubImageByRefFromImage(ref Bitmap image, int subImageIndex)
        {
            if (CurrentSubImageIndex == subImageIndex)
            {
                return;
            }

            Bitmap subImage = GetSubImageFromImageByRef(ref image, subImageIndex);

            this.CurrentSubImage = subImage;
            this.CurrentSubImageIndex = subImageIndex;
        }

        public void SetSubImageByRef(ref Bitmap subImage, int subImageIndex)
        {
            CurrentSubImage = subImage;
            CurrentSubImageIndex = subImageIndex;
        }

        public int GetSubImageTileCount(int subImageIndex)
        {
            Size subImageSize = allSubImages[subImageIndex].Size;

            int subImageWidth = subImageSize.Width;
            int subImageHeight = subImageSize.Height;

            int subImageTilesWide = subImageWidth / TileSize;
            int subImageTilesHigh = subImageHeight / TileSize;

            int totalSubImageTiles = subImageTilesWide * subImageTilesHigh;
            return totalSubImageTiles;
        }

        public bool GetSubImageSorted(int subImageIndex)
        {
            bool isSorted = allSubImages[subImageIndex].IsSorted;
            return isSorted;
        }

        public void SetSubImageSorted(int subImageIndex, bool isSorted)
        {
            allSubImages[subImageIndex].IsSorted = isSorted;
        }

        public bool IsSubImageSorted(int subImageIndex)
        {
            bool isSorted = allSubImages[subImageIndex].IsSorted;
            return isSorted;
        }

        public Bitmap GetTileImage(int tileIndex)
        {
            int subImageIndex = GetSubImageIndexFromTileIndex(tileIndex);

            int tileSubImageIndex = GetSubImageTileIndexFromTileIndex(tileIndex, subImageIndex);

            Bitmap tileImage = GetTileImage(subImageIndex, tileSubImageIndex);
            return tileImage;
        }

        public Bitmap GetTileImage(int subImageIndex, int subImageTileIndex)
        {
            Bitmap subImage = GetSubImage(subImageIndex);

            int subImageWidth = subImage.Width;

            int tilesWide = subImageWidth / TileSize;

            int x = (subImageTileIndex % tilesWide) * TileSize;
            int y = (subImageTileIndex / tilesWide) * TileSize;

            Rectangle rect = new Rectangle(0, 0, TileSize, TileSize);
            Bitmap tileImage = new Bitmap(TileSize, TileSize);
            using (Graphics graphics = Graphics.FromImage(tileImage))
            {
                graphics.DrawImage(subImage, rect, x, y, TileSize, TileSize, GraphicsUnit.Pixel);
            }

            return tileImage;
        }

        public PixelFormat GetPixelFormat()
        {
            if (CurrentSubImage == null)
            {
                throw new NullReferenceException();
            }

            PixelFormat format = CurrentSubImage.PixelFormat;
            return format;
        }
    }
}